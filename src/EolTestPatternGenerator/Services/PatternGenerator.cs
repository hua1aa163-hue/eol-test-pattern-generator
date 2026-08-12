using EolTestPatternGenerator.Models;
using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

public static class PatternGenerator
{
    private static readonly Scalar White = new(255, 255, 255);

    public static Mat Generate(PatternSettings settings)
    {
        Validate(settings);

        var canvas = new Mat(settings.CanvasHeight, settings.CanvasWidth, MatType.CV_8UC3, Scalar.Black);

        switch (settings.PatternType)
        {
            case PatternType.Border:
                DrawBorder(canvas, settings);
                break;
            case PatternType.PhaseStripes:
                DrawPhaseStripes(canvas, settings);
                break;
            case PatternType.NinePointGrid:
            case PatternType.DistortionGrid:
                DrawDotGrid(canvas, settings);
                break;
            case PatternType.CorrectionCross:
                DrawCorrectionCross(canvas, settings);
                break;
            case PatternType.WhiteRectangle:
                FillRectangleClipped(
                    canvas,
                    settings.PatternX,
                    settings.PatternY,
                    settings.PatternWidth,
                    settings.PatternHeight,
                    White);
                break;
            case PatternType.Black:
                break;
            default:
                canvas.Dispose();
                throw new ArgumentOutOfRangeException(nameof(settings.PatternType), settings.PatternType, null);
        }

        return canvas;
    }

