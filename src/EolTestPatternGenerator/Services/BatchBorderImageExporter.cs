using EolTestPatternGenerator.Models;
using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

/// <summary>
/// 批量给一个文件夹内的图片叠加白框。计划阶段会先冻结输入文件列表，
/// 因此即使输出目录后来出现新文件，也不会在同一批次中被再次当作输入。
/// </summary>
public static class BatchBorderImageExporter
{
    private static readonly StringComparer PathComparer = StringComparer.OrdinalIgnoreCase;

    /// <summary>
    /// 扫描源文件夹当前层的可读写图片，并计算保持原文件名、原扩展名的输出路径。
    /// 源目录与输出目录必须不同，防止批量任务覆盖原图。
    /// </summary>
    public static BatchBorderExportPlan CreatePlan(string sourceDirectory, string outputDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);

        string source = NormalizeDirectory(sourceDirectory);
        string output = NormalizeDirectory(outputDirectory);
        if (!Directory.Exists(source))
        {
            throw new DirectoryNotFoundException($"找不到源图片文件夹：{source}");
        }

        if (PathComparer.Equals(source, output) ||
            Directory.Exists(output) && WindowsFileSystemIdentity.AreSameDirectory(source, output))
        {
            throw new InvalidOperationException(
                "源图片文件夹与输出文件夹不能相同，否则会覆盖原图。请选择另一个输出文件夹。");
        }

        // 先完整物化列表，再开始任何写盘操作；这也是源、输出目录存在嵌套关系时的安全边界。
        BatchBorderExportItem[] items = Directory
            .EnumerateFiles(source, "*", SearchOption.TopDirectoryOnly)
            .Select(path => new { Path = Path.GetFullPath(path), Format = TryGetFormat(path) })
            .Where(item => item.Format.HasValue)
            .OrderBy(item => Path.GetFileName(item.Path), StringComparer.CurrentCultureIgnoreCase)
            .Select(item => new BatchBorderExportItem(
                item.Path,
                Path.Combine(output, Path.GetFileName(item.Path)),
                item.Format!.Value))
            .ToArray();

        string[] existingOutputs = items
            .Where(item => File.Exists(item.OutputPath))
            .Select(item => item.OutputPath)
            .ToArray();

