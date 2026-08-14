using System.Drawing;
using System.Drawing.Imaging;
using EolTestPatternGenerator.Models;
using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

/// <summary>
/// 以纯 C# 复现 MATLAB 非整数子像素周期的连续左右眼融合。
/// </summary>
public static class NonIntegerFusionGenerator
{
    private const int MaximumDimension = 16384;
    private const long MaximumPixels = 40_000_000;
    private const double MaximumEffectivePeriod = 1_000_000.0;
    private const double FineRationalTolerance = 1e-6;
    private const double CoarseRationalTolerance = 1.0 / 20.0;

    // OpenCV Mat 的三通道顺序是 B、G、R。
    private static readonly Scalar White = new(255, 255, 255);
    private static readonly Scalar Red = new(0, 0, 255);
    private static readonly Scalar Blue = new(255, 0, 0);

    /// <summary>
    /// 使用内置左右眼图像生成一张融合图。
    /// 返回的 <see cref="Mat"/> 为 CV_8UC3 BGR，由调用方负责 Dispose。
    /// </summary>
    public static Mat Generate(NonIntegerFusionSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ValidateCommonSettings(settings);
        ValidateBuiltInCanvas(settings);

        if (settings.SourceMode == NonIntegerFusionSourceMode.CustomImages)
        {
            throw new InvalidOperationException(
                "自定义图像模式需要调用包含左右眼 Mat 或 Bitmap 参数的 Generate 重载。");
        }

        FusionGeometry geometry = CreateGeometry(settings);
        (Mat left, Mat right) = CreateBuiltInSources(settings, geometry);

        using (left)
        using (right)
        {
            return FuseNormalizedSources(settings, geometry, left, right);
        }
    }

    /// <summary>
    /// 创建内置模式实际参与融合的左右图源。ReverseEyes=true 时返回值已经交换，
    /// 便于界面保存与最终融合一致的 L/R TIFF。两张 Mat 均由调用方负责 Dispose。
    /// </summary>
    public static (Mat Left, Mat Right) CreateEffectiveBuiltInSources(NonIntegerFusionSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ValidateCommonSettings(settings);
        ValidateBuiltInCanvas(settings);
        if (settings.SourceMode == NonIntegerFusionSourceMode.CustomImages)
        {
            throw new InvalidOperationException("自定义模式的图源由调用方提供。");
        }

        FusionGeometry geometry = CreateGeometry(settings);
        (Mat left, Mat right) = CreateBuiltInSources(settings, geometry);
        return settings.ReverseEyes ? (right, left) : (left, right);
    }

    /// <summary>
    /// 融合调用方传入的左右眼 Mat。输入必须为 8 位灰度、BGR 或 BGRA，
    /// 两张图必须同尺寸。本方法不修改也不释放输入 Mat。
    /// 返回的 Mat 由调用方负责 Dispose。
    /// </summary>
    public static Mat Generate(
        NonIntegerFusionSettings settings,
        Mat leftSource,
        Mat rightSource)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(leftSource);
        ArgumentNullException.ThrowIfNull(rightSource);
        ValidateCustomMode(settings);

        using Mat normalizedLeft = NormalizeCustomMat(leftSource, nameof(leftSource));
        using Mat normalizedRight = NormalizeCustomMat(rightSource, nameof(rightSource));
        ValidateSourcePair(normalizedLeft, normalizedRight);

