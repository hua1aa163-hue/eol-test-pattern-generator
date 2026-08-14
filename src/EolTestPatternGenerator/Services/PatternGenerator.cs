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
            BorderOverlayRenderer.Apply(canvas, settings.BorderOverlay);
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

        bool isFullCanvas = settings.PatternType is PatternType.Black or PatternType.FullWhite or
            PatternType.FullRed or PatternType.FullGreen or PatternType.FullBlue or PatternType.ImportedImage;
        if (isFullCanvas)
        {
            if (settings.LeftMargin != 0 || settings.TopMargin != 0 ||
                settings.RightMargin != 0 || settings.BottomMargin != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(settings), "全屏图卡的四边距必须固定为 0。");
            }
        }
        else
        {
            // Resolve 以 long 计算派生宽高，并统一检查负尺寸和 int 溢出。
            _ = settings.GetOuterRegion();
            if (settings.IsDotGrid &&
                (settings.CalculatedCenterSpanWidth < 0 || settings.CalculatedCenterSpanHeight < 0))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(settings),
                    "点阵外缘区域必须至少容纳一个完整圆点直径。");
            }

            if (settings.IsDotGrid &&
                ((settings.Columns == 1 && settings.CalculatedCenterSpanWidth != 0) ||
                 (settings.Rows == 1 && settings.CalculatedCenterSpanHeight != 0)))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(settings),
                    "单列点阵的水平圆心跨度、单行点阵的垂直圆心跨度必须为 0。");
            }
        }

        if (settings.PatternType == PatternType.PhaseStripes)
        {
            CrosstalkPixelCycle cycle = CrosstalkPixelCyclePresets.Resolve(settings);
            ValidatePixelCycle(cycle);
            if (settings.Phase < 1 || settings.Phase > cycle.PeriodLength)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(settings),
                    $"相位必须在 1 到当前周期像素数 {cycle.PeriodLength} 之间。");
            }
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
        if (border.Enabled)
        {
            if (border.LineWidth < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(settings), "白框叠加层线宽必须大于 0。");
            }

            _ = border.GetRegion(settings.CanvasWidth, settings.CanvasHeight);
        }
    }

    private static void ValidatePixelCycle(CrosstalkPixelCycle cycle)
    {
        if (cycle.Pixels is null ||
            cycle.Pixels.Count is < CrosstalkPixelCycle.MinimumPeriodLength or
                > CrosstalkPixelCycle.MaximumPeriodLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(cycle),
                $"串扰周期像素数必须在 {CrosstalkPixelCycle.MinimumPeriodLength} 到 " +
                $"{CrosstalkPixelCycle.MaximumPeriodLength} 之间。");
        }

        for (int index = 0; index < cycle.Pixels.Count; index++)
        {
            RgbChannelMask mask = cycle.Pixels[index];
            if ((mask & ~RgbChannelMask.All) != 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(cycle),
                    $"串扰周期第 {index + 1} 个像素包含未知通道值 {(int)mask}。");
            }
        }

        // 同时验证角度范围以及 3 × tan(theta) 是否能得到有限的连续行位移。
        _ = cycle.CalculateEffectiveRowAdvance();
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
        if ((2L * thickness) >= width || (2L * thickness) >= height)
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
        CrosstalkPixelCycle cycle = CrosstalkPixelCyclePresets.Resolve(settings);
        double rowAdvance = cycle.CalculateEffectiveRowAdvance();
        int left = Math.Max(0, settings.PatternX);
        int top = Math.Max(0, settings.PatternY);
        int right = (int)Math.Min(canvas.Cols, (long)settings.PatternX + settings.PatternWidth);
        int bottom = (int)Math.Min(canvas.Rows, (long)settings.PatternY + settings.PatternHeight);

        if (right <= left || bottom <= top)
        {
            return;
        }

        for (int y = top; y < bottom; y++)
        {
            byte* row = (byte*)canvas.Ptr(y);
            int v = y - settings.PatternY;
            // 与非整数连续融合采用相同语义：每行移动 3 × tan(theta) 个周期位置。
            // 一个输出像素仍必须严格为 0/255，因此连续坐标落入哪个周期单元由 floor 决定。
            // 默认 18.435° 的行位移会在模型中吸附为精确 1，完全复现最初版 v-3u 公式。
            int rowCycleIndex = PositiveModuloFloor(
                (v * rowAdvance) + settings.Phase - 1.0d,
                cycle.PeriodLength);

            for (int x = left; x < right; x++)
            {
                int u = x - settings.PatternX;
                // 使用图案局部坐标计算可配置周期；long 中间值允许横向步进使用完整 int 范围。
                // 正模保证图案被画布裁剪到负坐标后，周期仍与未裁剪时完全连续。
                long cyclePosition =
                    ((long)u * cycle.ColumnAdvance) +
                    rowCycleIndex;
                int cycleIndex = PositiveModulo(cyclePosition, cycle.PeriodLength);
                RgbChannelMask channels = cycle.Pixels[cycleIndex];
                int offset = x * 3;

                // OpenCV 的内存通道顺序为 BGR。
                row[offset] = (channels & RgbChannelMask.Blue) != 0 ? (byte)255 : (byte)0;
                row[offset + 1] = (channels & RgbChannelMask.Green) != 0 ? (byte)255 : (byte)0;
                row[offset + 2] = (channels & RgbChannelMask.Red) != 0 ? (byte)255 : (byte)0;
            }
        }
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
        return (int)Math.Round((long)span * index / (double)(count - 1), MidpointRounding.AwayFromZero);
    }

    private static void FillRectangleClipped(Mat canvas, int x, int y, int width, int height, Scalar color)
    {
        int left = Math.Max(0, x);
        int top = Math.Max(0, y);
        int right = (int)Math.Min(canvas.Cols, (long)x + width);
        int bottom = (int)Math.Min(canvas.Rows, (long)y + height);

        if (right <= left || bottom <= top)
        {
            return;
        }

        Cv2.Rectangle(canvas, new Rect(left, top, right - left, bottom - top), color, -1, LineTypes.Link8);
    }

    private static int PositiveModulo(long value, int divisor)
    {
        long remainder = value % divisor;
        return (int)(remainder < 0 ? remainder + divisor : remainder);
    }

    private static int PositiveModuloFloor(double value, int divisor)
    {
        if (!double.IsFinite(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), "倾斜角产生的周期坐标超出支持范围。");
        }

        double remainder = value % divisor;
        if (remainder < 0.0d)
        {
            remainder += divisor;
        }

        int index = (int)Math.Floor(remainder);
        return index >= divisor ? 0 : index;
    }
}
