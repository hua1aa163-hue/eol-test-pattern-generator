using EolTestPatternGenerator.Models;
using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

public static class ImageFileWriter
{
    public static string GetExtension(ImageFormatKind format)
    {
        return format switch
        {
            ImageFormatKind.Png => ".png",
            ImageFormatKind.Jpeg => ".jpg",
            ImageFormatKind.Bmp => ".bmp",
            ImageFormatKind.Tiff => ".tiff",
            ImageFormatKind.WebP => ".webp",
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    public static string GetDialogFilter(ImageFormatKind format)
    {
        return format switch
        {
            ImageFormatKind.Png => "PNG 图像 (*.png)|*.png|所有文件 (*.*)|*.*",
            ImageFormatKind.Jpeg => "JPEG 图像 (*.jpg;*.jpeg)|*.jpg;*.jpeg|所有文件 (*.*)|*.*",
            ImageFormatKind.Bmp => "BMP 位图 (*.bmp)|*.bmp|所有文件 (*.*)|*.*",
            ImageFormatKind.Tiff => "TIFF 图像 (*.tif;*.tiff)|*.tif;*.tiff|所有文件 (*.*)|*.*",
            ImageFormatKind.WebP => "WebP 图像 (*.webp)|*.webp|所有文件 (*.*)|*.*",
            _ => "所有文件 (*.*)|*.*"
        };
    }

    public static string NormalizePath(string path, ImageFormatKind format)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string fullPath = Path.GetFullPath(path);
        string extension = Path.GetExtension(fullPath).ToLowerInvariant();
        bool extensionMatches = format switch
        {
            ImageFormatKind.Png => extension == ".png",
            ImageFormatKind.Jpeg => extension is ".jpg" or ".jpeg",
            ImageFormatKind.Bmp => extension == ".bmp",
            ImageFormatKind.Tiff => extension is ".tif" or ".tiff",
            ImageFormatKind.WebP => extension == ".webp",
            _ => false
        };

        return extensionMatches ? fullPath : Path.ChangeExtension(fullPath, GetExtension(format));
    }

    public static string Write(string path, Mat image, ImageExportOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(image);
        ArgumentNullException.ThrowIfNull(options);

        string fullPath = NormalizePath(path, options.Format);
        string extension = Path.GetExtension(fullPath);
        string? directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        ImageEncodingParam[] parameters = CreateEncodingParameters(options);
        // 先编码成内存字节再由 .NET 写盘，可稳定支持中文等 Unicode 输出路径。
        Cv2.ImEncode(extension, image, out byte[] encoded, parameters);
        if (encoded.Length == 0)
        {
            throw new IOException($"OpenCV 无法编码 {extension} 图像：{fullPath}");
        }

        WriteAllBytesAtomically(fullPath, encoded);
        return fullPath;
    }

    /// <summary>
    /// 在目标目录完成临时文件后再替换正式文件，避免编码成功但写盘中断时截断旧图片。
    /// </summary>
    internal static void WriteAllBytesAtomically(string path, ReadOnlySpan<byte> bytes)
    {
        string targetPath = Path.GetFullPath(path);
        string directory = Path.GetDirectoryName(targetPath)
                           ?? throw new InvalidOperationException("图片输出路径没有有效目录。");
        Directory.CreateDirectory(directory);

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
                stream.Write(bytes);
                stream.Flush(flushToDisk: true);
            }

            if (!File.Exists(targetPath))
            {
                File.Move(temporaryPath, targetPath);
            }
            else
            {
                try
                {
                    File.Replace(temporaryPath, targetPath, destinationBackupFileName: null, ignoreMetadataErrors: true);
                }
                catch (Exception exception) when (exception is IOException or PlatformNotSupportedException)
                {
                    File.Move(temporaryPath, targetPath, overwrite: true);
                }
            }
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
                // 清理失败不能覆盖原始写盘异常。
            }
        }
    }

    private static ImageEncodingParam[] CreateEncodingParameters(ImageExportOptions options)
    {
        int quality = Math.Clamp(options.Quality, 1, 100);

        return options.Format switch
        {
            ImageFormatKind.Jpeg => [new ImageEncodingParam(ImwriteFlags.JpegQuality, quality)],
            ImageFormatKind.WebP => [new ImageEncodingParam(ImwriteFlags.WebPQuality, quality)],
            _ => []
        };
    }
}