        FusionGeometry geometry = CreateGeometry(settings);
        return FuseNormalizedSources(settings, geometry, normalizedLeft, normalizedRight);
    }

    /// <summary>
    /// 融合调用方传入的左右眼 Bitmap。输入不会被修改或释放，
    /// 返回的 CV_8UC3 BGR Mat 由调用方负责 Dispose。
    /// </summary>
    public static Mat Generate(
        NonIntegerFusionSettings settings,
        Bitmap leftSource,
        Bitmap rightSource)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(leftSource);
        ArgumentNullException.ThrowIfNull(rightSource);
        ValidateCustomMode(settings);

        using Mat normalizedLeft = ConvertBitmapToBgr(leftSource);
        using Mat normalizedRight = ConvertBitmapToBgr(rightSource);
        ValidateSourcePair(normalizedLeft, normalizedRight);

        FusionGeometry geometry = CreateGeometry(settings);
        return FuseNormalizedSources(settings, geometry, normalizedLeft, normalizedRight);
    }

    /// <summary>返回 MATLAB 算法中的有效周期 T = spixtol * m。</summary>
    public static double CalculateEffectivePeriod(NonIntegerFusionSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ValidatePeriodInputs(settings.SubpixelPeriod, settings.ScaleFactor);
        return settings.SubpixelPeriod * settings.ScaleFactor;
    }

    /// <summary>
    /// 返回非整数周期的整数超周期。该值复现 MATLAB
    /// generate_cycle_pattern 的 sum(pattern)，用于对 picnum 取模。
    /// </summary>
    public static long CalculateSupercycleLength(double effectivePeriod)
    {
        ValidateEffectivePeriod(effectivePeriod);

        if (effectivePeriod % 1.0 == 0.0)
        {
            return checked((long)effectivePeriod);
        }

        Rational approximation = ApproximateRational(effectivePeriod, FineRationalTolerance);
        if (approximation.Denominator > 20)
        {
            approximation = ApproximateRational(effectivePeriod, CoarseRationalTolerance);
        }

        long basePeriod = checked((long)Math.Floor(effectivePeriod));
        double remainder = effectivePeriod - basePeriod;
        long extraCount = checked((long)Math.Round(
            remainder * approximation.Denominator,
            MidpointRounding.AwayFromZero));
        long supercycle = checked((basePeriod * approximation.Denominator) + extraCount);

        if (supercycle <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(effectivePeriod),
                "有效周期过小，无法得到可用的整数超周期。");
        }

        return supercycle;
    }

    private static (Mat Left, Mat Right) CreateBuiltInSources(
        NonIntegerFusionSettings settings,
        FusionGeometry geometry)
    {
        Mat? left = null;
        Mat? right = null;

        try
        {
            left = new Mat(
                settings.CanvasHeight,
                settings.CanvasWidth,
                MatType.CV_8UC3,
                Scalar.Black);
            right = new Mat(
                settings.CanvasHeight,
                settings.CanvasWidth,
                MatType.CV_8UC3,
                Scalar.Black);

            switch (settings.SourceMode)
            {
                case NonIntegerFusionSourceMode.FullWhiteBlack:
                    left.SetTo(White);
                    break;

                case NonIntegerFusionSourceMode.FixedOneSubpixelWhiteBlack:
                    FillPhaseLimitedLeftSource(
                        left,
                        geometry,
                        Math.Min(1.0, geometry.EffectivePeriod / 2.0));
                    break;

                case NonIntegerFusionSourceMode.DutyWhiteBlack:
                    if (!double.IsFinite(settings.Duty) || settings.Duty is < 0.0 or > 1.0)
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(settings),
                            "Duty 必须在 0 到 1 之间。");
                    }

                    // MATLAB 的 duty 是左眼半周期内的占比，因此实际宽度为 T/2*duty。
                    FillPhaseLimitedLeftSource(
                        left,
                        geometry,
                        (geometry.EffectivePeriod / 2.0) * settings.Duty);
                    break;

                case NonIntegerFusionSourceMode.RedBlue:
                    left.SetTo(Red);
                    right.SetTo(Blue);
                    break;

                case NonIntegerFusionSourceMode.CustomImages:
                    throw new InvalidOperationException("自定义图像需要由调用方传入。");

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(settings.SourceMode),
                        settings.SourceMode,
                        "未知的非整数融合图像源。");
            }

            return (left, right);
        }
        catch
        {
            left?.Dispose();
            right?.Dispose();
            throw;
        }
    }

    /// <summary>
    /// 按 MATLAB 的 R、G、B 逻辑子像素顺序生成窄带左眼源图。
    /// Mat 内存是 BGR，因此逻辑通道 q 需要映射到内存通道 2-q。
    /// </summary>
    private static unsafe void FillPhaseLimitedLeftSource(
        Mat left,
        FusionGeometry geometry,
        double litWidth)
    {
        if (litWidth <= 0.0)
        {
            return;
        }

        double halfPeriod = geometry.EffectivePeriod / 2.0;
        // Rows/Cols 是 OpenCvSharp P/Invoke 属性，在紧密循环前缓存以避免额外跨边界调用。
        int rows = left.Rows;
        int columns = left.Cols;
        for (int row = 0; row < rows; row++)
        {
            byte* destination = (byte*)left.Ptr(row);
            double rowOffset = row * 3.0 * geometry.TangentTheta;

            for (int column = 0; column < columns; column++)
            {
                int pixelOffset = column * 3;
                for (int logicalChannel = 0; logicalChannel < 3; logicalChannel++)
                {
                    int subpixelX = (column * 3) + logicalChannel;
                    double phase = PositiveModulo(
                        subpixelX - geometry.NormalizedPhaseShift - rowOffset,
                        geometry.EffectivePeriod);

                    if (phase < halfPeriod && phase < litWidth)
                    {
                        int bgrChannel = 2 - logicalChannel;
                        destination[pixelOffset + bgrChannel] = 255;
                    }
                }
            }
        }
    }

    private static unsafe Mat FuseNormalizedSources(
        NonIntegerFusionSettings settings,
        FusionGeometry geometry,
        Mat left,
        Mat right)
    {
        Mat effectiveLeft = settings.ReverseEyes ? right : left;
        Mat effectiveRight = settings.ReverseEyes ? left : right;
        var result = new Mat(left.Rows, left.Cols, MatType.CV_8UC3, Scalar.Black);

        try
        {
            int rows = result.Rows;
            int columns = result.Cols;
            for (int row = 0; row < rows; row++)
            {
                byte* leftRow = (byte*)effectiveLeft.Ptr(row);
                byte* rightRow = (byte*)effectiveRight.Ptr(row);
                byte* resultRow = (byte*)result.Ptr(row);
                double rowOffset = row * 3.0 * geometry.TangentTheta;

                for (int column = 0; column < columns; column++)
                {
                    int pixelOffset = column * 3;
                    for (int logicalChannel = 0; logicalChannel < 3; logicalChannel++)
                    {
                        int subpixelX = (column * 3) + logicalChannel;
                        double shiftedStart =
                            subpixelX - geometry.NormalizedPhaseShift - rowOffset;
                        double leftRatio = CalculateLeftCoverage(
                            shiftedStart,
                            shiftedStart + 1.0,
                            geometry.EffectivePeriod);

                        int bgrChannel = 2 - logicalChannel;
                        int index = pixelOffset + bgrChannel;
                        double blended =
                            (leftRow[index] * leftRatio) +
                            (rightRow[index] * (1.0 - leftRatio));
                        resultRow[index] = ToMatlabByte(blended);
                    }
                }
            }

            return result;
        }
        catch
        {
            result.Dispose();
            throw;
        }
    }

    /// <summary>
    /// 计算一个子像素区间与所有周期左半区间的交集长度。
    /// MATLAB 逐个周期累加交集；这里用等价的周期积分式，也支持 T&lt;1 时单个子像素跨越多个周期。
    /// </summary>
    private static double CalculateLeftCoverage(double start, double end, double period)
    {
        double covered =
            CalculatePeriodicLeftMeasure(end, period) -
            CalculatePeriodicLeftMeasure(start, period);
        double total = end - start;

        if (total <= 0.0)
        {
            return 0.5;
        }

        // 浮点边界上可能出现约 1e-15 的越界，裁剪后与面积比定义一致。
        return Math.Clamp(covered / total, 0.0, 1.0);
    }

    private static double CalculatePeriodicLeftMeasure(double coordinate, double period)
    {
        double cycle = Math.Floor(coordinate / period);
        double remainder = coordinate - (cycle * period);

        if (remainder < 0.0)
        {
            remainder += period;
            cycle -= 1.0;
        }
        else if (remainder >= period)
        {
            remainder -= period;
            cycle += 1.0;
        }

        return (cycle * period / 2.0) + Math.Min(remainder, period / 2.0);
    }

    private static byte ToMatlabByte(double value)
    {
        // MATLAB uint8(double) 会四舍五入并饱和到 0..255，不是 C# 的直接截断。
        double rounded = Math.Floor(value + 0.5);
        return (byte)Math.Clamp(rounded, byte.MinValue, byte.MaxValue);
    }

    private static FusionGeometry CreateGeometry(NonIntegerFusionSettings settings)
    {
        double effectivePeriod = CalculateEffectivePeriod(settings);
        long supercycle = CalculateSupercycleLength(effectivePeriod);
        double radians = settings.ThetaDegrees * Math.PI / 180.0;
        double tangentTheta = Math.Tan(radians);

        if (!double.IsFinite(tangentTheta))
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "倾斜角导致无效的正切值。");
        }

        return new FusionGeometry(
            effectivePeriod,
            supercycle,
            PositiveModulo(settings.PhaseShift, supercycle),
            tangentTheta);
    }

    private static double PositiveModulo(double value, double modulus)
    {
        double result = value % modulus;
        return result < 0.0 ? result + modulus : result;
    }

    private static void ValidateCommonSettings(NonIntegerFusionSettings settings)
    {
        if (!Enum.IsDefined(settings.SourceMode))
        {
            throw new ArgumentOutOfRangeException(
                nameof(settings.SourceMode),
                settings.SourceMode,
                "未知的非整数融合图像源。");
        }

        ValidatePeriodInputs(settings.SubpixelPeriod, settings.ScaleFactor);

        if (!double.IsFinite(settings.ThetaDegrees))
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "倾斜角必须为有限数值。");
        }

        if (!double.IsFinite(settings.PhaseShift))
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "循环移位必须为有限数值。");
        }
    }

    private static void ValidatePeriodInputs(double period, double scaleFactor)
    {
        if (!double.IsFinite(period) || period <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(period), "子像素周期必须为大于 0 的有限数值。");
        }

        if (!double.IsFinite(scaleFactor) || scaleFactor <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(scaleFactor), "缩放因子必须为大于 0 的有限数值。");
        }

        ValidateEffectivePeriod(period * scaleFactor);
    }

    private static void ValidateEffectivePeriod(double effectivePeriod)
    {
        if (!double.IsFinite(effectivePeriod) || effectivePeriod <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(effectivePeriod),
                "有效周期 T 必须为大于 0 的有限数值。");
        }

        if (effectivePeriod > MaximumEffectivePeriod)
        {
            throw new ArgumentOutOfRangeException(
                nameof(effectivePeriod),
                $"有效周期 T 不能超过 {MaximumEffectivePeriod:0} 个子像素。");
        }
    }

    private static void ValidateBuiltInCanvas(NonIntegerFusionSettings settings)
    {
        if (settings.CanvasWidth is < 1 or > MaximumDimension ||
            settings.CanvasHeight is < 1 or > MaximumDimension)
        {
            throw new ArgumentOutOfRangeException(
                nameof(settings),
                $"画布宽高必须在 1 到 {MaximumDimension} 像素之间。");
        }

        ValidatePixelCount(settings.CanvasWidth, settings.CanvasHeight);
    }

    private static void ValidateCustomMode(NonIntegerFusionSettings settings)
    {
        ValidateCommonSettings(settings);
        if (settings.SourceMode != NonIntegerFusionSourceMode.CustomImages)
        {
            throw new InvalidOperationException(
                "传入自定义左右眼图像时，SourceMode 必须设为 CustomImages。");
        }
    }

    private static void ValidateSourcePair(Mat left, Mat right)
    {
        if (left.Rows != right.Rows || left.Cols != right.Cols)
        {
            throw new ArgumentException("左右眼图像必须具有完全相同的宽度和高度。");
        }

        if (left.Cols > MaximumDimension || left.Rows > MaximumDimension)
        {
            throw new ArgumentOutOfRangeException(
                nameof(left),
                $"自定义图像宽高不能超过 {MaximumDimension} 像素。");
        }

        ValidatePixelCount(left.Cols, left.Rows);
    }

    private static void ValidatePixelCount(int width, int height)
    {
        if ((long)width * height > MaximumPixels)
        {
            throw new ArgumentOutOfRangeException(
                nameof(width),
                $"为避免内存不足，图像总像素不能超过 {MaximumPixels:N0}。");
        }
    }

    private static Mat NormalizeCustomMat(Mat source, string parameterName)
    {
        if (source.Empty())
        {
            throw new ArgumentException("自定义图像不能为空。", parameterName);
        }

        if (source.Type() == MatType.CV_8UC3)
        {
            return source.Clone();
        }

        var converted = new Mat();
        try
        {
            if (source.Type() == MatType.CV_8UC1)
            {
                Cv2.CvtColor(source, converted, ColorConversionCodes.GRAY2BGR);
                return converted;
            }

            if (source.Type() == MatType.CV_8UC4)
            {
                Cv2.CvtColor(source, converted, ColorConversionCodes.BGRA2BGR);
                return converted;
            }

            throw new ArgumentException(
                $"自定义 Mat 类型 {source.Type()} 不受支持，请传入 CV_8UC1、CV_8UC3 或 CV_8UC4。",
                parameterName);
        }
        catch
        {
            converted.Dispose();
            throw;
        }
    }

    private static unsafe Mat ConvertBitmapToBgr(Bitmap bitmap)
    {
        if (bitmap.Width < 1 || bitmap.Height < 1)
        {
            throw new ArgumentException("Bitmap 不能为空。", nameof(bitmap));
        }

        Bitmap? converted = null;
        Bitmap lockSource = bitmap;

        try
        {
            if (bitmap.PixelFormat != PixelFormat.Format24bppRgb)
            {
                converted = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);
                using Graphics graphics = Graphics.FromImage(converted);
                graphics.Clear(Color.Black);
                graphics.DrawImageUnscaled(bitmap, 0, 0);
                lockSource = converted;
            }

            return Copy24BitBitmapToMat(lockSource);
        }
        finally
        {
            // 非 24 位 Bitmap 使用了内部转换副本，不影响调用方传入的原 Bitmap。
            converted?.Dispose();
        }
    }

    private static unsafe Mat Copy24BitBitmapToMat(Bitmap bitmap)
    {
        var result = new Mat(bitmap.Height, bitmap.Width, MatType.CV_8UC3, Scalar.Black);
        BitmapData? bitmapData = null;

        try
        {
            var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            bitmapData = bitmap.LockBits(
                rectangle,
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            int rowBytes = bitmap.Width * 3;

            for (int row = 0; row < bitmap.Height; row++)
            {
                byte* sourceRow = (byte*)bitmapData.Scan0 + (row * bitmapData.Stride);
                byte* destinationRow = (byte*)result.Ptr(row);
                Buffer.MemoryCopy(sourceRow, destinationRow, rowBytes, rowBytes);
            }

            return result;
        }
        catch
        {
            result.Dispose();
            throw;
        }
        finally
        {
            if (bitmapData is not null)
            {
                bitmap.UnlockBits(bitmapData);
            }
        }
    }

    /// <summary>
    /// 使用连分数取得与 MATLAB rat(value,tolerance) 相同的低阶有理近似。
    /// 只有分母参与超周期计算，但同时保留分子便于判定误差。
    /// </summary>
    private static Rational ApproximateRational(double value, double tolerance)
    {
        double target = Math.Abs(value);
        long previousNumerator = 0;
        long numerator = 1;
        long previousDenominator = 1;
        long denominator = 0;
        double continuedValue = target;

        for (int iteration = 0; iteration < 64; iteration++)
        {
            double coefficientValue = Math.Floor(continuedValue);
            if (coefficientValue > long.MaxValue)
            {
                break;
            }

            long coefficient = (long)coefficientValue;
            long nextNumerator;
            long nextDenominator;

            try
            {
                nextNumerator = checked((coefficient * numerator) + previousNumerator);
                nextDenominator = checked((coefficient * denominator) + previousDenominator);
            }
            catch (OverflowException)
            {
                break;
            }

            previousNumerator = numerator;
            numerator = nextNumerator;
            previousDenominator = denominator;
            denominator = nextDenominator;

            if (denominator > 0 &&
                Math.Abs(target - (numerator / (double)denominator)) <= tolerance)
            {
                return new Rational(value < 0.0 ? -numerator : numerator, denominator);
            }

            double fractionalPart = continuedValue - coefficientValue;
            if (fractionalPart <= double.Epsilon)
            {
                break;
            }

            continuedValue = 1.0 / fractionalPart;
        }

        throw new ArgumentOutOfRangeException(
            nameof(value),
            "无法为有效周期构造稳定的有理数近似。");
    }

    private readonly record struct FusionGeometry(
        double EffectivePeriod,
        long Supercycle,
        double NormalizedPhaseShift,
        double TangentTheta);

    private readonly record struct Rational(long Numerator, long Denominator);
}
