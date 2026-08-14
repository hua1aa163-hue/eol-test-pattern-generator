using EolTestPatternGenerator.Models;
using EolTestPatternGenerator.Controls;
using OpenCvSharp;
using DrawingPoint = System.Drawing.Point;
using DrawingPointF = System.Drawing.PointF;
using DrawingSize = System.Drawing.Size;

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
        VerifyAdjustableScreenOnePatterns();
        VerifyRgbPixelOrders();
        VerifyImportedImageBorderOverlay();
        VerifyPreviewCoordinateTransform();
        VerifyNonIntegerContinuousFusion();
        VerifyDiscreteCrosstalkAndLightTools();
        VerifyUserSettingsPersistence();
    }

    /// <summary>
    /// 验证缩放、拖动后的客户区位置仍能准确换算为原图像素坐标。
    /// </summary>
    private static void VerifyPreviewCoordinateTransform()
    {
        var clientSize = new DrawingSize(800, 400);
        var initialViewCenter = new DrawingPointF(50F, 40F);
        DrawingPointF imagePoint = PreviewCoordinateTransform.ClientToImage(
            new DrawingPointF(460F, 220F),
            clientSize,
            initialViewCenter,
            2F);
        AssertPointClose(imagePoint, new DrawingPointF(80F, 50F), "初始预览坐标");

        // 以 (460,220) 为滚轮锚点缩放到 3 倍后，同一客户点仍应对应同一原图位置。
        var zoomedViewCenter = new DrawingPointF(60F, 43.333333F);
        imagePoint = PreviewCoordinateTransform.ClientToImage(
            new DrawingPointF(460F, 220F),
            clientSize,
            zoomedViewCenter,
            3F);
        AssertPointClose(imagePoint, new DrawingPointF(80F, 50F), "缩放锚点坐标");

        // 再拖动 (30,-12)，原鼠标像素应跟随到新的鼠标位置 (490,208)。
        var draggedViewCenter = new DrawingPointF(50F, 47.333333F);
        imagePoint = PreviewCoordinateTransform.ClientToImage(
            new DrawingPointF(490F, 208F),
            clientSize,
            draggedViewCenter,
            3F);
        AssertPointClose(imagePoint, new DrawingPointF(80F, 50F), "拖动后的坐标");

        DrawingPointF imageCenterClient = PreviewCoordinateTransform.ImageToClient(
            new DrawingPointF(50F, 40F),
            clientSize,
            draggedViewCenter,
            3F);
        AssertPointClose(imageCenterClient, new DrawingPointF(400F, 178F), "拖动后的图片中心");

        var imageSize = new DrawingSize(100, 80);
        AssertPixel(new DrawingPoint(350, 160), new DrawingPoint(0, 0), "左上边界");
        AssertPixel(new DrawingPoint(449, 239), new DrawingPoint(99, 79), "右下有效像素");
        AssertPixel(new DrawingPoint(450, 240), null, "右下外边界");
        AssertPixel(new DrawingPoint(349, 159), null, "左上外边界");

        // 小数图像坐标必须向下取整；这个断言可防止误改为四舍五入。
        DrawingPoint? fractionalPixel = PreviewCoordinateTransform.TryGetImagePixel(
            new DrawingPoint(499, 239),
            clientSize,
            initialViewCenter,
            2F,
            imageSize);
        if (fractionalPixel != new DrawingPoint(99, 59))
        {
            throw new InvalidOperationException(
                $"小数坐标换算为 {fractionalPixel?.ToString() ?? "图片外"}，期望 {{X=99,Y=59}}。");
        }

        DrawingPoint? fractionalBoundary = PreviewCoordinateTransform.TryGetImagePixel(
            new DrawingPoint(500, 200),
            clientSize,
            initialViewCenter,
            2F,
            imageSize);
        if (fractionalBoundary is not null)
        {
            throw new InvalidOperationException($"右边界应位于图片外，实际为 {fractionalBoundary}。");
        }

        // 奇数客户区的几何中心保留半像素，避免整数除法造成整体偏移。
        DrawingPointF oddCenter = PreviewCoordinateTransform.ClientToImage(
            new DrawingPointF(400.5F, 200.5F),
            new DrawingSize(801, 401),
            initialViewCenter,
            1F);
        AssertPointClose(oddCenter, initialViewCenter, "奇数客户区中心");

        void AssertPixel(DrawingPoint clientPoint, DrawingPoint? expected, string description)
        {
            DrawingPoint? actual = PreviewCoordinateTransform.TryGetImagePixel(
                clientPoint,
                clientSize,
                initialViewCenter,
                1F,
                imageSize);
            if (actual != expected)
            {
                throw new InvalidOperationException(
                    $"{description}换算为 {actual?.ToString() ?? "图片外"}，" +
                    $"期望 {expected?.ToString() ?? "图片外"}。");
            }
        }

        static void AssertPointClose(DrawingPointF actual, DrawingPointF expected, string description)
        {
            const float tolerance = 0.0001F;
            if (Math.Abs(actual.X - expected.X) > tolerance ||
                Math.Abs(actual.Y - expected.Y) > tolerance)
            {
                throw new InvalidOperationException(
                    $"{description}为 ({actual.X},{actual.Y})，" +
                    $"期望 ({expected.X},{expected.Y})。");
            }
        }
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

    /// <summary>
    /// 验证“1号屏图卡”独立窗口使用的可调画布和左右区域算法。
    /// 测试区域故意重叠，以确认白色区域同样参与绘制，而不是仅依赖白色背景。
    /// </summary>
    private static void VerifyAdjustableScreenOnePatterns()
    {
        var settings = new ScreenOneSettings
        {
            CanvasWidth = 10,
            CanvasHeight = 8,
            LeftX = 1,
            LeftY = 1,
            LeftWidth = 5,
            LeftHeight = 5,
            RightX = 4,
            RightY = 2,
            RightWidth = 4,
            RightHeight = 4
        };

        Verify(
            ScreenOneCardKind.BlackLeftWhiteRight,
            static (x, y) =>
                IsInside(x, y, 1, 1, 5, 5) &&
                !IsInside(x, y, 4, 2, 4, 4));
        Verify(
            ScreenOneCardKind.WhiteLeftBlackRight,
            static (x, y) => IsInside(x, y, 4, 2, 4, 4));
        Verify(
            ScreenOneCardKind.BlackBoth,
            static (x, y) =>
                IsInside(x, y, 1, 1, 5, 5) ||
                IsInside(x, y, 4, 2, 4, 4));

        void Verify(ScreenOneCardKind cardKind, Func<int, int, bool> isExpectedBlack)
        {
            using Mat image = ScreenOnePatternGenerator.Generate(settings, cardKind);
            AssertImageShape(image, settings.CanvasWidth, settings.CanvasHeight, $"可调1号屏 {cardKind}");

            ForEachPixel(image, (x, y, blue, green, red) =>
            {
                AssertBinaryChannel(blue, PatternType.ScreenSplit, x, y, "B");
                AssertBinaryChannel(green, PatternType.ScreenSplit, x, y, "G");
                AssertBinaryChannel(red, PatternType.ScreenSplit, x, y, "R");

                byte expected = isExpectedBlack(x, y) ? (byte)0 : (byte)255;
                if (blue != expected || green != expected || red != expected)
                {
                    throw new InvalidOperationException(
                        $"可调1号屏 {cardKind} 在 ({x},{y}) 的 BGR=({blue},{green},{red})，" +
                        $"期望 {(expected == 0 ? "黑色" : "白色")}。");
                }
            });
        }
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

    /// <summary>
    /// 验证 MATLAB 连续覆盖率算法的 RGB 子像素映射、非整数覆盖率和左右眼反转。
    /// </summary>
    private static void VerifyNonIntegerContinuousFusion()
    {
        var settings = new NonIntegerFusionSettings
        {
            CanvasWidth = 3,
            CanvasHeight = 1,
            SubpixelPeriod = 8d,
            ScaleFactor = 1d,
            ThetaDegrees = 0d,
            PhaseShift = 0d,
            SourceMode = NonIntegerFusionSourceMode.FullWhiteBlack
        };

        using (Mat integerPeriod = NonIntegerFusionGenerator.Generate(settings))
        {
            AssertImageShape(integerPeriod, 3, 1, "整数周期连续融合");
            AssertBgr(integerPeriod, 0, 0, 255, 255, 255, "整数周期像素0");
            AssertBgr(integerPeriod, 1, 0, 0, 0, 255, "整数周期像素1");
            AssertBgr(integerPeriod, 2, 0, 255, 0, 0, "整数周期像素2");
        }

        settings.CanvasWidth = 2;
        settings.SubpixelPeriod = 8.5d;
        using (Mat nonIntegerPeriod = NonIntegerFusionGenerator.Generate(settings))
        {
            AssertBgr(nonIntegerPeriod, 1, 0, 0, 64, 255, "8.5子像素周期覆盖率");
        }

        long supercycle = NonIntegerFusionGenerator.CalculateSupercycleLength(8.5d);
        if (supercycle != 17)
        {
            throw new InvalidOperationException($"8.5 子像素周期的超周期为 {supercycle}，期望 17。");
        }

        settings.CanvasWidth = 1;
        settings.SubpixelPeriod = 8d;
        settings.SourceMode = NonIntegerFusionSourceMode.DutyWhiteBlack;
        settings.Duty = 0.25d;
        using (Mat duty = NonIntegerFusionGenerator.Generate(settings))
        {
            AssertBgr(duty, 0, 0, 0, 0, 255, "Duty 左眼源图");
        }

        settings.SourceMode = NonIntegerFusionSourceMode.FullWhiteBlack;
        settings.ReverseEyes = true;
        using Mat reversed = NonIntegerFusionGenerator.Generate(settings);
        AssertBgr(reversed, 0, 0, 0, 0, 0, "连续融合左右眼反转");

        var customSettings = new NonIntegerFusionSettings
        {
            CanvasWidth = 1,
            CanvasHeight = 1,
            SubpixelPeriod = 8d,
            ScaleFactor = 1d,
            ThetaDegrees = 0d,
            SourceMode = NonIntegerFusionSourceMode.CustomImages
        };
        using var customLeft = new Mat(1, 1, MatType.CV_8UC3, new Scalar(0, 0, 255));
        using var customRight = new Mat(1, 1, MatType.CV_8UC3, new Scalar(255, 0, 0));
        using (Mat custom = NonIntegerFusionGenerator.Generate(customSettings, customLeft, customRight))
        {
            AssertBgr(custom, 0, 0, 0, 0, 255, "自定义彩色图源");
        }

        customSettings.ReverseEyes = true;
        using (Mat customReversed = NonIntegerFusionGenerator.Generate(customSettings, customLeft, customRight))
        {
            AssertBgr(customReversed, 0, 0, 255, 0, 0, "自定义彩色图源反转");
        }

        UnicodeImageLoader.SwapRedBlueInPlace(customLeft);
        AssertBgr(customLeft, 0, 0, 255, 0, 0, "旧 MATLAB R/B 兼容转换");
    }

    /// <summary>
    /// 验证 Lt_Source_Creator 离散公式、原文件名、全组解析和 LightTools 文本布局。
    /// </summary>
    private static void VerifyDiscreteCrosstalkAndLightTools()
    {
        var settings = new DiscreteCrosstalkSettings
        {
            CanvasWidth = 4,
            CanvasHeight = 2,
            TiltAngleDegrees = 18.435d,
            SubpixelPeriod = 8,
            LitSubpixelCount = 4,
            Group = 0,
            PixelSizeXMicrometers = 57.6d,
            PixelSizeYMicrometers = 57.6d,
            ScreenSizeInches = 5d,
            PartitionWidthSubpixels = 5760d,
            StrategyAngleDegrees = 0d,
            TranslationSubpixels = 0d,
            Direction = DiscreteCrosstalkDirection.LeftPositive,
            ResultType = DiscreteCrosstalkResultType.WhiteBlack,
            SingleLightSourceFile = true
        };

        string fileName = DiscreteCrosstalkGenerator.GetBmpFileName(settings, 0);
        if (!string.Equals(fileName, "5.00_8_4_0WB.bmp", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"离散串扰文件名为 {fileName}，期望 5.00_8_4_0WB.bmp。");
        }

        using Mat image = DiscreteCrosstalkGenerator.Generate(settings);
        AssertImageShape(image, 4, 2, "离散串扰4×2样例");
        ForEachPixel(image, (x, y, blue, green, red) =>
        {
            AssertBinaryChannel(blue, PatternType.PhaseStripes, x, y, "B");
            AssertBinaryChannel(green, PatternType.PhaseStripes, x, y, "G");
            AssertBinaryChannel(red, PatternType.PhaseStripes, x, y, "R");
        });

        const string expectedMesh =
            "MESH: 12 2  -0.1152 -0.0576 0.1152 0.0576\r\n" +
            "255.0000\t255.0000\t255.0000\t255.0000\t0.0000\t0.0000\t0.0000\t0.0000\t255.0000\t255.0000\t255.0000\t255.0000\t\r\n" +
            "0.0000\t255.0000\t255.0000\t255.0000\t255.0000\t0.0000\t0.0000\t0.0000\t0.0000\t255.0000\t255.0000\t255.0000\t\r\n" +
            "\r\n";
        string actualMesh = LightToolsMeshWriter.CreateSingleSourceText(settings, image);
        if (!string.Equals(actualMesh, expectedMesh, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("离散串扰4×2样例的 LightTools MESH 文本与参考结果不一致。");
        }

        settings.Group = -1;
        IReadOnlyList<int> groups = DiscreteCrosstalkGenerator.ResolveGroups(settings);
        if (groups.Count != 8 || !groups.SequenceEqual(Enumerable.Range(0, 8)))
        {
            throw new InvalidOperationException("离散串扰 Group=-1 未解析为 0 到 7 共八组。");
        }
    }

    /// <summary>
    /// 使用独立临时文件验证用户输入可以跨实例保存，且 Load 返回深副本。
    /// </summary>
    private static void VerifyUserSettingsPersistence()
    {
        string uniqueFileName = $"EolTestPatternGenerator_Settings_{Guid.NewGuid():N}.json";
        string temporaryRoot = Path.GetFullPath(Path.GetTempPath());
        string settingsPath = Path.GetFullPath(Path.Combine(temporaryRoot, uniqueFileName));
        if (!string.Equals(Path.GetDirectoryName(settingsPath),
                temporaryRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(Path.GetFileName(settingsPath), uniqueFileName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("无法确认设置自检临时文件的安全范围。");
        }

        try
        {
            var writer = new UserSettingsStore(settingsPath);
            writer.UpdateAndSave(preferences =>
            {
                preferences.Main.Quality = 42;
                preferences.NonIntegerBlend.Matlab.WriteMesh = false;
                preferences.NonIntegerBlend.Discrete.OutputFormat = ImageFormatKind.Bmp;
                preferences.NonIntegerBlend.Converter.SourceImagePath = @"C:\测试\输入图.png";
            });

            var reader = new UserSettingsStore(settingsPath);
            ApplicationPreferences loaded = reader.Load();
            if (loaded.Main.Quality != 42 ||
                loaded.NonIntegerBlend.Matlab.WriteMesh ||
                loaded.NonIntegerBlend.Discrete.OutputFormat != ImageFormatKind.Bmp ||
                !string.Equals(
                    loaded.NonIntegerBlend.Converter.SourceImagePath,
                    @"C:\测试\输入图.png",
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException("用户输入未能从 JSON 设置文件完整恢复。");
            }

            loaded.Main.Quality = 7;
            if (reader.Load().Main.Quality != 42)
            {
                throw new InvalidOperationException("设置仓库 Load 返回了共享的可变对象，而不是深副本。");
            }
        }
        finally
        {
            if (File.Exists(settingsPath))
            {
                File.Delete(settingsPath);
            }
        }
    }

    private static void AssertBgr(
        Mat image,
        int x,
        int y,
        byte expectedBlue,
        byte expectedGreen,
        byte expectedRed,
        string description)
    {
        (byte blue, byte green, byte red) = GetBgrPixel(image, x, y);
        if (blue != expectedBlue || green != expectedGreen || red != expectedRed)
        {
            throw new InvalidOperationException(
                $"{description}在 ({x},{y}) 的 BGR=({blue},{green},{red})，" +
                $"期望 ({expectedBlue},{expectedGreen},{expectedRed})。");
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
