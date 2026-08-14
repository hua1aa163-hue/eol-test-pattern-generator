using System.Globalization;
using System.Text;
using EolTestPatternGenerator.Models;
using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

/// <summary>
/// 将离散光源图写为 LightTools 可读取的 MESH 光源文本。
/// </summary>
public static class LightToolsMeshWriter
{
    private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
    private static readonly UTF8Encoding Utf8WithoutBom = new(encoderShouldEmitUTF8Identifier: false);
    private static readonly string[] FormattedByteValues = Enumerable
        .Range(byte.MinValue, byte.MaxValue + 1)
        .Select(value => value.ToString("F4", InvariantCulture))
        .ToArray();

    /// <summary>
    /// 生成单光源文本。每个像素按逻辑 R、G、B 展开为三个网格采样值。
    /// </summary>
    public static string CreateSingleSourceText(DiscreteCrosstalkSettings settings, Mat image)
    {
        ValidateImage(settings, image);
        return CreateMeshText(settings, image, logicalChannelFilter: null);
    }

    /// <summary>
    /// 分别生成 R、G、B 光源文本。非当前颜色的子像素写为字面量 0，
    /// 当前颜色的采样值固定保留四位小数，与原程序格式一致。
    /// </summary>
    public static (string Red, string Green, string Blue) CreateRgbSourceTexts(
        DiscreteCrosstalkSettings settings,
        Mat image)
    {
        ValidateImage(settings, image);
        return (
            CreateMeshText(settings, image, logicalChannelFilter: 0),
            CreateMeshText(settings, image, logicalChannelFilter: 1),
            CreateMeshText(settings, image, logicalChannelFilter: 2));
    }

    /// <summary>按原程序命名写入一个“{文件主体}.txt”光源文件。</summary>
    public static string WriteSingleSourceFile(
        string outputDirectory,
        DiscreteCrosstalkSettings settings,
        int group,
        Mat image)
    {
        string directory = PrepareOutput(settings, group, image, outputDirectory);
        string path = Path.Combine(
            directory,
            DiscreteCrosstalkGenerator.GetFileBaseName(settings, group) + ".txt");
        WriteMeshFile(path, settings, image, logicalChannelFilter: null);
        return path;
    }

    /// <summary>按原程序命名写入“_R.txt”、“_G.txt”、“_B.txt”三个光源文件。</summary>
    public static IReadOnlyList<string> WriteRgbSourceFiles(
        string outputDirectory,
        DiscreteCrosstalkSettings settings,
        int group,
        Mat image)
    {
        string directory = PrepareOutput(settings, group, image, outputDirectory);
        string baseName = DiscreteCrosstalkGenerator.GetFileBaseName(settings, group);
        string redPath = Path.Combine(directory, baseName + "_R.txt");
        string greenPath = Path.Combine(directory, baseName + "_G.txt");
        string bluePath = Path.Combine(directory, baseName + "_B.txt");

        // 三个大文件依次流式写出，避免同时在托管堆中保留三份完整文本。
        WriteMeshFile(redPath, settings, image, logicalChannelFilter: 0);
        WriteMeshFile(greenPath, settings, image, logicalChannelFilter: 1);
        WriteMeshFile(bluePath, settings, image, logicalChannelFilter: 2);
        return [redPath, greenPath, bluePath];
    }

    /// <summary>依据 SingleLightSourceFile 设置写入一个或三个 TXT 文件。</summary>
    public static IReadOnlyList<string> WriteForGroup(
        string outputDirectory,
        DiscreteCrosstalkSettings settings,
        int group,
        Mat image)
    {
        return settings.SingleLightSourceFile
            ? [WriteSingleSourceFile(outputDirectory, settings, group, image)]
            : WriteRgbSourceFiles(outputDirectory, settings, group, image);
    }