        return new BatchBorderExportPlan(source, output, items, existingOutputs);
    }

    /// <summary>
    /// 按冻结计划逐张解码、绘框并原子保存。单张图片失败不会中止其余文件，
    /// 调用方可通过返回结果一次展示所有失败项。
    /// </summary>
    public static BatchBorderExportResult Export(
        BatchBorderExportPlan plan,
        BorderOverlaySettings borderSettings,
        int lossyQuality,
        bool overwriteExisting,
        IProgress<BatchBorderExportProgress>? progress = null)
    {
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(borderSettings);

        if (!overwriteExisting && plan.ExistingOutputPaths.Count > 0)
        {
            throw new IOException(
                $"输出目录已有 {plan.ExistingOutputPaths.Count} 个同名文件，尚未获得覆盖确认。");
        }

        Directory.CreateDirectory(plan.OutputDirectory);
        // 与批量二合一共用目录租约：整个批次不共享 Delete，防止计划完成后
        // 把输出目录替换为指向源目录的 junction/符号链接。
        using WindowsFileSystemIdentity.DirectoryIdentityLease directoryLease =
            WindowsFileSystemIdentity.AcquireIndependentDirectoryLease(
                plan.SourceDirectory,
                plan.OutputDirectory);

        // “批量加白框”按钮本身就表示启用叠加；只复用编辑器中的四边距和线宽。
        BorderOverlaySettings border = borderSettings.Clone();
        border.Enabled = true;

        var outputPaths = new List<string>(plan.Items.Count);
        var failures = new List<BatchBorderExportFailure>();
        for (int index = 0; index < plan.Items.Count; index++)
        {
            BatchBorderExportItem item = plan.Items[index];
            try
            {
                TiffPageValidator.EnsureSinglePage(
                    item.SourcePath,
                    item.Format == ImageFormatKind.Tiff);
                using Mat image = UnicodeImageLoader.LoadUnchanged(item.SourcePath);
                ValidateEditableImage(image, item.SourcePath);
                // GetRegion 会针对每张图片自己的尺寸解析四边距，支持同一批次中的混合分辨率。
                BorderOverlayRenderer.Apply(image, border);
                var exportOptions = new ImageExportOptions
                {
                    Format = item.Format,
                    Quality = lossyQuality
                };

                string actualPath = overwriteExisting
                    ? ImageFileWriter.Write(item.OutputPath, image, exportOptions)
                    : WriteWithoutOverwrite(item.OutputPath, image, exportOptions);
                outputPaths.Add(actualPath);
            }
            catch (Exception exception)
            {
                failures.Add(new BatchBorderExportFailure(
                    item.SourcePath,
                    item.OutputPath,
                    exception.Message));
            }
            finally
            {
                progress?.Report(new BatchBorderExportProgress(
                    index + 1,
                    plan.Items.Count,
                    Path.GetFileName(item.SourcePath)));
            }
        }

        return new BatchBorderExportResult(outputPaths, failures);
    }

    /// <summary>把现有支持的图片扩展名映射到对应编码器，保留 .jpeg/.tif 等原扩展名。</summary>
    public static ImageFormatKind? TryGetFormat(string path)
    {
        return Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".png" => ImageFormatKind.Png,
            ".jpg" or ".jpeg" => ImageFormatKind.Jpeg,
            ".bmp" => ImageFormatKind.Bmp,
            ".tif" or ".tiff" => ImageFormatKind.Tiff,
            ".webp" => ImageFormatKind.WebP,
            _ => null
        };
    }

    /// <summary>
    /// 未经覆盖确认时先写唯一暂存文件，再用不允许覆盖的原子移动占用正式文件名。
    /// 这样可防止“计划完成后、实际保存前”新出现的同名文件被静默替换。
    /// </summary>
    private static string WriteWithoutOverwrite(
        string outputPath,
        Mat image,
        ImageExportOptions exportOptions)
    {
        string target = Path.GetFullPath(outputPath);
        string directory = Path.GetDirectoryName(target)
                           ?? throw new InvalidOperationException("输出图片路径没有有效目录。");
        string extension = Path.GetExtension(target);
        string stagingPath = Path.Combine(
            directory,
            $".{Path.GetFileNameWithoutExtension(target)}.{Guid.NewGuid():N}{extension}");

        string? actualStagingPath = null;
        try
        {
            actualStagingPath = ImageFileWriter.Write(stagingPath, image, exportOptions);
            File.Move(actualStagingPath, target, overwrite: false);
            actualStagingPath = null;
            return target;
        }
        finally
        {
            try
            {
                if (!string.IsNullOrEmpty(actualStagingPath) && File.Exists(actualStagingPath))
                {
                    File.Delete(actualStagingPath);
                }
            }
            catch
            {
                // 暂存文件清理失败不能覆盖实际导出错误。
            }
        }
    }

    private static string NormalizeDirectory(string directory)
    {
        return Path.TrimEndingDirectorySeparator(Path.GetFullPath(directory));
    }

    private static void ValidateEditableImage(Mat image, string sourcePath)
    {
        MatType type = image.Type();
        if (type != MatType.CV_8UC1 && type != MatType.CV_8UC3 && type != MatType.CV_8UC4)
        {
            throw new InvalidDataException(
                $"批量白框只处理 8 位灰度、BGR 或 BGRA 图片；不会静默转换高位深或其他像素格式：{sourcePath}");
        }
    }
}

public sealed class BatchBorderExportPlan
{
    internal BatchBorderExportPlan(
        string sourceDirectory,
        string outputDirectory,
        IReadOnlyList<BatchBorderExportItem> items,
        IReadOnlyList<string> existingOutputPaths)
    {
        SourceDirectory = sourceDirectory;
        OutputDirectory = outputDirectory;
        Items = items;
        ExistingOutputPaths = existingOutputPaths;
    }

    public string SourceDirectory { get; }

    public string OutputDirectory { get; }

    public IReadOnlyList<BatchBorderExportItem> Items { get; }

    public IReadOnlyList<string> ExistingOutputPaths { get; }
}

public sealed record BatchBorderExportItem(
    string SourcePath,
    string OutputPath,
    ImageFormatKind Format);

public sealed record BatchBorderExportProgress(int CompletedCount, int TotalCount, string FileName);

public sealed record BatchBorderExportFailure(
    string SourcePath,
    string OutputPath,
    string ErrorMessage);

public sealed class BatchBorderExportResult
{
    internal BatchBorderExportResult(
        IReadOnlyList<string> outputPaths,
        IReadOnlyList<BatchBorderExportFailure> failures)
    {
        OutputPaths = outputPaths;
        Failures = failures;
    }

    public IReadOnlyList<string> OutputPaths { get; }

    public IReadOnlyList<BatchBorderExportFailure> Failures { get; }
}
