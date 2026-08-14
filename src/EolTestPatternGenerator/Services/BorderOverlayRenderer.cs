using EolTestPatternGenerator.Models;
using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

/// <summary>在生成结果最后叠加可裁剪的白色矩形框，不改变画布尺寸。</summary>
public static class BorderOverlayRenderer
{
    private static readonly Scalar White = new(255, 255, 255);

    public static void Apply(Mat image, BorderOverlaySettings settings)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentNullException.ThrowIfNull(settings);
        if (!settings.Enabled)
        {
            return;
        }

        if (settings.LineWidth < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "白框线宽必须大于 0。");
        }

        PixelRegion region = settings.GetRegion(image.Cols, image.Rows);
        int thickness = Math.Min(settings.LineWidth, Math.Min(region.Width, region.Height));
        if ((2L * thickness) >= region.Width || (2L * thickness) >= region.Height)
        {
            FillClipped(image, region.X, region.Y, region.Width, region.Height);
            return;
        }

        FillClipped(image, region.X, region.Y, region.Width, thickness);
        FillClipped(image, region.X, CheckedAdd(region.Height, region.Y, -thickness), region.Width, thickness);
        FillClipped(image, region.X, CheckedAdd(region.Y, thickness), thickness, region.Height - (2 * thickness));
        FillClipped(
            image,
            CheckedAdd(region.Width, region.X, -thickness),
            CheckedAdd(region.Y, thickness),
            thickness,
            region.Height - (2 * thickness));
    }

    private static void FillClipped(Mat image, int x, int y, int width, int height)
    {
        long rightValue = (long)x + width;
        long bottomValue = (long)y + height;
        int left = Math.Max(0, x);
        int top = Math.Max(0, y);
        int right = (int)Math.Min(image.Cols, rightValue);
        int bottom = (int)Math.Min(image.Rows, bottomValue);
        if (right <= left || bottom <= top)
        {
            return;
        }

        Cv2.Rectangle(image, new Rect(left, top, right - left, bottom - top), White, -1, LineTypes.Link8);
    }

    private static int CheckedAdd(int first, int second, int third = 0)
    {
        long result = (long)first + second + third;
        if (result is < int.MinValue or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(first), "白框绘制坐标超出支持范围。");
        }

        return (int)result;
    }
}