    private static void Validate(PatternSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        if (settings.CanvasWidth is < 1 or > 16384 || settings.CanvasHeight is < 1 or > 16384)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "画布宽高必须在 1 到 16384 像素之间。");
        }

        if ((long)settings.CanvasWidth * settings.CanvasHeight > 40_000_000)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "为避免内存不足，画布总像素不能超过 4000 万。");
        }

        if (settings.PatternWidth < 1 || settings.PatternHeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "图案宽高必须大于 0。");
        }

        if (settings.Phase is < 1 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "相位必须在 1 到 8 之间。");
        }

        if (settings.DotRadius is < 0 or > 2048)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "圆点半径必须在 0 到 2048 像素之间。");
        }

        if (settings.Rows < 1 || settings.Columns < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "点阵行列数必须大于 0。");
        }

        if (settings.LineWidth < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "线宽必须大于 0。");
        }
    }

    private static void DrawBorder(Mat canvas, PatternSettings settings)
    {
        int thickness = Math.Min(settings.LineWidth, Math.Min(settings.PatternWidth, settings.PatternHeight));
        if ((2 * thickness) >= settings.PatternWidth || (2 * thickness) >= settings.PatternHeight)
        {
            FillRectangleClipped(
                canvas,
                settings.PatternX,
                settings.PatternY,
                settings.PatternWidth,
                settings.PatternHeight,
                White);
            return;
        }

        FillRectangleClipped(canvas, settings.PatternX, settings.PatternY, settings.PatternWidth, thickness, White);
        FillRectangleClipped(
            canvas,
            settings.PatternX,
            settings.PatternY + settings.PatternHeight - thickness,
            settings.PatternWidth,
            thickness,
            White);
        FillRectangleClipped(
            canvas,
            settings.PatternX,
            settings.PatternY + thickness,
            thickness,
            settings.PatternHeight - (2 * thickness),
            White);
        FillRectangleClipped(
            canvas,
            settings.PatternX + settings.PatternWidth - thickness,
            settings.PatternY + thickness,
            thickness,
            settings.PatternHeight - (2 * thickness),
            White);
    }

    private static unsafe void DrawPhaseStripes(Mat canvas, PatternSettings settings)
    {
        int left = Math.Max(0, settings.PatternX);
        int top = Math.Max(0, settings.PatternY);
        int right = Math.Min(canvas.Cols, settings.PatternX + settings.PatternWidth);
        int bottom = Math.Min(canvas.Rows, settings.PatternY + settings.PatternHeight);

        if (right <= left || bottom <= top)
        {
            return;
        }

        for (int y = top; y < bottom; y++)
        {
            byte* row = (byte*)canvas.Ptr(y);
            int v = y - settings.PatternY;

            for (int x = left; x < right; x++)
            {
                int u = x - settings.PatternX;
                int phaseIndex = PositiveModulo(v - (3 * u) + settings.Phase - 1, 8);
                int offset = x * 3;

                // OpenCV 为 BGR 顺序；定义来自样图的 RGB 二值相位公式。
                row[offset] = phaseIndex is >= 2 and <= 5 ? (byte)255 : (byte)0;
                row[offset + 1] = phaseIndex is >= 1 and <= 4 ? (byte)255 : (byte)0;
                row[offset + 2] = phaseIndex is >= 0 and <= 3 ? (byte)255 : (byte)0;
            }
        }
    }

    private static void DrawDotGrid(Mat canvas, PatternSettings settings)
    {
        for (int row = 0; row < settings.Rows; row++)
        {
            int centerY = settings.PatternY + InterpolateSpan(settings.PatternHeight, row, settings.Rows);

            for (int column = 0; column < settings.Columns; column++)
            {
                int centerX = settings.PatternX + InterpolateSpan(settings.PatternWidth, column, settings.Columns);
                DrawFilledDisk(canvas, centerX, centerY, settings.DotRadius);
            }
        }
    }

    private static void DrawCorrectionCross(Mat canvas, PatternSettings settings)
    {
        int centerX = settings.PatternX + (settings.PatternWidth / 2);
        int centerY = settings.PatternY + (settings.PatternHeight / 2);
        int lineWidth = Math.Min(settings.LineWidth, Math.Min(settings.PatternWidth, settings.PatternHeight));
        int halfLine = lineWidth / 2;

        // 使用无圆头的实心矩形，默认参数与“上下校正图.png”的阈值几何完全一致。
        FillRectangleClipped(
            canvas,
            settings.PatternX,
            centerY - halfLine,
            settings.PatternWidth,
            lineWidth,
            White);
        FillRectangleClipped(
            canvas,
            centerX - halfLine,
            settings.PatternY,
            lineWidth,
            settings.PatternHeight,
            White);
    }

    private static unsafe void DrawFilledDisk(Mat canvas, int centerX, int centerY, int radius)
    {
        long radiusSquared = (long)radius * radius;

        for (int dy = -radius; dy <= radius; dy++)
        {
            int y = centerY + dy;
            if ((uint)y >= (uint)canvas.Rows)
            {
                continue;
            }

            byte* row = (byte*)canvas.Ptr(y);
            long dySquared = (long)dy * dy;

            for (int dx = -radius; dx <= radius; dx++)
            {
                if (((long)dx * dx) + dySquared > radiusSquared)
                {
                    continue;
                }

                int x = centerX + dx;
                if ((uint)x >= (uint)canvas.Cols)
                {
                    continue;
                }

                int offset = x * 3;
                row[offset] = 255;
                row[offset + 1] = 255;
                row[offset + 2] = 255;
            }
        }
    }

    private static int InterpolateSpan(int span, int index, int count)
    {
        if (count <= 1)
        {
            return 0;
        }

        return (int)Math.Round(span * index / (double)(count - 1), MidpointRounding.AwayFromZero);
    }

    private static void FillRectangleClipped(Mat canvas, int x, int y, int width, int height, Scalar color)
    {
        int left = Math.Max(0, x);
        int top = Math.Max(0, y);
        int right = Math.Min(canvas.Cols, x + width);
        int bottom = Math.Min(canvas.Rows, y + height);

        if (right <= left || bottom <= top)
        {
            return;
        }

        Cv2.Rectangle(canvas, new Rect(left, top, right - left, bottom - top), color, -1, LineTypes.Link8);
    }

    private static int PositiveModulo(int value, int divisor)
    {
        int remainder = value % divisor;
        return remainder < 0 ? remainder + divisor : remainder;
    }
}
