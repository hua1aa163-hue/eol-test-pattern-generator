using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

/// <summary>
/// 通过 .NET 读取文件字节后交给 OpenCV 解码，避免原生 imread 对中文路径的兼容差异。
/// </summary>
public static class UnicodeImageLoader
{
    public static Mat LoadColor(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        string fullPath = Path.GetFullPath(path);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("找不到图像文件。", fullPath);
        }

        byte[] encoded = File.ReadAllBytes(fullPath);
        Mat image = Cv2.ImDecode(encoded, ImreadModes.Color);
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
