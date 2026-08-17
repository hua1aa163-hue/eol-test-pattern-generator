using EolTestPatternGenerator.Models;
using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

/// <summary>
/// 将一个文件夹当前层的图片批量水平复制为“二合一”图片。
/// 计划阶段先冻结全部输入和覆盖风险，执行阶段不会重新扫描目录。
/// </summary>
public static class BatchTwoInOneImageExporter
{
    private static readonly StringComparer PathComparer = StringComparer.OrdinalIgnoreCase;

    /// <summary>
    /// 冻结源文件夹当前层的 PNG、JPEG、BMP、TIFF 和 WebP 图片，
    /// 并保留每个文件的原文件名及原扩展名计算输出路径。
    /// </summary>
    public static BatchTwoInOneExportPlan CreatePlan(string sourceDirectory, string outputDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);

        string source = NormalizeDirectory(sourceDirectory);
        string output = NormalizeDirectory(outputDirectory);
        EnsureDirectoriesAreIndependent(source, output);

        BatchTwoInOneExportItem[] items = Directory
            .EnumerateFiles(source, "*", SearchOption.TopDirectoryOnly)
            .Select(path => new { Path = Path.GetFullPath(path), Format = TryGetFormat(path) })
            .Where(item => item.Format.HasValue)
            .OrderBy(item => Path.GetFileName(item.Path), StringComparer.OrdinalIgnoreCase)
            .Select(item => new BatchTwoInOneExportItem(
                item.Path,
                Path.Combine(output, Path.GetFileName(item.Path)),
                item.Format!.Value))
            .ToArray();

        string[] existingOutputs = items
            .Where(item => File.Exists(item.OutputPath))
            .Select(item => item.OutputPath)
            .ToArray();

