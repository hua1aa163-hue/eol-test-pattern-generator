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
        Cv2.ImEncode(extension, image, out byte[] encoded, parameters);
        if (encoded.Length == 0)
        {
            throw new IOException($"OpenCV 无法编码 {extension} 图像：{fullPath}");
        }

        File.WriteAllBytes(fullPath, encoded);
        return fullPath;
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
