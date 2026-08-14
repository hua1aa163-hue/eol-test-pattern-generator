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

        if (settings.Width < 1 || settings.Height < 1 || settings.LineWidth < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "白框宽、高和线宽必须大于 0。");
        }

        int thickness = Math.Min(settings.LineWidth, Math.Min(settings.Width, settings.Height));
        if ((2 * thickness) >= settings.Width || (2 * thickness) >= settings.Height)
        {
            FillClipped(image, settings.X, settings.Y, settings.Width, settings.Height);
            return;
        }

        FillClipped(image, settings.X, settings.Y, settings.Width, thickness);
        FillClipped(image, settings.X, settings.Y + settings.Height - thickness, settings.Width, thickness);
        FillClipped(image, settings.X, settings.Y + thickness, thickness, settings.Height - (2 * thickness));
        FillClipped(
            image,
            settings.X + settings.Width - thickness,
            settings.Y + thickness,
            thickness,
            settings.Height - (2 * thickness));
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
}
