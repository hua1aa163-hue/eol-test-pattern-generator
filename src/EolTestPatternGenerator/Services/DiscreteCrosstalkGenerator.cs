using System.Globalization;
using EolTestPatternGenerator.Models;
using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

/// <summary>
/// 生成“Lighttool Source Creator”风格的离散 RGB 子像素光源图。
/// </summary>
public static class DiscreteCrosstalkGenerator
{
    private const int MaximumDimension = 16384;
    private const long MaximumPixelCount = 40_000_000;

    /// <summary>
    /// 生成设置中指定的单个相位组。返回的 Mat 为标准 OpenCV BGR 图像，
    /// 所有权交给调用方，调用方必须负责 Dispose。
    /// </summary>
    public static Mat Generate(
        DiscreteCrosstalkSettings settings,
        Mat? customSourceA = null,
        Mat? customSourceB = null)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ValidateSettings(settings);

        if (settings.Group == -1)
        {
            throw new InvalidOperationException("组数为 -1 时会产生多张图，请调用 GenerateBatch 或逐组调用 GenerateGroup。");
        }

        ValidateSources(settings, customSourceA, customSourceB);
        return GenerateCore(settings, settings.Group, customSourceA, customSourceB);
    }

    /// <summary>
    /// 生成明确指定的一个相位组；该参数不会修改 settings.Group。
    /// 返回的 Mat 由调用方负责 Dispose。
    /// </summary>
    public static Mat GenerateGroup(
        DiscreteCrosstalkSettings settings,
        int group,
        Mat? customSourceA = null,
        Mat? customSourceB = null)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ValidateSettings(settings);
        ValidateGroup(group, settings.SubpixelPeriod);
        ValidateSources(settings, customSourceA, customSourceB);
        return GenerateCore(settings, group, customSourceA, customSourceB);
    }

    /// <summary>
    /// 按设置生成一个或全部相位组。回调接收到的 Mat 只在回调执行期间有效，
    /// 本方法会在回调返回后释放它，避免批量生成时积累大图占用内存。
    /// </summary>
    public static void GenerateBatch(
        DiscreteCrosstalkSettings settings,
        Action<int, Mat> consumeImage,
        Mat? customSourceA = null,
        Mat? customSourceB = null)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(consumeImage);
        ValidateSettings(settings);
        ValidateSources(settings, customSourceA, customSourceB);

        foreach (int group in ResolveGroups(settings))
        {
            using Mat image = GenerateCore(settings, group, customSourceA, customSourceB);
            consumeImage(group, image);
        }
    }

    /// <summary>
    /// 将设置解析为实际需要生成的组号。Group=-1 返回 0 到周期减一。
    /// </summary>
    public static IReadOnlyList<int> ResolveGroups(DiscreteCrosstalkSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ValidateSettings(settings);

        return settings.Group == -1
            ? Enumerable.Range(0, settings.SubpixelPeriod).ToArray()
            : [settings.Group];
    }

    /// <summary>返回与原程序相同的文件名主体，不含扩展名。</summary>
    public static string GetFileBaseName(DiscreteCrosstalkSettings settings, int group)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ValidateSettings(settings);
        ValidateGroup(group, settings.SubpixelPeriod);

        string typeCode = settings.ResultType switch
        {
            DiscreteCrosstalkResultType.WhiteBlack => "WB",
            DiscreteCrosstalkResultType.BlackWhite => "BW",
            DiscreteCrosstalkResultType.RedBlue => "RB",
            DiscreteCrosstalkResultType.Custom => "USER",
            _ => throw new ArgumentOutOfRangeException(nameof(settings.ResultType), settings.ResultType, null)
        };

        return string.Format(
            CultureInfo.InvariantCulture,
            "{0:F2}_{1}_{2}_{3}{4}",
            settings.ScreenSizeInches,
            settings.SubpixelPeriod,
            settings.LitSubpixelCount,
            group,
            typeCode);
    }

    /// <summary>返回与原程序相同的 BMP 文件名。</summary>
    public static string GetBmpFileName(DiscreteCrosstalkSettings settings, int group) =>
        GetFileBaseName(settings, group) + ".bmp";

    /// <summary>
    /// 生成设置指定的一个或全部相位组并保存为 BMP。通过内存编码写盘，
    /// 因而输出目录包含中文字符时也不会受 OpenCV 窄字符路径限制。
    /// </summary>
    public static IReadOnlyList<string> ExportBmpBatch(
        string outputDirectory,
        DiscreteCrosstalkSettings settings,
        Mat? customSourceA = null,
        Mat? customSourceB = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);
        string fullDirectory = Path.GetFullPath(outputDirectory);
        Directory.CreateDirectory(fullDirectory);

        var paths = new List<string>();
        GenerateBatch(
            settings,
            (group, image) =>
            {
                string path = Path.Combine(fullDirectory, GetBmpFileName(settings, group));
                Cv2.ImEncode(".bmp", image, out byte[] encoded);
                if (encoded.Length == 0)
                {
                    throw new IOException($"OpenCV 无法编码 BMP 图像：{path}");
                }

                ImageFileWriter.WriteAllBytesAtomically(path, encoded);
                paths.Add(path);
            },
            customSourceA,
            customSourceB);
        return paths;
    }

    internal static void ValidateSettings(DiscreteCrosstalkSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        if (settings.CanvasWidth is < 1 or > MaximumDimension ||
            settings.CanvasHeight is < 1 or > MaximumDimension)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "画布宽高必须在 1 到 16384 像素之间。");
        }

        if ((long)settings.CanvasWidth * settings.CanvasHeight > MaximumPixelCount)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "为避免内存不足，画布总像素不能超过 4000 万。");
        }

        if (settings.SubpixelPeriod is < 1 or > 4096)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "子像素周期必须在 1 到 4096 之间。");
        }

        if (settings.LitSubpixelCount < 1 || settings.LitSubpixelCount > settings.SubpixelPeriod)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "点亮颗数必须在 1 到子像素周期之间。");
        }

        if (settings.Group < -1 || settings.Group >= settings.SubpixelPeriod)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "组数必须为 -1，或在 0 到子像素周期减一之间。");
        }

        ValidatePositiveFinite(settings.PartitionWidthSubpixels, "分区宽度");
        ValidatePositiveFinite(settings.PixelSizeXMicrometers, "像素大小 X");
        ValidatePositiveFinite(settings.PixelSizeYMicrometers, "像素大小 Y");
        ValidatePositiveFinite(settings.ScreenSizeInches, "屏幕尺寸");
        ValidateFiniteRange(settings.TranslationSubpixels, 9e15, "平移量");
        ValidateAngle(settings.TiltAngleDegrees, settings.CanvasHeight, "倾斜角度");
        ValidateAngle(settings.StrategyAngleDegrees, settings.CanvasHeight, "策略角度");

        if (!Enum.IsDefined(settings.Direction))
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "未知的相位修正方向。");
        }

        if (!Enum.IsDefined(settings.ResultType))
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "未知的离散光源结果类型。");
        }
    }

    internal static void ValidateGroup(int group, int period)
    {
        if (group < 0 || group >= period)
        {
            throw new ArgumentOutOfRangeException(nameof(group), $"组号必须在 0 到 {period - 1} 之间。");
        }
    }

    private static void ValidateSources(
        DiscreteCrosstalkSettings settings,
        Mat? customSourceA,
        Mat? customSourceB)
    {
        if (settings.ResultType != DiscreteCrosstalkResultType.Custom)
        {
            return;
        }

        if (customSourceA is null || customSourceB is null)
        {
            throw new ArgumentNullException(
                customSourceA is null ? nameof(customSourceA) : nameof(customSourceB),
                "自定义模式必须同时提供图源 A 和图源 B。");
        }

        if (customSourceA.Empty() || customSourceB.Empty())
        {
            throw new InvalidDataException("自定义图源不能为空图片。");
        }

        if (customSourceA.Type() != MatType.CV_8UC3 || customSourceB.Type() != MatType.CV_8UC3)
        {
            throw new InvalidDataException("自定义图源必须都是 8 位三通道 BGR 图片。");
        }

        if (customSourceA.Cols != customSourceB.Cols || customSourceA.Rows != customSourceB.Rows)
        {
            throw new InvalidDataException("两张自定义图源的宽度和高度必须完全相同。");
        }

        if (customSourceA.Cols != settings.CanvasWidth || customSourceA.Rows != settings.CanvasHeight)
        {
            throw new InvalidDataException("自定义图源尺寸必须与设置中的输出画布尺寸完全相同。");
        }
    }

    private static unsafe Mat GenerateCore(
        DiscreteCrosstalkSettings settings,
        int group,
        Mat? customSourceA,
        Mat? customSourceB)
    {
        var output = new Mat(
            settings.CanvasHeight,
            settings.CanvasWidth,
            MatType.CV_8UC3,
            Scalar.Black);

        try
        {
            double tiltTangent = Math.Tan(DegreesToRadians(settings.TiltAngleDegrees));
            double strategyTangent = Math.Tan(DegreesToRadians(settings.StrategyAngleDegrees));
            long rowStep = checked((long)Math.Round(3d * tiltTangent, MidpointRounding.AwayFromZero));
            // 原程序在这里使用截断转换，平移量的小数部分不会进入中心线基准。
            long translatedWidth = checked((long)Math.Truncate(
                (3d * settings.CanvasWidth) + settings.TranslationSubpixels));
            int directionSign = (int)settings.Direction;
            int halfPeriod = settings.SubpixelPeriod / 2;
            bool custom = settings.ResultType == DiscreteCrosstalkResultType.Custom;

            // 内置图源按 OpenCV 的 B、G、R 内存顺序保存。
            byte[] sourceAColor = settings.ResultType switch
            {
                DiscreteCrosstalkResultType.WhiteBlack => [255, 255, 255],
                DiscreteCrosstalkResultType.BlackWhite => [0, 0, 0],
                DiscreteCrosstalkResultType.RedBlue => [0, 0, 255],
                DiscreteCrosstalkResultType.Custom => [0, 0, 0],
                _ => throw new ArgumentOutOfRangeException(nameof(settings.ResultType), settings.ResultType, null)
            };
            byte[] sourceBColor = settings.ResultType switch
            {
                DiscreteCrosstalkResultType.WhiteBlack => [0, 0, 0],
                DiscreteCrosstalkResultType.BlackWhite => [255, 255, 255],
                DiscreteCrosstalkResultType.RedBlue => [255, 0, 0],
                DiscreteCrosstalkResultType.Custom => [0, 0, 0],
                _ => throw new ArgumentOutOfRangeException(nameof(settings.ResultType), settings.ResultType, null)
            };

            for (int y = 0; y < settings.CanvasHeight; y++)
            {
                byte* destinationRow = (byte*)output.Ptr(y);
                byte* sourceARow = custom ? (byte*)customSourceA!.Ptr(y) : null;
                byte* sourceBRow = custom ? (byte*)customSourceB!.Ptr(y) : null;
                long phase = PositiveModulo(group + ((long)y * rowStep), settings.SubpixelPeriod);
                double partitionCenter = (translatedWidth + (3d * y * strategyTangent)) / 2d;
                double halfPartitionWidth = settings.PartitionWidthSubpixels / 2d;
                double leftPartitionBoundary = partitionCenter - halfPartitionWidth;
                double rightPartitionBoundary = partitionCenter + halfPartitionWidth;

                // 原程序先在 RGB 逻辑顺序中将一个像素展开为三个子像素。
                // 输出 Mat 则仍保持 OpenCV 的 BGR 内存顺序，故写入时需要 2-c 映射。
                for (int x = 0; x < settings.CanvasWidth; x++)
                {
                    int pixelOffset = x * 3;

                    for (int logicalChannel = 0; logicalChannel < 3; logicalChannel++)
                    {
                        long subpixel = pixelOffset + logicalChannel;
                        long partitionCorrection = CalculatePartitionCorrection(
                            subpixel,
                            leftPartitionBoundary,
                            rightPartitionBoundary,
                            settings.PartitionWidthSubpixels,
                            directionSign);
                        int position = PositiveModulo(
                            subpixel - partitionCorrection - phase,
                            settings.SubpixelPeriod);

                        bool useSourceA = position < settings.LitSubpixelCount;
                        bool useSourceB = !useSourceA &&
                                          position >= halfPeriod &&
                                          position < halfPeriod + settings.LitSubpixelCount;
                        if (!useSourceA && !useSourceB)
                        {
                            continue;
                        }

                        int bgrChannel = 2 - logicalChannel;
                        int channelOffset = pixelOffset + bgrChannel;
                        if (custom)
                        {
                            destinationRow[channelOffset] = useSourceA
                                ? sourceARow[channelOffset]
                                : sourceBRow[channelOffset];
                        }
                        else
                        {
                            destinationRow[channelOffset] = useSourceA
                                ? sourceAColor[bgrChannel]
                                : sourceBColor[bgrChannel];
                        }
                    }
                }
            }

            return output;
        }
        catch
        {
            output.Dispose();
            throw;
        }
    }

    private static long CalculatePartitionCorrection(
        long subpixel,
        double leftBoundary,
        double rightBoundary,
        double partitionWidth,
        int directionSign)
    {
        // 中心分区使用严格内边界；边界点归入相邻分区，与原程序的 ceil/floor 分支一致。
        if (subpixel > leftBoundary && subpixel < rightBoundary)
        {
            return 0;
        }

        double relativeToRight = (subpixel - rightBoundary) / partitionWidth;
        double partitionIndex = subpixel >= rightBoundary
            ? Math.Floor(relativeToRight + 1d)
            : Math.Ceiling(relativeToRight);
        return checked((long)partitionIndex * directionSign);
    }

    private static int PositiveModulo(long value, int divisor)
    {
        long remainder = value % divisor;
        return (int)(remainder < 0 ? remainder + divisor : remainder);
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180d;

    private static void ValidatePositiveFinite(double value, string displayName)
    {
        if (!double.IsFinite(value) || value <= 0d)
        {
            throw new ArgumentOutOfRangeException(nameof(value), $"{displayName}必须是大于 0 的有限数值。");
        }
    }

    private static void ValidateFiniteRange(double value, double maximumAbsoluteValue, string displayName)
    {
        if (!double.IsFinite(value) || Math.Abs(value) > maximumAbsoluteValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                $"{displayName}必须是绝对值不超过 {maximumAbsoluteValue:G} 的有限数值。");
        }
    }

    private static void ValidateAngle(double degrees, int height, string displayName)
    {
        if (!double.IsFinite(degrees))
        {
            throw new ArgumentOutOfRangeException(nameof(degrees), $"{displayName}必须是有限数值。");
        }

        double tangent = Math.Tan(DegreesToRadians(degrees));
        double maximumTerm = Math.Abs(3d * Math.Max(1, height - 1) * tangent);
        if (!double.IsFinite(tangent) || maximumTerm > 9e15)
        {
            throw new ArgumentOutOfRangeException(nameof(degrees), $"{displayName}过于接近正负 90 度，无法稳定计算。");
        }
    }
}