    /// <summary>
    /// 生成设置指定的一个或全部相位组，并直接输出对应 LightTools 文本。
    /// 生成的 Mat 在每组写完后立即释放。
    /// </summary>
    public static IReadOnlyList<string> ExportBatch(
        string outputDirectory,
        DiscreteCrosstalkSettings settings,
        Mat? customSourceA = null,
        Mat? customSourceB = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);
        var paths = new List<string>();
        DiscreteCrosstalkGenerator.GenerateBatch(
            settings,
            (group, image) => paths.AddRange(WriteForGroup(outputDirectory, settings, group, image)),
            customSourceA,
            customSourceB);
        return paths;
    }

    /// <summary>
    /// 将任意 8 位三通道 BGR 图片写成一个子像素光源文件，或拆成 R/G/B 三个文件。
    /// 该入口供“图片转文件”和连续融合结果复用，不套用离散图卡的旧文件命名规则。
    /// </summary>
    public static IReadOnlyList<string> WriteImageSourceFiles(
        string outputDirectory,
        string fileBaseName,
        Mat image,
        double pixelSizeXMicrometers,
        double pixelSizeYMicrometers,
        bool singleSourceFile)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileBaseName);
        ArgumentNullException.ThrowIfNull(image);

        string safeBaseName = Path.GetFileName(fileBaseName.Trim());
        if (!string.Equals(safeBaseName, fileBaseName.Trim(), StringComparison.Ordinal) ||
            safeBaseName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            throw new ArgumentException("文件名主体不能包含目录分隔符或无效字符。", nameof(fileBaseName));
        }

        var settings = new DiscreteCrosstalkSettings
        {
            CanvasWidth = image.Cols,
            CanvasHeight = image.Rows,
            SubpixelPeriod = 1,
            LitSubpixelCount = 1,
            Group = 0,
            PixelSizeXMicrometers = pixelSizeXMicrometers,
            PixelSizeYMicrometers = pixelSizeYMicrometers,
            ScreenSizeInches = 1d,
            PartitionWidthSubpixels = Math.Max(1, image.Cols * 3),
            SingleLightSourceFile = singleSourceFile
        };

        ValidateImage(settings, image);
        string directory = Path.GetFullPath(outputDirectory);
        Directory.CreateDirectory(directory);

        if (singleSourceFile)
        {
            string path = Path.Combine(directory, safeBaseName + ".txt");
            WriteMeshFile(path, settings, image, logicalChannelFilter: null);
            return [path];
        }

        string redPath = Path.Combine(directory, safeBaseName + "_R.txt");
        string greenPath = Path.Combine(directory, safeBaseName + "_G.txt");
        string bluePath = Path.Combine(directory, safeBaseName + "_B.txt");
        WriteMeshFile(redPath, settings, image, logicalChannelFilter: 0);
        WriteMeshFile(greenPath, settings, image, logicalChannelFilter: 1);
        WriteMeshFile(bluePath, settings, image, logicalChannelFilter: 2);
        return [redPath, greenPath, bluePath];
    }

    private static unsafe string CreateMeshText(
        DiscreteCrosstalkSettings settings,
        Mat image,
        int? logicalChannelFilter)
    {
        long estimatedCharacters = (long)settings.CanvasWidth * settings.CanvasHeight * 12L;
        int initialCapacity = (int)Math.Clamp(estimatedCharacters, 128L, 4_000_000L);
        var builder = new StringBuilder(initialCapacity);
        using var writer = new StringWriter(builder, InvariantCulture);
        WriteMesh(writer, settings, image, logicalChannelFilter);
        return builder.ToString();
    }

    private static void WriteMeshFile(
        string path,
        DiscreteCrosstalkSettings settings,
        Mat image,
        int? logicalChannelFilter)
    {
        string targetPath = Path.GetFullPath(path);
        string directory = Path.GetDirectoryName(targetPath)
                           ?? throw new InvalidOperationException("LightTools 输出路径没有有效目录。");
        Directory.CreateDirectory(directory);

        // 先在目标目录写完整临时文件，再原子替换，避免磁盘写入失败时截断原有 MESH。
        string temporaryPath = Path.Combine(
            directory,
            $".{Path.GetFileName(targetPath)}.{Environment.ProcessId}.{Guid.NewGuid():N}.tmp");
        try
        {
            using (var stream = new FileStream(
                       temporaryPath,
                       FileMode.CreateNew,
                       FileAccess.Write,
                       FileShare.None,
                       bufferSize: 64 * 1024,
                       FileOptions.WriteThrough))
            {
                using (var writer = new StreamWriter(
                           stream,
                           Utf8WithoutBom,
                           bufferSize: 64 * 1024,
                           leaveOpen: true))
                {
                    WriteMesh(writer, settings, image, logicalChannelFilter);
                    writer.Flush();
                }

                stream.Flush(flushToDisk: true);
            }

            ReplaceAtomically(temporaryPath, targetPath);
        }
        finally
        {
            try
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
            catch
            {
                // 清理失败不能覆盖原始导出异常。
            }
        }
    }

    private static void ReplaceAtomically(string temporaryPath, string targetPath)
    {
        if (!File.Exists(targetPath))
        {
            File.Move(temporaryPath, targetPath);
            return;
        }

        try
        {
            File.Replace(temporaryPath, targetPath, destinationBackupFileName: null, ignoreMetadataErrors: true);
        }
        catch (Exception exception) when (exception is IOException or PlatformNotSupportedException)
        {
            // 某些文件系统不支持 Replace；同目录覆盖移动仍不会暴露半写入内容。
            File.Move(temporaryPath, targetPath, overwrite: true);
        }
    }

    private static unsafe void WriteMesh(
        TextWriter writer,
        DiscreteCrosstalkSettings settings,
        Mat image,
        int? logicalChannelFilter)
    {
        // Rows/Cols 是 OpenCvSharp 的原生调用；先缓存，避免在大图循环中反复跨越 P/Invoke。
        int rows = image.Rows;
        int columns = image.Cols;
        double halfWidth = settings.CanvasWidth * settings.PixelSizeXMicrometers / 2000d;
        double halfHeight = settings.CanvasHeight * settings.PixelSizeYMicrometers / 2000d;

        writer.Write("MESH: ");
        writer.Write(settings.CanvasWidth * 3);
        writer.Write(' ');
        writer.Write(settings.CanvasHeight);
        writer.Write("  ");
        writer.Write((-halfWidth).ToString("G6", InvariantCulture));
        writer.Write(' ');
        writer.Write((-halfHeight).ToString("G6", InvariantCulture));
        writer.Write(' ');
        writer.Write(halfWidth.ToString("G6", InvariantCulture));
        writer.Write(' ');
        writer.Write(halfHeight.ToString("G6", InvariantCulture));
        writer.Write("\r\n");

        for (int y = 0; y < rows; y++)
        {
            byte* row = (byte*)image.Ptr(y);
            for (int x = 0; x < columns; x++)
            {
                int pixelOffset = x * 3;
                for (int logicalChannel = 0; logicalChannel < 3; logicalChannel++)
                {
                    if (logicalChannelFilter is not null && logicalChannel != logicalChannelFilter.Value)
                    {
                        // 原程序在分通道文件中对非目标子像素写整数 0，而不是 0.0000。
                        writer.Write('0');
                    }
                    else
                    {
                        int bgrChannel = 2 - logicalChannel;
                        // 像素只有 256 种取值，查表可避免大图导出时创建数千万个短字符串。
                        writer.Write(FormattedByteValues[row[pixelOffset + bgrChannel]]);
                    }

                    writer.Write('\t');
                }
            }

            writer.Write("\r\n");
        }

        // 原程序在最后一行之后再写一个空行。
        writer.Write("\r\n");
    }

    private static string PrepareOutput(
        DiscreteCrosstalkSettings settings,
        int group,
        Mat image,
        string outputDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);
        ValidateImage(settings, image);
        DiscreteCrosstalkGenerator.ValidateGroup(group, settings.SubpixelPeriod);
        string directory = Path.GetFullPath(outputDirectory);
        Directory.CreateDirectory(directory);
        return directory;
    }

    private static void ValidateImage(DiscreteCrosstalkSettings settings, Mat image)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(image);
        DiscreteCrosstalkGenerator.ValidateSettings(settings);

        if (image.Empty())
        {
            throw new InvalidDataException("不能导出空图片。");
        }

        if (image.Type() != MatType.CV_8UC3)
        {
            throw new InvalidDataException("LightTools 光源图片必须是 8 位三通道 BGR 图片。");
        }

        if (image.Cols != settings.CanvasWidth || image.Rows != settings.CanvasHeight)
        {
            throw new InvalidDataException("LightTools 光源图片尺寸必须与离散光源设置的画布尺寸相同。");
        }
    }
}
