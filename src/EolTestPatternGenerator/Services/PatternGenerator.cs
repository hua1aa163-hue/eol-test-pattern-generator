using EolTestPatternGenerator.Models;
using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

public static class PatternGenerator
{
    // OpenCV 的 Scalar 通道顺序是 B、G、R，而界面对用户展示的是 R、G、B。
    private static readonly Scalar White = new(255, 255, 255);
    private static readonly Scalar Red = new(0, 0, 255);
    private static readonly Scalar Green = new(0, 255, 0);
    private static readonly Scalar Blue = new(255, 0, 0);

    public static Mat Generate(PatternSettings settings)
    {
        // 返回的 Mat 由调用方负责 Dispose；预览和导出路径均使用 using 接管生命周期。
        Validate(settings);

        // 所有图卡统一转换为三通道8位BGR，避免编码时出现透明度或调色板差异。
        Mat canvas = settings.PatternType == PatternType.ImportedImage
            ? LoadImportedImage(settings)
            : new Mat(settings.CanvasHeight, settings.CanvasWidth, MatType.CV_8UC3, Scalar.Black);

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
            case PatternType.FullWhite:
                canvas.SetTo(White);
                break;
            case PatternType.FullRed:
                canvas.SetTo(Red);
                break;
            case PatternType.FullGreen:
                canvas.SetTo(Green);
                break;
            case PatternType.FullBlue:
                canvas.SetTo(Blue);
                break;
            case PatternType.ScreenSplit:
                DrawScreenSplit(canvas, settings);
                break;
            case PatternType.ImportedImage:
                // 底图已在创建画布时解码、转换和缩放；下方仍会应用通用白框叠加层。
                break;
            default:
                canvas.Dispose();
                throw new ArgumentOutOfRangeException(nameof(settings.PatternType), settings.PatternType, null);
        }

        // 白框始终最后绘制，因此可覆盖内置图卡和导入的外部底图。
        if (settings.BorderOverlay.Enabled)
        {
            DrawBorder(
                canvas,
                settings.BorderOverlay.X,
                settings.BorderOverlay.Y,
                settings.BorderOverlay.Width,
                settings.BorderOverlay.Height,
                settings.BorderOverlay.LineWidth);
        }

