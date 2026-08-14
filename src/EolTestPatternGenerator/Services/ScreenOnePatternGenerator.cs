using EolTestPatternGenerator.Models;
using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

/// <summary>
/// 根据可编辑参数生成“显示器”的三种二值图卡。
/// </summary>
public static class ScreenOnePatternGenerator
{
    private static readonly Scalar White = new(255, 255, 255);

    /// <summary>
    /// 生成指定图卡。所有绘制均使用实心矩形且不进行抗锯齿，
    /// 因此输出像素的 RGB 通道严格只有 0 和 255。
    /// </summary>
    public static Mat Generate(ScreenOneSettings settings, ScreenOneCardKind cardKind)
    {
        Validate(settings);
        PixelRegion leftRegion = settings.GetLeftRegion();
        PixelRegion rightRegion = settings.GetRightRegion();

        // 三张参考图的区域外背景均为纯白色。
        var canvas = new Mat(
            settings.CanvasHeight,
            settings.CanvasWidth,
            MatType.CV_8UC3,
            White);

        try
        {
            switch (cardKind)
            {
                case ScreenOneCardKind.BlackLeftWhiteRight:
                    // 后绘制右侧白区域；区域重叠时，白色区域仍能实际覆盖黑色区域。
                    FillRectangleClipped(
                        canvas,
                        leftRegion.X,
                        leftRegion.Y,
                        leftRegion.Width,
                        leftRegion.Height,
                        Scalar.Black);
                    FillRectangleClipped(
                        canvas,
                        rightRegion.X,
                        rightRegion.Y,
                        rightRegion.Width,
                        rightRegion.Height,
                        White);
                    break;

                case ScreenOneCardKind.WhiteLeftBlackRight:
                    FillRectangleClipped(
                        canvas,
                        leftRegion.X,
                        leftRegion.Y,
                        leftRegion.Width,
                        leftRegion.Height,
                        White);
                    FillRectangleClipped(
                        canvas,
                        rightRegion.X,
                        rightRegion.Y,
                        rightRegion.Width,
                        rightRegion.Height,
                        Scalar.Black);
                    break;

                case ScreenOneCardKind.BlackBoth:
                    FillRectangleClipped(
                        canvas,
                        leftRegion.X,
                        leftRegion.Y,
                        leftRegion.Width,
                        leftRegion.Height,
                        Scalar.Black);
                    FillRectangleClipped(
                        canvas,
                        rightRegion.X,
                        rightRegion.Y,
                        rightRegion.Width,
                        rightRegion.Height,
                        Scalar.Black);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(cardKind), cardKind, "未知的显示器图卡类型。");
            }

            return canvas;
        }
        catch
        {
            canvas.Dispose();
            throw;
        }
    }

    /// <summary>
    /// 返回三种图卡固定使用的文件名（不含扩展名）。
    /// </summary>
    public static string GetFileBaseName(ScreenOneCardKind cardKind)
    {
        return cardKind switch
        {
            ScreenOneCardKind.BlackLeftWhiteRight => "1.B_W",
            ScreenOneCardKind.WhiteLeftBlackRight => "2.W_B",
            ScreenOneCardKind.BlackBoth => "3.B",
            _ => throw new ArgumentOutOfRangeException(nameof(cardKind), cardKind, null)
        };
    }

    private static void Validate(ScreenOneSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        if (settings.CanvasWidth is < 1 or > 16384 || settings.CanvasHeight is < 1 or > 16384)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "画布宽高必须在 1 到 16384 像素之间。");
        }

        // 与主生成器保持相同的内存保护限制。
        if ((long)settings.CanvasWidth * settings.CanvasHeight > 40_000_000)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "为避免内存不足，画布总像素不能超过 4000 万。");
        }

        // Resolve 同时检查派生尺寸为正数、long 计算和 int 绘图坐标范围。
        _ = settings.GetLeftRegion();
        _ = settings.GetRightRegion();
    }

    /// <summary>
    /// 将矩形裁剪到画布范围内，支持用户输入负坐标或超出画布的区域。
    /// </summary>
    private static void FillRectangleClipped(
        Mat canvas,
        int x,
        int y,
        int width,
        int height,
        Scalar color)
    {
        long requestedRight = (long)x + width;
        long requestedBottom = (long)y + height;
        int left = Math.Max(0, x);
        int top = Math.Max(0, y);
        int right = (int)Math.Min(canvas.Cols, requestedRight);
        int bottom = (int)Math.Min(canvas.Rows, requestedBottom);

        if (right <= left || bottom <= top)
        {
            return;
        }

        Cv2.Rectangle(
            canvas,
            new Rect(left, top, right - left, bottom - top),
            color,
            thickness: -1,
            lineType: LineTypes.Link8);
    }
}
