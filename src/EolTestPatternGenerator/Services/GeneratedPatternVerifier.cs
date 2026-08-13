using EolTestPatternGenerator.Models;
using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

/// <summary>
/// 对生成算法执行不依赖外部参考图片的像素级回归验证。
/// 验证失败时抛出 <see cref="InvalidOperationException"/>。
/// </summary>
public static class GeneratedPatternVerifier
{
    public static void RunAll()
    {
        VerifySolidColors();
        VerifyScreenOneTwoDimensionalPatterns();
        VerifyRgbPixelOrders();
        VerifyImportedImageBorderOverlay();
    }

    private static void VerifyImportedImageBorderOverlay()
    {
        string uniqueDirectoryName = $"EolTestPatternGenerator_Verifier_{Guid.NewGuid():N}";
        string temporaryRoot = Path.GetFullPath(Path.GetTempPath());
        string temporaryDirectory = Path.GetFullPath(Path.Combine(temporaryRoot, uniqueDirectoryName));

        if (!string.Equals(Path.GetDirectoryName(temporaryDirectory),
                temporaryRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(Path.GetFileName(temporaryDirectory), uniqueDirectoryName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("无法确认回归验证临时目录的安全范围。");
        }

        Directory.CreateDirectory(temporaryDirectory);
        try
        {
            string sourcePath = Path.Combine(temporaryDirectory, "中文二值底图.png");
            using (var source = new Mat(2, 3, MatType.CV_8UC3, Scalar.Black))
            {
                SetBgrPixel(source, 1, 0, 255, 255, 255);
                SetBgrPixel(source, 2, 0, 0, 0, 255);
                SetBgrPixel(source, 0, 1, 0, 255, 0);
                SetBgrPixel(source, 1, 1, 255, 0, 0);
                SetBgrPixel(source, 2, 1, 255, 255, 255);

                if (!Cv2.ImEncode(".png", source, out byte[] encoded) || encoded.Length == 0)
                {
                    throw new InvalidOperationException("无法编码含中文路径回归验证所需的二值 PNG 底图。");
                }

                File.WriteAllBytes(sourcePath, encoded);
            }

            var settings = new PatternSettings
            {
                PatternType = PatternType.ImportedImage,
                CanvasWidth = 9,
                CanvasHeight = 6,
                PatternX = 0,
                PatternY = 0,
                PatternWidth = 9,
                PatternHeight = 6,
                SourceImagePath = sourcePath,
                BorderOverlay = new BorderOverlaySettings
                {
                    Enabled = true,
                    X = 2,
                    Y = 1,
                    Width = 5,
                    Height = 4,
                    LineWidth = 1
                }
            };

            using Mat actual = PatternGenerator.Generate(settings);
            AssertImageShape(actual, 9, 6, "含中文路径的导入底图白框");
            int rows = actual.Rows;
            int columns = actual.Cols;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    bool onBorder =
                        ((y == 1 || y == 4) && x >= 2 && x <= 6) ||
                        ((x == 2 || x == 6) && y >= 1 && y <= 4);
                    (byte expectedBlue, byte expectedGreen, byte expectedRed) = onBorder
                        ? ((byte)255, (byte)255, (byte)255)
                        : GetNearestNeighborExpectedPixel(x, y);
                    (byte actualBlue, byte actualGreen, byte actualRed) = GetBgrPixel(actual, x, y);

                    if (actualBlue != expectedBlue ||
                        actualGreen != expectedGreen ||
                        actualRed != expectedRed)
                    {
                        throw new InvalidOperationException(
                            $"导入底图白框在 ({x},{y}) 的 BGR=({actualBlue},{actualGreen},{actualRed})，" +
                            $"期望 ({expectedBlue},{expectedGreen},{expectedRed})。" +
                            (onBorder ? "该像素应属于白框。" : "该像素应保留最近邻缩放后的底图。"));
                    }
                }
            }
        }
        finally
        {
            string confirmedDirectory = Path.GetFullPath(temporaryDirectory);
            bool isConfirmedUniqueDirectory =
                string.Equals(confirmedDirectory, temporaryDirectory, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(Path.GetDirectoryName(confirmedDirectory),
                    temporaryRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                    StringComparison.OrdinalIgnoreCase) &&
                string.Equals(Path.GetFileName(confirmedDirectory), uniqueDirectoryName, StringComparison.Ordinal) &&
                uniqueDirectoryName.StartsWith("EolTestPatternGenerator_Verifier_", StringComparison.Ordinal) &&
                uniqueDirectoryName.Length == "EolTestPatternGenerator_Verifier_".Length + 32;

            if (!isConfirmedUniqueDirectory)
            {
                throw new InvalidOperationException("拒绝清理未经确认的回归验证临时目录。");
            }

            if (Directory.Exists(confirmedDirectory))
            {
                Directory.Delete(confirmedDirectory, recursive: true);
            }
        }

        static (byte Blue, byte Green, byte Red) GetNearestNeighborExpectedPixel(int x, int y)
        {
            // 3×2 最近邻放大到 9×6：每个源像素成为一个 3×3 色块。
            int sourceX = x / 3;
            int sourceY = y / 3;
            return (sourceX, sourceY) switch
            {
                (0, 0) => (0, 0, 0),
                (1, 0) => (255, 255, 255),
                (2, 0) => (0, 0, 255),
                (0, 1) => (0, 255, 0),
                (1, 1) => (255, 0, 0),
                (2, 1) => (255, 255, 255),
                _ => throw new InvalidOperationException("最近邻缩放回归验证坐标超出预期范围。")
            };
        }
    }

    private static void VerifySolidColors()
    {
        VerifySolidColor(PatternType.Black, blue: 0, green: 0, red: 0);
        VerifySolidColor(PatternType.FullWhite, blue: 255, green: 255, red: 255);
        VerifySolidColor(PatternType.FullRed, blue: 0, green: 0, red: 255);
        VerifySolidColor(PatternType.FullGreen, blue: 0, green: 255, red: 0);
        VerifySolidColor(PatternType.FullBlue, blue: 255, green: 0, red: 0);
    }

    private static void VerifySolidColor(PatternType type, byte blue, byte green, byte red)
    {
        PatternSettings settings = PatternPresets.Create(type);
        settings.CanvasWidth = 17;
        settings.CanvasHeight = 11;
        settings.PatternWidth = settings.CanvasWidth;
        settings.PatternHeight = settings.CanvasHeight;

        using Mat image = PatternGenerator.Generate(settings);
        AssertImageShape(image, settings.CanvasWidth, settings.CanvasHeight, type.ToString());

        ForEachPixel(image, (x, y, actualBlue, actualGreen, actualRed) =>
        {
            AssertBinaryChannel(actualBlue, type, x, y, "B");
            AssertBinaryChannel(actualGreen, type, x, y, "G");
            AssertBinaryChannel(actualRed, type, x, y, "R");

            if (actualBlue != blue || actualGreen != green || actualRed != red)
            {
                throw new InvalidOperationException(
                    $"纯色图 {type} 在 ({x},{y}) 的 BGR=({actualBlue},{actualGreen},{actualRed})，" +
                    $"期望 ({blue},{green},{red})。");
            }
        });
    }

    private static void VerifyScreenOneTwoDimensionalPatterns()
    {
        VerifyScreenOnePattern(
            ScreenSplitMode.TwoDimensionalBlackLeft,
            "1.B_W.png",
            static (x, y) => IsInside(x, y, 50, 50, 1500, 1900));

        VerifyScreenOnePattern(
            ScreenSplitMode.TwoDimensionalBlackRight,
            "2.W_B.png",
            static (x, y) => IsInside(x, y, 1650, 50, 1500, 1900));

        VerifyScreenOnePattern(
            ScreenSplitMode.TwoDimensionalBlackBoth,
            "3.B.png",
            static (x, y) =>
                IsInside(x, y, 50, 50, 1500, 1900) ||
                IsInside(x, y, 1650, 50, 1500, 1900));
    }

    private static void VerifyScreenOnePattern(
        ScreenSplitMode mode,
        string expectedFileName,
        Func<int, int, bool> isExpectedBlack)
    {
        PatternSettings settings = PatternPresets.Create(PatternType.ScreenSplit);
        settings.CanvasWidth = 3200;
        settings.CanvasHeight = 2000;
        settings.ScreenSplitMode = mode;

        string actualFileName = PatternFileNames.Get(settings);
        if (!string.Equals(actualFileName, expectedFileName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"1号屏模式 {mode} 的文件名为“{actualFileName}”，期望“{expectedFileName}”。");
        }

        using Mat image = PatternGenerator.Generate(settings);
        AssertImageShape(image, 3200, 2000, $"1号屏 {expectedFileName}");

        long blackPixels = 0;
        int blackMinX = image.Cols;
        int blackMinY = image.Rows;
        int blackMaxX = -1;
        int blackMaxY = -1;

        ForEachPixel(image, (x, y, blue, green, red) =>
        {
            AssertBinaryChannel(blue, PatternType.ScreenSplit, x, y, "B");
            AssertBinaryChannel(green, PatternType.ScreenSplit, x, y, "G");
            AssertBinaryChannel(red, PatternType.ScreenSplit, x, y, "R");

            bool expectedBlack = isExpectedBlack(x, y);
            byte expected = expectedBlack ? (byte)0 : (byte)255;
            if (blue != expected || green != expected || red != expected)
            {
                throw new InvalidOperationException(
                    $"1号屏 {expectedFileName} 在 ({x},{y}) 的 BGR=({blue},{green},{red})，" +
                    $"期望 {(expectedBlack ? "黑色" : "白色")}。");
            }

            if (blue == 0 && green == 0 && red == 0)
            {
                blackPixels++;
                blackMinX = Math.Min(blackMinX, x);
                blackMinY = Math.Min(blackMinY, y);
                blackMaxX = Math.Max(blackMaxX, x);
                blackMaxY = Math.Max(blackMaxY, y);
            }
        });

        long expectedBlackPixels = mode == ScreenSplitMode.TwoDimensionalBlackBoth
            ? 2L * 1500 * 1900
            : 1500L * 1900;
        if (blackPixels != expectedBlackPixels)
        {
            throw new InvalidOperationException(
                $"1号屏 {expectedFileName} 有 {blackPixels:N0} 个黑色像素，" +
                $"期望 {expectedBlackPixels:N0} 个。");
        }

        int expectedMinX = mode == ScreenSplitMode.TwoDimensionalBlackRight ? 1650 : 50;
        int expectedMaxX = mode == ScreenSplitMode.TwoDimensionalBlackLeft ? 1549 : 3149;
        if (blackMinX != expectedMinX || blackMinY != 50 ||
            blackMaxX != expectedMaxX || blackMaxY != 1949)
        {
            throw new InvalidOperationException(
                $"1号屏 {expectedFileName} 的黑色包围盒为 " +
                $"({blackMinX},{blackMinY})-({blackMaxX},{blackMaxY})，" +
                $"期望 ({expectedMinX},50)-({expectedMaxX},1949)。");
        }
    }

    private static void VerifyRgbPixelOrders()
    {
        PatternSettings baselineSettings = CreateSmallPhaseSettings(RgbPixelOrder.RGB);
        using Mat baseline = PatternGenerator.Generate(baselineSettings);
        AssertImageShape(baseline, baselineSettings.CanvasWidth, baselineSettings.CanvasHeight, "RGB 排列基准");

        foreach (RgbPixelOrder order in Enum.GetValues<RgbPixelOrder>())
        {
            PatternSettings settings = CreateSmallPhaseSettings(order);
            using Mat actual = PatternGenerator.Generate(settings);
            AssertImageShape(actual, settings.CanvasWidth, settings.CanvasHeight, $"RGB 排列 {order}");
            VerifyChannelPermutation(baseline, actual, order);
        }
    }

    private static PatternSettings CreateSmallPhaseSettings(RgbPixelOrder order)
    {
        PatternSettings settings = PatternPresets.Create(PatternType.PhaseStripes);
        settings.CanvasWidth = 13;
        settings.CanvasHeight = 9;
        settings.PatternX = 0;
        settings.PatternY = 0;
        settings.PatternWidth = settings.CanvasWidth;
        settings.PatternHeight = settings.CanvasHeight;
        settings.Phase = 1;
        settings.PixelOrder = order;
        return settings;
    }

    private static unsafe void VerifyChannelPermutation(Mat baseline, Mat actual, RgbPixelOrder order)
    {
        int rows = baseline.Rows;
        int columns = baseline.Cols;

        for (int y = 0; y < rows; y++)
        {
            byte* baselineRow = (byte*)baseline.Ptr(y);
            byte* actualRow = (byte*)actual.Ptr(y);

            for (int x = 0; x < columns; x++)
            {
                int offset = x * 3;
                byte logicalRed = baselineRow[offset + 2];
                byte logicalGreen = baselineRow[offset + 1];
                byte logicalBlue = baselineRow[offset];
                (byte expectedRed, byte expectedGreen, byte expectedBlue) = order switch
                {
                    RgbPixelOrder.RGB => (logicalRed, logicalGreen, logicalBlue),
                    RgbPixelOrder.RBG => (logicalRed, logicalBlue, logicalGreen),
                    RgbPixelOrder.GRB => (logicalGreen, logicalRed, logicalBlue),
                    RgbPixelOrder.GBR => (logicalGreen, logicalBlue, logicalRed),
                    RgbPixelOrder.BRG => (logicalBlue, logicalRed, logicalGreen),
                    RgbPixelOrder.BGR => (logicalBlue, logicalGreen, logicalRed),
                    _ => throw new ArgumentOutOfRangeException(nameof(order), order, null)
                };

                byte actualBlue = actualRow[offset];
                byte actualGreen = actualRow[offset + 1];
                byte actualRed = actualRow[offset + 2];
                if (actualBlue != expectedBlue || actualGreen != expectedGreen || actualRed != expectedRed)
                {
                    throw new InvalidOperationException(
                        $"RGB 排列 {order} 在 ({x},{y}) 的 BGR=({actualBlue},{actualGreen},{actualRed})，" +
                        $"期望 ({expectedBlue},{expectedGreen},{expectedRed})；" +
                        $"RGB 基准为 ({logicalRed},{logicalGreen},{logicalBlue})。");
                }
            }
        }
    }

    private static void AssertImageShape(Mat image, int expectedWidth, int expectedHeight, string description)
    {
        if (image.Type() != MatType.CV_8UC3)
        {
            throw new InvalidOperationException(
                $"{description} 的 Mat 类型为 {image.Type()}，期望 {MatType.CV_8UC3}。");
        }

        if (image.Cols != expectedWidth || image.Rows != expectedHeight)
        {
            throw new InvalidOperationException(
                $"{description} 的尺寸为 {image.Cols}×{image.Rows}，" +
                $"期望 {expectedWidth}×{expectedHeight}。");
        }
    }

    private static void AssertBinaryChannel(
        byte value,
        PatternType type,
        int x,
        int y,
        string channel)
    {
        if (value is not 0 and not 255)
        {
            throw new InvalidOperationException(
                $"{type} 在 ({x},{y}) 的 {channel} 通道出现非二值像素 {value}；只允许 0 或 255。");
        }
    }

    private static bool IsInside(int x, int y, int left, int top, int width, int height)
    {
        return x >= left && x < left + width && y >= top && y < top + height;
    }

    private static unsafe void SetBgrPixel(
        Mat image,
        int x,
        int y,
        byte blue,
        byte green,
        byte red)
    {
        byte* pixel = (byte*)image.Ptr(y) + (x * 3);
        pixel[0] = blue;
        pixel[1] = green;
        pixel[2] = red;
    }

    private static unsafe (byte Blue, byte Green, byte Red) GetBgrPixel(Mat image, int x, int y)
    {
        byte* pixel = (byte*)image.Ptr(y) + (x * 3);
        return (pixel[0], pixel[1], pixel[2]);
    }

    private static unsafe void ForEachPixel(
        Mat image,
        Action<int, int, byte, byte, byte> visitor)
    {
        int rows = image.Rows;
        int columns = image.Cols;

        for (int y = 0; y < rows; y++)
        {
            byte* row = (byte*)image.Ptr(y);
            for (int x = 0; x < columns; x++)
            {
                int offset = x * 3;
                visitor(x, y, row[offset], row[offset + 1], row[offset + 2]);
            }
        }
    }
}