        return new BatchTwoInOneExportPlan(source, output, items, existingOutputs);
    }

    /// <summary>
    /// 执行冻结计划。PNG、BMP、单页 TIFF 的 8 位灰度、BGR、BGRA 会保持原通道类型；
    /// JPEG/WebP、高位深、多页 TIFF 或编码器无法保留通道的文件会记录为单项失败，
    /// 不影响其余图片继续处理。
    /// </summary>
    public static BatchTwoInOneExportResult Export(
        BatchTwoInOneExportPlan plan,
        bool overwriteExisting,
        IProgress<BatchTwoInOneExportProgress>? progress = null)
    {
        ArgumentNullException.ThrowIfNull(plan);

        // 集中预检在写入任何图片前完成，调用方可据此只询问用户一次。
        if (!overwriteExisting && plan.ExistingOutputPaths.Count > 0)
        {
            throw new IOException(
                $"输出目录已有 {plan.ExistingOutputPaths.Count} 个同名文件，尚未获得覆盖确认。");
        }

        Directory.CreateDirectory(plan.OutputDirectory);
        // 整个批次持有不共享 Delete 的目标和路径入口句柄。除了再次验证物理身份，
        // 还会阻止执行期间把目录替换为指向源目录的 junction/符号链接。
        using WindowsFileSystemIdentity.DirectoryIdentityLease directoryLease =
            WindowsFileSystemIdentity.AcquireIndependentDirectoryLease(
                plan.SourceDirectory,
                plan.OutputDirectory);

        var outputPaths = new List<string>(plan.Items.Count);
        var failures = new List<BatchTwoInOneExportFailure>();
        for (int index = 0; index < plan.Items.Count; index++)
        {
            BatchTwoInOneExportItem item = plan.Items[index];
            try
            {
                ValidateLosslessFormat(item);
                TiffPageValidator.EnsureSinglePage(
                    item.SourcePath,
                    item.Format == ImageFormatKind.Tiff);
                using Mat source = UnicodeImageLoader.LoadUnchanged(item.SourcePath);
                ValidateSupportedImage(source, item.SourcePath);
                using Mat combined = HorizontalImageComposer.DuplicateToRight(source);

                // 先在内存中编码并回读验证通道，验证通过后才允许触碰正式输出路径。
                byte[] encoded = EncodeAndValidate(combined, item);
                string actualPath = overwriteExisting
                    ? WriteWithOverwrite(item.OutputPath, encoded)
                    : WriteWithoutOverwrite(item.OutputPath, encoded);
                outputPaths.Add(actualPath);
            }
            catch (Exception exception)
            {
                failures.Add(new BatchTwoInOneExportFailure(
                    item.SourcePath,
                    item.OutputPath,
                    exception.Message));
            }
            finally
            {
                progress?.Report(new BatchTwoInOneExportProgress(
                    index + 1,
                    plan.Items.Count,
                    Path.GetFileName(item.SourcePath)));
            }
        }

        return new BatchTwoInOneExportResult(outputPaths, failures);
    }

    /// <summary>把支持的原扩展名映射到编码器；扩展名比较不区分大小写。</summary>
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

    private static byte[] EncodeAndValidate(
        Mat image,
        BatchTwoInOneExportItem item)
    {
        string extension = Path.GetExtension(item.OutputPath);
        Cv2.ImEncode(extension, image, out byte[] encoded);
        if (encoded.Length == 0)
        {
            throw new IOException($"OpenCV 无法编码 {extension} 图像：{item.OutputPath}");
        }

        using Mat decoded = Cv2.ImDecode(encoded, ImreadModes.Unchanged);
        if (decoded.Empty())
        {
            throw new InvalidDataException($"编码后无法重新读取图片：{item.SourcePath}");
        }

        if (decoded.Rows != image.Rows || decoded.Cols != image.Cols || decoded.Type() != image.Type())
        {
            throw new InvalidDataException(
                $"{extension} 编码器不能无损保留该图片的 8 位通道类型；" +
                $"源类型 {image.Type()}，编码后类型 {decoded.Type()}：{item.SourcePath}");
        }

        return encoded;
    }

    private static void ValidateLosslessFormat(BatchTwoInOneExportItem item)
    {
        if (item.Format is ImageFormatKind.Jpeg or ImageFormatKind.WebP)
        {
            throw new InvalidDataException(
                "JPEG/WebP 是有损格式，不能保证二合一左右逐像素一致。" +
                $"请先转换为 PNG、BMP 或 TIFF：{item.SourcePath}");
        }
    }

    private static string WriteWithOverwrite(string outputPath, ReadOnlySpan<byte> bytes)
    {
        string target = Path.GetFullPath(outputPath);
        ImageFileWriter.WriteAllBytesAtomically(target, bytes);
        return target;
    }

    /// <summary>
    /// 未授权覆盖时先在目标目录完成唯一暂存文件，再以禁止覆盖的原子移动占用正式文件名。
    /// 即使计划完成后出现竞态同名文件，也绝不会替换它。
    /// </summary>
    private static string WriteWithoutOverwrite(string outputPath, ReadOnlySpan<byte> bytes)
    {
        string target = Path.GetFullPath(outputPath);
        string directory = Path.GetDirectoryName(target)
                           ?? throw new InvalidOperationException("输出图片路径没有有效目录。");
        string extension = Path.GetExtension(target);
        string stagingPath = Path.Combine(
            directory,
            $".{Path.GetFileNameWithoutExtension(target)}.{Guid.NewGuid():N}{extension}");

        try
        {
            ImageFileWriter.WriteAllBytesAtomically(stagingPath, bytes);
            File.Move(stagingPath, target, overwrite: false);
            return target;
        }
        finally
        {
            try
            {
                if (File.Exists(stagingPath))
                {
                    File.Delete(stagingPath);
                }
            }
            catch
            {
                // 暂存文件清理失败不能覆盖真实的导出错误。
            }
        }
    }

    private static string NormalizeDirectory(string directory)
    {
        return Path.TrimEndingDirectorySeparator(Path.GetFullPath(directory));
    }

    private static void EnsureDirectoriesAreIndependent(string sourceDirectory, string outputDirectory)
    {
        if (!Directory.Exists(sourceDirectory))
        {
            throw new DirectoryNotFoundException($"找不到源图片文件夹：{sourceDirectory}");
        }

        // 除规范化字符串比较外，再比较 Windows 文件系统实体，防止 junction、
        // 符号链接、映射盘或 UNC 别名绕过“不能覆盖源图”的限制。
        if (PathComparer.Equals(sourceDirectory, outputDirectory) ||
            Directory.Exists(outputDirectory) &&
            WindowsFileSystemIdentity.AreSameDirectory(sourceDirectory, outputDirectory))
        {
            throw new InvalidOperationException(
                "源图片文件夹与输出文件夹不能相同，否则会覆盖原图。请选择另一个输出文件夹。");
        }
    }

    private static void ValidateSupportedImage(Mat image, string sourcePath)
    {
        MatType type = image.Type();
        if (type != MatType.CV_8UC1 && type != MatType.CV_8UC3 && type != MatType.CV_8UC4)
        {
            throw new InvalidDataException(
                "批量二合一只处理 8 位灰度、BGR 或 BGRA 图片；" +
                $"高位深及其他像素格式不会被静默转换：{sourcePath}（实际类型 {type}）");
        }
    }
}

public sealed class BatchTwoInOneExportPlan
{
    internal BatchTwoInOneExportPlan(
        string sourceDirectory,
        string outputDirectory,
        IReadOnlyList<BatchTwoInOneExportItem> items,
        IReadOnlyList<string> existingOutputPaths)
    {
        SourceDirectory = sourceDirectory;
        OutputDirectory = outputDirectory;
        Items = Array.AsReadOnly(items.ToArray());
        ExistingOutputPaths = Array.AsReadOnly(existingOutputPaths.ToArray());
    }

    public string SourceDirectory { get; }

    public string OutputDirectory { get; }

    public IReadOnlyList<BatchTwoInOneExportItem> Items { get; }

    public IReadOnlyList<string> ExistingOutputPaths { get; }
}

public sealed record BatchTwoInOneExportItem(
    string SourcePath,
    string OutputPath,
    ImageFormatKind Format);

public sealed record BatchTwoInOneExportProgress(int CompletedCount, int TotalCount, string FileName);

public sealed record BatchTwoInOneExportFailure(
    string SourcePath,
    string OutputPath,
    string ErrorMessage);

public sealed class BatchTwoInOneExportResult
{
    internal BatchTwoInOneExportResult(
        IReadOnlyList<string> outputPaths,
        IReadOnlyList<BatchTwoInOneExportFailure> failures)
    {
        OutputPaths = Array.AsReadOnly(outputPaths.ToArray());
        Failures = Array.AsReadOnly(failures.ToArray());
    }

    public IReadOnlyList<string> OutputPaths { get; }

    public IReadOnlyList<BatchTwoInOneExportFailure> Failures { get; }
}
