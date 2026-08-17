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
        VerifyMarginGeometryAndPresets();
        VerifyNegativeMarginsAndSingleDot();
        VerifySharedDotRadiusPreservesBothLayouts();
        VerifyMainBatchUsesPerPatternProfiles();
        VerifyScreenOneTwoDimensionalPatterns();
        VerifyAdjustableScreenOnePatterns();
        VerifyRgbPixelOrders();
        VerifyCrosstalkTiltAndReferenceDefaults();
        VerifyImportedImageBorderOverlay();
        VerifyPreviewCoordinateTransform();
        VerifyNonIntegerContinuousFusion();
        VerifyDiscreteCrosstalkAndLightTools();
        VerifyUserSettingsPersistence();
        VerifySettingsRecoveryBackup();
        VerifyDialogDirectoryResolution();
        VerifyLegacySettingsMigration();
    }

    /// <summary>验证九点阵与畸变点阵双向同步半径时，两套独立圆心布局都不漂移。</summary>
    private static void VerifySharedDotRadiusPreservesBothLayouts()
    {
        PatternSettings ninePoint = PatternPresets.Create(PatternType.NinePointGrid);
        PatternSettings distortion = PatternPresets.Create(PatternType.DistortionGrid);
        (int X, int Y, int Width, int Height) nineGeometry =
            (ninePoint.PatternX, ninePoint.PatternY, ninePoint.PatternWidth, ninePoint.PatternHeight);
        (int X, int Y, int Width, int Height) distortionGeometry =
            (distortion.PatternX, distortion.PatternY, distortion.PatternWidth, distortion.PatternHeight);

        // 模拟在九点阵页把共享半径改为 12。
        ninePoint.SetDotRadiusPreservingCenterGeometry(12);
        distortion.SetDotRadiusPreservingCenterGeometry(12);
        AssertGeometry(ninePoint, nineGeometry, 12, "九点阵→畸变点阵");
        AssertGeometry(distortion, distortionGeometry, 12, "九点阵→畸变点阵");

        // 模拟切到畸变点阵后反向把共享半径改为 2。
        ninePoint.SetDotRadiusPreservingCenterGeometry(2);
        distortion.SetDotRadiusPreservingCenterGeometry(2);
        AssertGeometry(ninePoint, nineGeometry, 2, "畸变点阵→九点阵");
        AssertGeometry(distortion, distortionGeometry, 2, "畸变点阵→九点阵");

        PatternSettings singleDot = PatternPresets.Create(PatternType.NinePointGrid);
        singleDot.Rows = 1;
        singleDot.Columns = 1;
        singleDot.NormalizeSingleAxisDotSpans();
        int singleCenterX = singleDot.PatternX;
        int singleCenterY = singleDot.PatternY;
        singleDot.SetDotRadiusPreservingCenterGeometry(9);
        if (singleDot.PatternX != singleCenterX || singleDot.PatternY != singleCenterY ||
            singleDot.PatternWidth != 0 || singleDot.PatternHeight != 0)
        {
            throw new InvalidOperationException("单行单列点阵同步半径后未保持 0 圆心跨度。");
        }

        static void AssertGeometry(
            PatternSettings settings,
            (int X, int Y, int Width, int Height) expected,
            int expectedRadius,
            string description)
        {
            if (settings.DotRadius != expectedRadius ||
                settings.PatternX != expected.X || settings.PatternY != expected.Y ||
                settings.PatternWidth != expected.Width || settings.PatternHeight != expected.Height)
            {
                throw new InvalidOperationException($"{description}同步半径后圆心几何发生漂移。");
            }
        }
    }

    /// <summary>
    /// 验证用户在点阵/外框页设置的半径和线宽不会被批量导出时
    /// 当前激活的纯色图卡中无关的默认值覆盖。
    /// </summary>
    private static void VerifyMainBatchUsesPerPatternProfiles()
    {
        PatternSettings gridProfile = PatternPresets.Create(PatternType.NinePointGrid);
        gridProfile.DotRadius = 13;
        PatternSettings borderProfile = PatternPresets.Create(PatternType.Border);
        borderProfile.LineWidth = 17;
        var profiles = new Dictionary<PatternType, PatternSettings>
        {
            [PatternType.NinePointGrid] = gridProfile,
            [PatternType.Border] = borderProfile
        };

        PatternSettings exportedGrid = MainPatternBatchExporter.CreateExportSettings(
            PatternType.NinePointGrid,
            1920,
            1080,
            fallbackDotRadius: 4,
            fallbackLineWidth: 5,
            profiles);
        PatternSettings exportedBorder = MainPatternBatchExporter.CreateExportSettings(
            PatternType.Border,
            1920,
            1080,
            fallbackDotRadius: 4,
            fallbackLineWidth: 5,
            profiles);
        PatternSettings fallbackGrid = MainPatternBatchExporter.CreateExportSettings(
            PatternType.DistortionGrid,
            1920,
            1080,
            fallbackDotRadius: 9,
            fallbackLineWidth: 5,
            profiles: null);

        if (exportedGrid.DotRadius != 13 || exportedBorder.LineWidth != 17 || fallbackGrid.DotRadius != 9)
        {
            throw new InvalidOperationException(
                "基础图卡批量导出未优先使用各图卡快照中的半径/线宽。");
        }
    }

    /// <summary>验证 v2 四边距与旧参考预设的坐标、尺寸和点阵圆心跨度完全等价。</summary>
    private static void VerifyMarginGeometryAndPresets()
    {
        AssertPreset(
            PatternType.Border,
            new RegionMargins(71, 226, 72, 227),
            expectedX: 71,
            expectedY: 226,
            expectedWidth: 1777,
            expectedHeight: 627);
        AssertPreset(
            PatternType.NinePointGrid,
            new RegionMargins(72, 197, 71, 196),
            expectedX: 76,
            expectedY: 201,
            expectedWidth: 1768,
            expectedHeight: 678);
        AssertPreset(
            PatternType.DistortionGrid,
            new RegionMargins(72, 227, 71, 226),
            expectedX: 76,
            expectedY: 231,
            expectedWidth: 1768,
            expectedHeight: 618);

        foreach (PatternType type in new[]
                 {
                     PatternType.Black,
                     PatternType.FullWhite,
                     PatternType.FullRed,
                     PatternType.FullGreen,
                     PatternType.FullBlue,
                     PatternType.ImportedImage
                 })
        {
            PatternSettings fullCanvas = PatternPresets.Create(type);
            RegionMargins margins = fullCanvas.GetMargins();
            if (margins.Left != 0 || margins.Top != 0 || margins.Right != 0 || margins.Bottom != 0 ||
                fullCanvas.CalculatedOuterWidth != fullCanvas.CanvasWidth ||
                fullCanvas.CalculatedOuterHeight != fullCanvas.CanvasHeight)
            {
                throw new InvalidOperationException($"全屏预设 {type} 未固定为四边距 0。");
            }
        }

        // 极端负边距会令派生尺寸超过 int；必须得到受控参数错误而不是整数回绕。
        bool overflowRejected = false;
        try
        {
            _ = MarginGeometry.Resolve(
                1920,
                1080,
                new RegionMargins(int.MinValue, 0, int.MinValue, 0),
                "溢出测试");
        }
        catch (ArgumentOutOfRangeException)
        {
            overflowRejected = true;
        }

        if (!overflowRejected)
        {
            throw new InvalidOperationException("四边距 long 溢出校验未拒绝超出 int 的派生区域。");
        }

        static void AssertPreset(
            PatternType type,
            RegionMargins expectedMargins,
            int expectedX,
            int expectedY,
            int expectedWidth,
            int expectedHeight)
        {
            PatternSettings settings = PatternPresets.Create(type);
            RegionMargins actualMargins = settings.GetMargins();
            if (actualMargins.Left != expectedMargins.Left ||
                actualMargins.Top != expectedMargins.Top ||
                actualMargins.Right != expectedMargins.Right ||
                actualMargins.Bottom != expectedMargins.Bottom ||
                settings.PatternX != expectedX ||
                settings.PatternY != expectedY ||
                settings.PatternWidth != expectedWidth ||
                settings.PatternHeight != expectedHeight)
            {
                throw new InvalidOperationException(
                    $"{type} 四边距迁移后与旧预设坐标/尺寸不等价。");
            }
        }
    }

    /// <summary>验证负边距裁剪，以及点阵单行单列时的圆心跨度和圆外缘定义。</summary>
    private static void VerifyNegativeMarginsAndSingleDot()
    {
        PatternSettings rectangle = PatternPresets.Create(PatternType.WhiteRectangle);
        rectangle.CanvasWidth = 6;
        rectangle.CanvasHeight = 5;
        rectangle.SetMargins(new RegionMargins(-2, 1, 1, -1));
        using (Mat clipped = PatternGenerator.Generate(rectangle))
        {
            ForEachPixel(clipped, (x, y, blue, green, red) =>
            {
                byte expected = x <= 4 && y >= 1 ? (byte)255 : (byte)0;
                if (blue != expected || green != expected || red != expected)
                {
                    throw new InvalidOperationException($"负边距矩形在 ({x},{y}) 未按画布正确裁剪。");
                }
            });
        }

        PatternSettings singleDot = PatternPresets.Create(PatternType.NinePointGrid);
        singleDot.CanvasWidth = 9;
        singleDot.CanvasHeight = 7;
        singleDot.DotRadius = 1;
        singleDot.Rows = 1;
        singleDot.Columns = 1;
        singleDot.SetMargins(new RegionMargins(3, 2, 3, 2));
        if (singleDot.CalculatedOuterWidth != 3 || singleDot.CalculatedOuterHeight != 3 ||
            singleDot.CalculatedCenterSpanWidth != 0 || singleDot.CalculatedCenterSpanHeight != 0 ||
            singleDot.PatternX != 4 || singleDot.PatternY != 3)
        {
            throw new InvalidOperationException("单行单列点阵的外缘尺寸或圆心跨度计算错误。");
        }

        using Mat dotImage = PatternGenerator.Generate(singleDot);
        AssertBgr(dotImage, 4, 3, 255, 255, 255, "单点阵圆心");
        AssertBgr(dotImage, 4, 2, 255, 255, 255, "单点阵上外缘");
        AssertBgr(dotImage, 3, 3, 255, 255, 255, "单点阵左外缘");
        AssertBgr(dotImage, 0, 0, 0, 0, 0, "单点阵区域外");

        PatternSettings singleRow = PatternPresets.Create(PatternType.DistortionGrid);
        int firstCenterX = singleRow.PatternX;
        int firstCenterY = singleRow.PatternY;
        int originalHorizontalSpan = singleRow.PatternWidth;
        singleRow.Rows = 1;
        if (!singleRow.NormalizeSingleAxisDotSpans() ||
            singleRow.PatternX != firstCenterX || singleRow.PatternY != firstCenterY ||
            singleRow.PatternWidth != originalHorizontalSpan || singleRow.PatternHeight != 0 ||
            singleRow.CalculatedOuterHeight != (2L * singleRow.DotRadius) + 1L)
        {
            throw new InvalidOperationException("单行点阵未保留首圆心并将垂直跨度归零。");
        }

        PatternSettings singleColumn = PatternPresets.Create(PatternType.DistortionGrid);
        firstCenterX = singleColumn.PatternX;
        firstCenterY = singleColumn.PatternY;
        int originalVerticalSpan = singleColumn.PatternHeight;
        singleColumn.Columns = 1;
        if (!singleColumn.NormalizeSingleAxisDotSpans() ||
            singleColumn.PatternX != firstCenterX || singleColumn.PatternY != firstCenterY ||
            singleColumn.PatternWidth != 0 || singleColumn.PatternHeight != originalVerticalSpan ||
            singleColumn.CalculatedOuterWidth != (2L * singleColumn.DotRadius) + 1L)
        {
            throw new InvalidOperationException("单列点阵未保留首圆心并将水平跨度归零。");
        }

        PatternSettings inconsistentSingleAxis = PatternPresets.Create(PatternType.NinePointGrid);
        inconsistentSingleAxis.Rows = 1;
        bool inconsistentRejected = false;
        try
        {
            using Mat _ = PatternGenerator.Generate(inconsistentSingleAxis);
        }
        catch (ArgumentOutOfRangeException)
        {
            inconsistentRejected = true;
        }

        if (!inconsistentRejected)
        {
            throw new InvalidOperationException("生成器未拒绝单行但垂直圆心跨度非 0 的伪外缘配置。");
        }
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
    /// 验证“显示器”页面使用的可调画布和左右区域算法。
    /// 测试区域故意重叠，以确认白色区域同样参与绘制，而不是仅依赖白色背景。
    /// </summary>
    private static void VerifyAdjustableScreenOnePatterns()
    {
        var settings = new ScreenOneSettings
        {
            CanvasWidth = 10,
            CanvasHeight = 8,
            LeftRegionLeftMargin = 1,
            LeftRegionTopMargin = 1,
            LeftRegionRightMargin = 4,
            LeftRegionBottomMargin = 2,
            RightRegionLeftMargin = 4,
            RightRegionTopMargin = 2,
            RightRegionRightMargin = 2,
            RightRegionBottomMargin = 2
        };

        if (settings.CalculatedLeftWidth != 5 || settings.CalculatedLeftHeight != 5 ||
            settings.CalculatedRightWidth != 4 || settings.CalculatedRightHeight != 4)
        {
            throw new InvalidOperationException("显示器左右区域的四边距派生尺寸错误。");
        }

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
            AssertImageShape(image, settings.CanvasWidth, settings.CanvasHeight, $"可调显示器图卡 {cardKind}");

            ForEachPixel(image, (x, y, blue, green, red) =>
            {
                AssertBinaryChannel(blue, PatternType.ScreenSplit, x, y, "B");
                AssertBinaryChannel(green, PatternType.ScreenSplit, x, y, "G");
                AssertBinaryChannel(red, PatternType.ScreenSplit, x, y, "R");

                byte expected = isExpectedBlack(x, y) ? (byte)0 : (byte)255;
                if (blue != expected || green != expected || red != expected)
                {
                    throw new InvalidOperationException(
                        $"可调显示器图卡 {cardKind} 在 ({x},{y}) 的 BGR=({blue},{green},{red})，" +
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
                $"显示器模式 {mode} 的文件名为“{actualFileName}”，期望“{expectedFileName}”。");
        }

        using Mat image = PatternGenerator.Generate(settings);
        AssertImageShape(image, 3200, 2000, $"显示器 {expectedFileName}");

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
                    $"显示器 {expectedFileName} 在 ({x},{y}) 的 BGR=({blue},{green},{red})，" +
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
                $"显示器 {expectedFileName} 有 {blackPixels:N0} 个黑色像素，" +
                $"期望 {expectedBlackPixels:N0} 个。");
        }

        int expectedMinX = mode == ScreenSplitMode.TwoDimensionalBlackRight ? 1650 : 50;
        int expectedMaxX = mode == ScreenSplitMode.TwoDimensionalBlackLeft ? 1549 : 3149;
        if (blackMinX != expectedMinX || blackMinY != 50 ||
            blackMaxX != expectedMaxX || blackMaxY != 1949)
        {
            throw new InvalidOperationException(
                $"显示器 {expectedFileName} 的黑色包围盒为 " +
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

            // 显式采用新版周期预设时必须与没有 PixelCycle 字段的旧配置逐像素一致。
            PatternSettings migrated = settings.Clone();
            migrated.PixelOrder = order == RgbPixelOrder.RGB ? RgbPixelOrder.BGR : RgbPixelOrder.RGB;
            migrated.PixelCycle = CrosstalkPixelCyclePresets.CreateLegacy(order);
            using Mat migratedImage = PatternGenerator.Generate(migrated);
            AssertImagesEqual(actual, migratedImage, $"旧 {order} 排列迁移");

            if (!CrosstalkPixelCyclePresets.TryGetLegacyOrder(migrated.PixelCycle, out RgbPixelOrder detected) ||
                detected != order)
            {
                throw new InvalidOperationException($"新版周期未能识别旧 {order} 排列预设。");
            }
        }

        VerifyCustomPixelCycle();
    }

    /// <summary>验证可变周期、逐像素多通道/全灭选择、相位和斜向步进。</summary>
    private static void VerifyCustomPixelCycle()
    {
        PatternSettings settings = CreateSmallPhaseSettings(RgbPixelOrder.BGR);
        settings.CanvasWidth = 5;
        settings.CanvasHeight = 2;
        settings.PatternWidth = 5;
        settings.PatternHeight = 2;
        settings.PixelCycle = new CrosstalkPixelCycle
        {
            Pixels =
            [
                RgbChannelMask.None,
                RgbChannelMask.Red,
                RgbChannelMask.Green,
                RgbChannelMask.Blue,
                RgbChannelMask.All
            ],
            ColumnAdvance = 1,
            RowAdvance = 0
        };

        using (Mat horizontal = PatternGenerator.Generate(settings))
        {
            ForEachPixel(horizontal, (x, y, blue, green, red) =>
            {
                AssertBinaryChannel(blue, PatternType.PhaseStripes, x, y, "B");
                AssertBinaryChannel(green, PatternType.PhaseStripes, x, y, "G");
                AssertBinaryChannel(red, PatternType.PhaseStripes, x, y, "R");
            });
            AssertBgr(horizontal, 0, 0, 0, 0, 0, "自定义周期全灭像素");
            AssertBgr(horizontal, 1, 0, 0, 0, 255, "自定义周期红像素");
            AssertBgr(horizontal, 2, 0, 0, 255, 0, "自定义周期绿像素");
            AssertBgr(horizontal, 3, 0, 255, 0, 0, "自定义周期蓝像素");
            AssertBgr(horizontal, 4, 0, 255, 255, 255, "自定义周期三通道像素");
        }

        // 相位采用与旧版相同的 1 基编号；相位 2 使左上角读取周期的第 2 个位置。
        settings.Phase = 2;
        using (Mat shifted = PatternGenerator.Generate(settings))
        {
            AssertBgr(shifted, 0, 0, 0, 0, 255, "自定义周期相位偏移");
        }

        settings.Phase = 1;
        settings.CanvasWidth = 4;
        settings.PatternWidth = 4;
        settings.PixelCycle = new CrosstalkPixelCycle
        {
            Pixels =
            [
                RgbChannelMask.Red,
                RgbChannelMask.Green,
                RgbChannelMask.Blue,
                RgbChannelMask.None
            ],
            ColumnAdvance = -1,
            RowAdvance = 1
        };
        using (Mat diagonal = PatternGenerator.Generate(settings))
        {
            // (x=1,y=0) 的位置为 -1 mod 4 = 3（全灭），下一行则回到索引 0（红）。
            AssertBgr(diagonal, 1, 0, 0, 0, 0, "自定义周期负向斜率");
            AssertBgr(diagonal, 1, 1, 0, 0, 255, "自定义周期纵向步进");
        }

        PatternSettings clone = settings.Clone();
        clone.PixelCycle!.Pixels[0] = RgbChannelMask.All;
        if (settings.PixelCycle.Pixels[0] != RgbChannelMask.Red)
        {
            throw new InvalidOperationException("PatternSettings.Clone 未深拷贝串扰像素周期。");
        }

        CrosstalkPixelCycle resized = settings.PixelCycle.Clone();
        resized.Resize(6);
        if (resized.PeriodLength != 6 ||
            resized.Pixels[4] != RgbChannelMask.None ||
            resized.Pixels[5] != RgbChannelMask.None)
        {
            throw new InvalidOperationException("串扰像素周期扩展时未以全灭像素补齐。");
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

    /// <summary>
    /// 验证默认角度仍等价于最初版整数行位移、非整数角度可稳定改变图案，
    /// 并锁定“恢复最初版默认参数”按钮使用的完整快照。
    /// </summary>
    private static void VerifyCrosstalkTiltAndReferenceDefaults()
    {
        CrosstalkPixelCycle angleDefault =
            CrosstalkPixelCyclePresets.CreateLegacy(RgbPixelOrder.RGB);
        CrosstalkPixelCycle legacyInteger = angleDefault.Clone();
        legacyInteger.TiltAngleDegrees = null;
        legacyInteger.RowAdvance = 1;

        if (angleDefault.ResolveTiltAngleDegrees() != CrosstalkPixelCycle.DefaultTiltAngleDegrees ||
            angleDefault.CalculateEffectiveRowAdvance() != 1.0d ||
            legacyInteger.CalculateEffectiveRowAdvance() != 1.0d)
        {
            throw new InvalidOperationException("默认 18.435° 未保持最初版每行 1 个周期位置的位移。");
        }

        CrosstalkPixelCycle angleBoundary = angleDefault.Clone();
        angleBoundary.SetTiltAngleDegrees(89.0d);
        angleBoundary.SetTiltAngleDegrees(-89.0d);
        bool rejectedOutsideAngleRange = false;
        try
        {
            angleBoundary.SetTiltAngleDegrees(89.000001d);
        }
        catch (ArgumentOutOfRangeException)
        {
            rejectedOutsideAngleRange = true;
        }

        if (!rejectedOutsideAngleRange)
        {
            throw new InvalidOperationException("串扰倾斜角未限制在 -89° 到 89° 范围内。");
        }

        var legacyExtreme = new CrosstalkPixelCycle
        {
            Pixels = [RgbChannelMask.Red],
            RowAdvance = 4096,
            TiltAngleDegrees = null
        };
        if (legacyExtreme.ResolveTiltAngleDegrees() != CrosstalkPixelCycle.MaximumTiltAngleDegrees)
        {
            throw new InvalidOperationException("旧版超范围纵向步进未安全截断到 89°。");
        }

        for (int phase = 1; phase <= 8; phase++)
        {
            PatternSettings angleSettings = CreateSmallPhaseSettings(RgbPixelOrder.RGB);
            angleSettings.Phase = phase;
            angleSettings.PixelCycle = angleDefault.Clone();
            PatternSettings legacySettings = angleSettings.Clone();
            legacySettings.PixelCycle = legacyInteger.Clone();

            using Mat angleImage = PatternGenerator.Generate(angleSettings);
            using Mat legacyImage = PatternGenerator.Generate(legacySettings);
            AssertImagesEqual(legacyImage, angleImage, $"默认倾斜角相位 {phase}");
        }

        PatternSettings tiltedSettings = CreateSmallPhaseSettings(RgbPixelOrder.RGB);
        tiltedSettings.PixelCycle = angleDefault.Clone();
        tiltedSettings.PixelCycle.SetTiltAngleDegrees(10.125d);
        using Mat firstTilted = PatternGenerator.Generate(tiltedSettings);
        using Mat secondTilted = PatternGenerator.Generate(tiltedSettings.Clone());
        AssertImagesEqual(firstTilted, secondTilted, "非整数倾斜角确定性");

        PatternSettings defaultSettings = tiltedSettings.Clone();
        defaultSettings.PixelCycle = angleDefault.Clone();
        using Mat defaultImage = PatternGenerator.Generate(defaultSettings);
        if (Cv2.Norm(defaultImage, firstTilted, NormTypes.INF) == 0.0d)
        {
            throw new InvalidOperationException("非整数倾斜角没有改变串扰像素排列。");
        }

        ForEachPixel(firstTilted, (x, y, blue, green, red) =>
        {
            AssertBinaryChannel(blue, PatternType.PhaseStripes, x, y, "B");
            AssertBinaryChannel(green, PatternType.PhaseStripes, x, y, "G");
            AssertBinaryChannel(red, PatternType.PhaseStripes, x, y, "R");
        });

        // v1/v2 自定义配置缺少 TiltAngleDegrees 时，旧整数步进 3 应无损迁移为 45°。
        var legacyCustom = new CrosstalkPixelCycle
        {
            Pixels = [RgbChannelMask.Red, RgbChannelMask.Blue],
            ColumnAdvance = -2,
            RowAdvance = 3,
            TiltAngleDegrees = null
        };
        if (Math.Abs(legacyCustom.ResolveTiltAngleDegrees() - 45.0d) > 0.000000001d ||
            legacyCustom.CalculateEffectiveRowAdvance() != 3.0d ||
            legacyCustom.Clone().TiltAngleDegrees is not null)
        {
            throw new InvalidOperationException("旧串扰纵向步进未按 atan(RowAdvance / 3) 兼容迁移。");
        }

        PhaseStripePreferences defaults = PhaseStripePreferences.CreateReferenceDefault();
        PatternSettings restored = defaults.Settings;
        CrosstalkPixelCycle restoredCycle = CrosstalkPixelCyclePresets.Resolve(restored);
        BorderOverlaySettings border = restored.BorderOverlay;
        if (restored.CanvasWidth != 1920 || restored.CanvasHeight != 1080 ||
            restored.PatternX != 71 || restored.PatternY != 226 ||
            restored.PatternWidth != 1777 || restored.PatternHeight != 627 ||
            restored.Phase != 1 ||
            !CrosstalkPixelCyclePresets.TryGetLegacyOrder(restoredCycle, out RgbPixelOrder order) ||
            order != RgbPixelOrder.RGB ||
            restoredCycle.PeriodLength != 8 ||
            restoredCycle.ResolveTiltAngleDegrees() != CrosstalkPixelCycle.DefaultTiltAngleDegrees ||
            border.Enabled || border.X != 71 || border.Y != 226 ||
            border.Width != 1777 || border.Height != 627 || border.LineWidth != 5 ||
            defaults.OutputFormat != ImageFormatKind.Png || defaults.Quality != 95 ||
            !defaults.PreviewOverlay.ShowCenterCrosshair ||
            !defaults.PreviewOverlay.ShowPixelCoordinates)
        {
            throw new InvalidOperationException("串扰页面恢复按钮使用的最初版默认参数不完整。");
        }
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

    private static void AssertImagesEqual(Mat expected, Mat actual, string description)
    {
        AssertImageShape(actual, expected.Cols, expected.Rows, description);
        double maximumDifference = Cv2.Norm(expected, actual, NormTypes.INF);
        if (maximumDifference != 0d)
        {
            throw new InvalidOperationException(
                $"{description} 与旧版输出不一致，最大通道差异为 {maximumDifference}。");
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
            throw new InvalidOperationException($"离散光源文件名为 {fileName}，期望 5.00_8_4_0WB.bmp。");
        }

        using Mat image = DiscreteCrosstalkGenerator.Generate(settings);
        AssertImageShape(image, 4, 2, "离散光源 4×2 样例");
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
            throw new InvalidOperationException("离散光源 4×2 样例的 LightTools MESH 文本与参考结果不一致。");
        }

        settings.Group = -1;
        IReadOnlyList<int> groups = DiscreteCrosstalkGenerator.ResolveGroups(settings);
        if (groups.Count != 8 || !groups.SequenceEqual(Enumerable.Range(0, 8)))
        {
            throw new InvalidOperationException("离散光源 Group=-1 未解析为 0 到 7 共八组。");
        }
    }

    /// <summary>
    /// 使用独立临时文件验证用户输入可以跨实例保存，且 Load 返回深副本。
    /// </summary>
    private static void VerifyLegacySettingsMigration()
    {
        string uniqueFileName = $"EolTestPatternGenerator_Migration_{Guid.NewGuid():N}.json";
        string temporaryRoot = Path.GetFullPath(Path.GetTempPath());
        string settingsPath = Path.GetFullPath(Path.Combine(temporaryRoot, uniqueFileName));
        if (!string.Equals(
                Path.GetDirectoryName(settingsPath),
                temporaryRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(Path.GetFileName(settingsPath), uniqueFileName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("无法确认 v1 配置迁移自检临时文件的安全范围。");
        }

        const string version1Json = """
        {
          "Version": 1,
          "Main": {
            "PatternProfiles": {
              "NinePointGrid": {
                "PatternType": "NinePointGrid",
                "CanvasWidth": 1920,
                "CanvasHeight": 1080,
                "PatternX": 76,
                "PatternY": 201,
                "PatternWidth": 1768,
                "PatternHeight": 678,
                "DotRadius": 4,
                "Rows": 3,
                "Columns": 3,
                "BorderOverlay": {
                  "Enabled": true,
                  "X": -2,
                  "Y": 3,
                  "Width": 100,
                  "Height": 50,
                  "LineWidth": 2
                }
              },
              "DistortionGrid": {
                "PatternType": "DistortionGrid",
                "CanvasWidth": 20,
                "CanvasHeight": 10,
                "PatternX": 4,
                "PatternY": 3,
                "PatternWidth": 12,
                "PatternHeight": 4,
                "DotRadius": 1,
                "Rows": 1,
                "Columns": 1
              }
            }
          },
          "PhaseStripe": {
            "Settings": {
              "PatternType": "PhaseStripes",
              "CanvasWidth": 1920,
              "CanvasHeight": 1080,
              "PatternX": 71,
              "PatternY": 226,
              "PatternWidth": 1777,
              "PatternHeight": 627
            }
          },
          "ScreenOne": {
            "Settings": {
              "CanvasWidth": 3200,
              "CanvasHeight": 2000,
              "LeftX": 50,
              "LeftY": 50,
              "LeftWidth": 1500,
              "LeftHeight": 1900,
              "RightX": 1650,
              "RightY": 50,
              "RightWidth": 1500,
              "RightHeight": 1900
            }
          }
        }
        """;

        try
        {
            File.WriteAllText(settingsPath, version1Json);
            var store = new UserSettingsStore(settingsPath);
            ApplicationPreferences migrated = store.Load();
            if (store.LastLoadError is not null ||
                migrated.Version != ApplicationPreferences.CurrentVersion ||
                migrated.Workspace.SelectedNavigationIndex != 0 ||
                migrated.Main.LastImportedImageDirectory.Length != 0 ||
                migrated.Main.LastExportDirectory.Length != 0 ||
                migrated.PhaseStripe.LastExportDirectory.Length != 0 ||
                migrated.ScreenOne.LastExportDirectory.Length != 0)
            {
                throw new InvalidOperationException("v1 配置未能无错误迁移并补齐新增持久化默认值。", store.LastLoadError);
            }

            PatternSettings grid = migrated.Main.PatternProfiles[PatternType.NinePointGrid];
            PatternSettings migratedSingleDot = migrated.Main.PatternProfiles[PatternType.DistortionGrid];
            CrosstalkPixelCycle migratedPhaseCycle =
                CrosstalkPixelCyclePresets.Resolve(migrated.PhaseStripe.Settings);
            RegionMargins gridMargins = grid.GetMargins();
            RegionMargins borderMargins = grid.BorderOverlay.GetMargins();
            ScreenOneSettings screen = migrated.ScreenOne.Settings;
            if (gridMargins.Left != 72 || gridMargins.Top != 197 ||
                gridMargins.Right != 71 || gridMargins.Bottom != 196 ||
                grid.PatternX != 76 || grid.PatternY != 201 ||
                grid.PatternWidth != 1768 || grid.PatternHeight != 678 ||
                borderMargins.Left != -2 || borderMargins.Top != 3 ||
                borderMargins.Right != 1822 || borderMargins.Bottom != 1027 ||
                migratedPhaseCycle.ResolveTiltAngleDegrees() != CrosstalkPixelCycle.DefaultTiltAngleDegrees ||
                migratedPhaseCycle.CalculateEffectiveRowAdvance() != 1.0d ||
                screen.LeftRegionLeftMargin != 50 || screen.LeftRegionRightMargin != 1650 ||
                screen.RightRegionLeftMargin != 1650 || screen.RightRegionRightMargin != 50)
            {
                throw new InvalidOperationException("v1 坐标尺寸未无损换算为 v2 四边距。");
            }

            RegionMargins migratedSingleDotMargins = migratedSingleDot.GetMargins();
            if (migratedSingleDot.PatternX != 4 || migratedSingleDot.PatternY != 3 ||
                migratedSingleDot.PatternWidth != 0 || migratedSingleDot.PatternHeight != 0 ||
                migratedSingleDotMargins.Left != 3 || migratedSingleDotMargins.Top != 2 ||
                migratedSingleDotMargins.Right != 14 || migratedSingleDotMargins.Bottom != 5)
            {
                throw new InvalidOperationException(
                    "v1 单行单列点阵未在保持首圆心像素的同时迁移为真实外缘。");
            }

            using (Mat migratedSingleDotImage = PatternGenerator.Generate(migratedSingleDot))
            {
                AssertBgr(migratedSingleDotImage, 4, 3, 255, 255, 255, "v1 单行单列点阵圆心");
                AssertBgr(migratedSingleDotImage, 3, 3, 255, 255, 255, "v1 单行单列点阵外缘");
                AssertBgr(migratedSingleDotImage, 16, 3, 0, 0, 0, "v1 已忽略跨度不应生成额外圆点");
            }

            PatternSettings gridWithoutOverlay = grid.Clone();
            gridWithoutOverlay.BorderOverlay.Enabled = false;
            using Mat migratedImage = PatternGenerator.Generate(gridWithoutOverlay);
            using Mat presetImage = PatternGenerator.Generate(PatternPresets.Create(PatternType.NinePointGrid));
            AssertImagesEqual(presetImage, migratedImage, "v1 点阵迁移像素");

            store.Save(migrated);
            string version2Json = File.ReadAllText(settingsPath);
            if (!version2Json.Contains("\"Version\": 2", StringComparison.Ordinal) ||
                !version2Json.Contains("\"LeftMargin\"", StringComparison.Ordinal) ||
                version2Json.Contains("\"PatternX\"", StringComparison.Ordinal) ||
                version2Json.Contains("\"LeftX\"", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("迁移后的配置没有以纯 v2 四边距格式保存。");
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
                preferences.Workspace.SelectedNavigationIndex = 6;
                preferences.Workspace.SelectedNavigationPageId = WorkspaceNavigationPages.ImageToVideo;
                preferences.Workspace.ProjectionTopology =
                    EolTestPatternGenerator.Projection.DisplayTopology.Extend;
                preferences.Workspace.RestoreWallpaperOnStop = false;
                preferences.Main.Quality = 42;
                preferences.Main.LastImportedImageDirectory = @"C:\图卡\输入";
                preferences.Main.LastExportDirectory = @"D:\图卡\主界面输出";
                preferences.PhaseStripe.LastExportDirectory = @"D:\图卡\串扰输出";
                preferences.ScreenOne.LastExportDirectory = @"D:\图卡\显示器输出";
                preferences.PhaseStripe.Settings.PixelCycle = new CrosstalkPixelCycle
                {
                    Pixels = [RgbChannelMask.Red, RgbChannelMask.Green | RgbChannelMask.Blue],
                    ColumnAdvance = -2,
                    RowAdvance = 3,
                    TiltAngleDegrees = 12.345678d
                };
                preferences.NonIntegerBlend.Matlab.WriteMesh = false;
                preferences.NonIntegerBlend.Discrete.OutputFormat = ImageFormatKind.Bmp;
                preferences.NonIntegerBlend.Converter.SourceImagePath = @"C:\测试\输入图.png";
            });

            var reader = new UserSettingsStore(settingsPath);
            ApplicationPreferences loaded = reader.Load();
            if (loaded.Workspace.SelectedNavigationIndex != 6 ||
                loaded.Workspace.SelectedNavigationPageId != WorkspaceNavigationPages.ImageToVideo ||
                loaded.Workspace.ProjectionTopology !=
                    EolTestPatternGenerator.Projection.DisplayTopology.Extend ||
                loaded.Workspace.RestoreWallpaperOnStop ||
                loaded.Main.Quality != 42 ||
                !string.Equals(loaded.Main.LastImportedImageDirectory, @"C:\图卡\输入", StringComparison.Ordinal) ||
                !string.Equals(loaded.Main.LastExportDirectory, @"D:\图卡\主界面输出", StringComparison.Ordinal) ||
                !string.Equals(loaded.PhaseStripe.LastExportDirectory, @"D:\图卡\串扰输出", StringComparison.Ordinal) ||
                !string.Equals(loaded.ScreenOne.LastExportDirectory, @"D:\图卡\显示器输出", StringComparison.Ordinal) ||
                loaded.PhaseStripe.Settings.PixelCycle is not
                {
                    PeriodLength: 2,
                    ColumnAdvance: -2,
                    RowAdvance: 3
                } loadedCycle ||
                loadedCycle.TiltAngleDegrees != 12.345678d ||
                loadedCycle.Pixels[0] != RgbChannelMask.Red ||
                loadedCycle.Pixels[1] != (RgbChannelMask.Green | RgbChannelMask.Blue) ||
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
            loaded.Workspace.SelectedNavigationIndex = 1;
            loaded.Workspace.ProjectionTopology =
                EolTestPatternGenerator.Projection.DisplayTopology.Internal;
            loaded.Main.LastExportDirectory = @"E:\被修改";
            loaded.PhaseStripe.Settings.PixelCycle!.Pixels[0] = RgbChannelMask.All;
            ApplicationPreferences unchanged = reader.Load();
            if (unchanged.Main.Quality != 42 ||
                unchanged.Workspace.SelectedNavigationIndex != 6 ||
                unchanged.Workspace.ProjectionTopology !=
                    EolTestPatternGenerator.Projection.DisplayTopology.Extend ||
                !string.Equals(unchanged.Main.LastExportDirectory, @"D:\图卡\主界面输出", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("设置仓库 Load 返回了共享的可变对象，而不是深副本。");
            }

            if (reader.Load().PhaseStripe.Settings.PixelCycle!.Pixels[0] != RgbChannelMask.Red)
            {
                throw new InvalidOperationException("设置仓库 Load 未深拷贝串扰像素周期。");
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

    /// <summary>
    /// 验证损坏配置和未来版本配置不会被默认值静默覆盖：
    /// 首次保存会先保留唯一原文备份，备份失败时则拒绝写入。
    /// </summary>
    private static void VerifySettingsRecoveryBackup()
    {
        VerifyRecoveryCase(
            "{ 这不是有效 JSON",
            typeof(System.Text.Json.JsonException),
            verifyLockedBackupFailure: true,
            "损坏配置");
        VerifyRecoveryCase(
            """
            {
              "Version": 999,
              "Sentinel": "future-version-original"
            }
            """,
            typeof(NotSupportedException),
            verifyLockedBackupFailure: false,
            "未来版本配置");

        static void VerifyRecoveryCase(
            string originalText,
            Type expectedLoadErrorType,
            bool verifyLockedBackupFailure,
            string description)
        {
            string uniqueFileName = $"EolTestPatternGenerator_Recovery_{Guid.NewGuid():N}.json";
            string temporaryRoot = Path.GetFullPath(Path.GetTempPath())
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string settingsPath = Path.GetFullPath(Path.Combine(temporaryRoot, uniqueFileName));
            if (!string.Equals(
                    Path.GetDirectoryName(settingsPath),
                    temporaryRoot,
                    StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(Path.GetFileName(settingsPath), uniqueFileName, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("无法确认恢复备份自检临时文件的安全范围。");
            }

            string backupPattern = uniqueFileName + ".recovery-*";
            try
            {
                File.WriteAllText(settingsPath, originalText);
                byte[] originalBytes = File.ReadAllBytes(settingsPath);
                var store = new UserSettingsStore(settingsPath);
                _ = store.Load();
                Exception? loadError = store.LastLoadError;
                if (loadError is null ||
                    !expectedLoadErrorType.IsInstanceOfType(loadError))
                {
                    throw new InvalidOperationException($"{description}未记录预期的加载错误。", loadError);
                }

                if (verifyLockedBackupFailure)
                {
                    Exception? saveError;
                    bool saved;
                    using (var lockedSource = new FileStream(
                               settingsPath,
                               FileMode.Open,
                               FileAccess.ReadWrite,
                               FileShare.None))
                    {
                        saved = store.TryUpdateAndSave(
                            preferences => preferences.Main.Quality = 40,
                            out saveError);
                    }

                    if (saved || saveError is not IOException ||
                        !File.ReadAllBytes(settingsPath).SequenceEqual(originalBytes) ||
                        Directory.EnumerateFiles(temporaryRoot, backupPattern).Any())
                    {
                        throw new InvalidOperationException(
                            "恢复备份失败时未拒绝覆盖原配置，或留下了不完整备份。",
                            saveError);
                    }
                }

                store.UpdateAndSave(preferences => preferences.Main.Quality = 41);
                string backupPath = store.LastRecoveryBackupPath
                    ?? throw new InvalidOperationException($"{description}首次保存前未创建恢复备份。");
                string[] firstBackups = Directory.EnumerateFiles(temporaryRoot, backupPattern).ToArray();
                if (!string.Equals(
                        Path.GetDirectoryName(backupPath),
                        temporaryRoot,
                        StringComparison.OrdinalIgnoreCase) ||
                    firstBackups.Length != 1 ||
                    !string.Equals(firstBackups[0], backupPath, StringComparison.OrdinalIgnoreCase) ||
                    !File.Exists(backupPath) ||
                    !File.ReadAllBytes(backupPath).SequenceEqual(originalBytes) ||
                    store.LastLoadError is null)
                {
                    throw new InvalidOperationException($"{description}的唯一恢复备份或错误信息不完整。");
                }

                // 同一次恢复之后的普通保存不应重复备份。
                store.UpdateAndSave(preferences => preferences.Main.Quality = 42);
                string[] finalBackups = Directory.EnumerateFiles(temporaryRoot, backupPattern).ToArray();
                ApplicationPreferences reloaded = store.Reload();
                if (finalBackups.Length != 1 ||
                    !string.Equals(store.LastRecoveryBackupPath, backupPath, StringComparison.OrdinalIgnoreCase) ||
                    !File.ReadAllBytes(backupPath).SequenceEqual(originalBytes) ||
                    reloaded.Main.Quality != 42 ||
                    store.LastLoadError is not null)
                {
                    throw new InvalidOperationException($"{description}恢复备份后的保存或重载不正确。");
                }
            }
            finally
            {
                if (File.Exists(settingsPath))
                {
                    File.Delete(settingsPath);
                }

                foreach (string backupPath in Directory.EnumerateFiles(temporaryRoot, backupPattern))
                {
                    File.Delete(backupPath);
                }
            }
        }
    }

    private static void VerifyDialogDirectoryResolution()
    {
        string temporaryRoot = Path.GetFullPath(Path.GetTempPath())
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        string nonexistentDirectory = Path.Combine(
            temporaryRoot,
            $"EolTestPatternGenerator_Missing_{Guid.NewGuid():N}");
        string fallbackFile = Path.Combine(temporaryRoot, "尚未创建的图片.png");

        string existing = DialogDirectoryResolver.ResolveExistingDirectory(temporaryRoot);
        string missing = DialogDirectoryResolver.ResolveExistingDirectory(nonexistentDirectory);
        string fallback = DialogDirectoryResolver.ResolveExistingDirectory(nonexistentDirectory, fallbackFile);
        string remembered = DialogDirectoryResolver.RememberFileDirectory(fallbackFile);
        string rememberedDirectory = DialogDirectoryResolver.RememberDirectory(
            nonexistentDirectory,
            temporaryRoot);
        string malformed = DialogDirectoryResolver.ResolveExistingDirectory("\0无效路径");

        if (!string.Equals(existing, temporaryRoot, StringComparison.OrdinalIgnoreCase) ||
            missing.Length != 0 ||
            !string.Equals(fallback, temporaryRoot, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(remembered, temporaryRoot, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(rememberedDirectory, temporaryRoot, StringComparison.OrdinalIgnoreCase) ||
            malformed.Length != 0)
        {
            throw new InvalidOperationException("文件对话框初始目录的存在性检查或安全回退不正确。");
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
