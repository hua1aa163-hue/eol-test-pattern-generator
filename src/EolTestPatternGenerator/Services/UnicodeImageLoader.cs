using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

/// <summary>
/// 通过 .NET 读取文件字节后交给 OpenCV 解码，避免原生 imread 对中文路径的兼容差异。
/// </summary>
public static class UnicodeImageLoader
{
    public static Mat LoadColor(string path)
    {
        return Load(path, ImreadModes.Color);
    }

    /// <summary>
    /// 保留 8 位灰度、BGR 或 BGRA 通道读取图片。批量白框使用该入口，
    /// 避免透明 PNG/WebP 在尚未绘框时就被强制转成不透明三通道。
    /// </summary>
    public static Mat LoadUnchanged(string path)
    {
        return Load(path, ImreadModes.Unchanged);
    }

    private static Mat Load(string path, ImreadModes mode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        string fullPath = Path.GetFullPath(path);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("找不到图像文件。", fullPath);
        }

        byte[] encoded = File.ReadAllBytes(fullPath);
        Mat image = Cv2.ImDecode(encoded, mode);
        if (image.Empty())
        {
            image.Dispose();
            throw new InvalidDataException($"OpenCV 无法解码图像：{fullPath}");
        }

        return image;
    }

    /// <summary>交换红、蓝通道，复现旧 MATLAB 文件夹分支的 [3 2 1] 行为。</summary>
    public static void SwapRedBlueInPlace(Mat image)
    {
        ArgumentNullException.ThrowIfNull(image);
        if (image.Empty() || image.Type() != MatType.CV_8UC3)
        {
            throw new InvalidDataException("只有 8 位三通道图像可以交换红蓝通道。");
        }

        Cv2.CvtColor(image, image, ColorConversionCodes.BGR2RGB);
    }
}