        return canvas;
    }

    private static Mat LoadImportedImage(PatternSettings settings)
    {
        string sourcePath = settings.SourceImagePath?.Trim() ?? string.Empty;
        if (sourcePath.Length == 0)
        {
            throw new InvalidOperationException("请选择要作为底图的图片文件。");
        }

        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException($"底图文件不存在：{sourcePath}", sourcePath);
        }

        byte[] encodedBytes;
        try
        {
            // File.ReadAllBytes 不依赖 OpenCV 的窄字符文件路径，可正确读取中文等 Unicode 路径。
            encodedBytes = File.ReadAllBytes(sourcePath);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new IOException($"无法读取底图文件：{sourcePath}", exception);
        }

        Mat decoded;
        try
        {
            decoded = Cv2.ImDecode(encodedBytes, ImreadModes.Unchanged);
        }
        catch (Exception exception)
        {
            throw new InvalidDataException($"底图解码失败，请确认图片文件有效：{sourcePath}", exception);
        }

        using (decoded)
        {
            if (decoded.Empty())
            {
                throw new InvalidDataException($"底图解码失败，请确认图片文件有效：{sourcePath}");
            }

            using var bgr = new Mat();
            try
            {
                switch (decoded.Channels())
                {
                    case 1:
                        Cv2.CvtColor(decoded, bgr, ColorConversionCodes.GRAY2BGR);
                        break;
                    case 3:
                        decoded.CopyTo(bgr);
                        break;
                    case 4:
                        Cv2.CvtColor(decoded, bgr, ColorConversionCodes.BGRA2BGR);
                        break;
                    default:
                        throw new InvalidDataException(
                            $"底图通道数不受支持（{decoded.Channels()} 通道），请选择灰度、BGR 或 BGRA 图片：{sourcePath}");
                }
            }
            catch (InvalidDataException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new InvalidDataException($"底图颜色格式转换失败：{sourcePath}", exception);
            }

            using var bgr8 = new Mat();
            if (bgr.Type() == MatType.CV_8UC3)
            {
                bgr.CopyTo(bgr8);
            }
            else
            {
                bgr.ConvertTo(bgr8, MatType.CV_8UC3);
            }

            if (bgr8.Cols == settings.CanvasWidth && bgr8.Rows == settings.CanvasHeight)
            {
                return bgr8.Clone();
            }

            var resized = new Mat();
            // 检测图使用最近邻缩放，防止双线性插值产生0/255之外的中间色。
            Cv2.Resize(
                bgr8,
                resized,
                new OpenCvSharp.Size(settings.CanvasWidth, settings.CanvasHeight),
                0,
                0,
                InterpolationFlags.Nearest);
            return resized;
        }
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

        BorderOverlaySettings border = settings.BorderOverlay;
        if (border.Enabled && (border.Width < 1 || border.Height < 1 || border.LineWidth < 1))
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "白框叠加层的宽、高和线宽必须大于 0。");
        }
    }

    private static void DrawBorder(Mat canvas, PatternSettings settings)
    {
        DrawBorder(
            canvas,
            settings.PatternX,
            settings.PatternY,
            settings.PatternWidth,
            settings.PatternHeight,
            settings.LineWidth);
    }

    private static void DrawBorder(Mat canvas, int x, int y, int width, int height, int lineWidth)
    {
        // width/height 表示外包矩形尺寸，右边和下边是排他边界；越界由填充函数裁剪。
        int thickness = Math.Min(lineWidth, Math.Min(width, height));
        if ((2 * thickness) >= width || (2 * thickness) >= height)
        {
            // 线宽覆盖内部空间时退化为实心矩形，避免四条边重叠产生不一致。
            FillRectangleClipped(canvas, x, y, width, height, White);
            return;
        }

        FillRectangleClipped(canvas, x, y, width, thickness, White);
        FillRectangleClipped(canvas, x, y + height - thickness, width, thickness, White);
        FillRectangleClipped(canvas, x, y + thickness, thickness, height - (2 * thickness), White);
        FillRectangleClipped(canvas, x + width - thickness, y + thickness, thickness, height - (2 * thickness), White);
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
                // 使用局部坐标计算8步周期；正模保证裁剪到负坐标后周期仍连续。
                int phaseIndex = PositiveModulo(v - (3 * u) + settings.Phase - 1, 8);
                int offset = x * 3;

                byte logicalR = phaseIndex is >= 0 and <= 3 ? (byte)255 : (byte)0;
                byte logicalG = phaseIndex is >= 1 and <= 4 ? (byte)255 : (byte)0;
                byte logicalB = phaseIndex is >= 2 and <= 5 ? (byte)255 : (byte)0;
                (byte red, byte green, byte blue) = ApplyPixelOrder(
                    logicalR,
                    logicalG,
                    logicalB,
                    settings.PixelOrder);

                // OpenCV 的内存通道顺序为 BGR。
                row[offset] = blue;
                row[offset + 1] = green;
                row[offset + 2] = red;
            }
        }
    }

    private static (byte Red, byte Green, byte Blue) ApplyPixelOrder(
        byte red,
        byte green,
        byte blue,
        RgbPixelOrder order)
    {
        // 排列仅置换三个颜色通道，不改变像素横向坐标或8步周期。
        return order switch
        {
            RgbPixelOrder.RGB => (red, green, blue),
            RgbPixelOrder.RBG => (red, blue, green),
            RgbPixelOrder.GRB => (green, red, blue),
            RgbPixelOrder.GBR => (green, blue, red),
            RgbPixelOrder.BRG => (blue, red, green),
            RgbPixelOrder.BGR => (blue, green, red),
            _ => throw new ArgumentOutOfRangeException(nameof(order), order, null)
        };
    }

    private static void DrawScreenSplit(Mat canvas, PatternSettings settings)
    {
        canvas.SetTo(White);

        switch (settings.ScreenSplitMode)
        {
            case ScreenSplitMode.TwoDimensionalBlackLeft:
                FillRectangleClipped(canvas, 50, 50, 1500, 1900, Scalar.Black);
                break;
            case ScreenSplitMode.TwoDimensionalBlackRight:
                FillRectangleClipped(canvas, 1650, 50, 1500, 1900, Scalar.Black);
                break;
            case ScreenSplitMode.TwoDimensionalBlackBoth:
                FillRectangleClipped(canvas, 50, 50, 1500, 1900, Scalar.Black);
                FillRectangleClipped(canvas, 1650, 50, 1500, 1900, Scalar.Black);
                break;
            case ScreenSplitMode.ThreeDimensionalBlackRight:
                FillRectangleClipped(canvas, 3250, 50, 3100, 1900, Scalar.Black);
                break;
            case ScreenSplitMode.ThreeDimensionalBlackLeft:
                FillRectangleClipped(canvas, 50, 50, 3100, 1900, Scalar.Black);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(settings.ScreenSplitMode), settings.ScreenSplitMode, null);
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

        // 以首末圆心跨度插值，确保最后一个圆心精确落在起点加跨度的位置。
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
