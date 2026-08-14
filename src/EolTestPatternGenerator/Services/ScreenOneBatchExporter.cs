using EolTestPatternGenerator.Models;

namespace EolTestPatternGenerator.Services;

public static class ScreenOneBatchExporter
{
    /// <summary>
    /// 使用参考图默认参数导出三张图卡。保留此重载以兼容原有调用。
    /// </summary>
    public static IReadOnlyList<string> ExportReferenceThree(
        string outputDirectory,
        ImageExportOptions exportOptions)
    {
        return ExportReferenceThree(
            outputDirectory,
            ScreenOneSettings.CreateReferenceDefault(),
            exportOptions);
    }

    /// <summary>
    /// 使用窗口中可编辑的画布和左右区域参数导出三张图卡。
    /// </summary>
    public static IReadOnlyList<string> ExportReferenceThree(
        string outputDirectory,
        ScreenOneSettings settings,
        ImageExportOptions exportOptions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(exportOptions);

        if (exportOptions.Format is ImageFormatKind.Jpeg or ImageFormatKind.WebP)
        {
            throw new InvalidOperationException("显示器参考图必须使用 PNG、BMP 或 TIFF，才能保证 RGB 通道只有 0/255。");
        }

        Directory.CreateDirectory(outputDirectory);
        var results = new List<string>(3);
        Export(ScreenOneCardKind.BlackLeftWhiteRight);
        Export(ScreenOneCardKind.WhiteLeftBlackRight);
        Export(ScreenOneCardKind.BlackBoth);
        return results;

        void Export(ScreenOneCardKind cardKind)
        {
            using var image = ScreenOnePatternGenerator.Generate(settings, cardKind);
            string path = Path.Combine(
                outputDirectory,
                ScreenOnePatternGenerator.GetFileBaseName(cardKind) +
                ImageFileWriter.GetExtension(exportOptions.Format));
            results.Add(ImageFileWriter.Write(path, image, exportOptions));
        }
    }

    public static IReadOnlyList<string> ExportAll(
        string outputDirectory,
        bool threeDimensional,
        ImageExportOptions exportOptions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);
        ArgumentNullException.ThrowIfNull(exportOptions);

        if (exportOptions.Format is ImageFormatKind.Jpeg or ImageFormatKind.WebP)
        {
            throw new InvalidOperationException("显示器图卡必须使用 PNG、BMP 或 TIFF，才能保证 RGB 通道只有 0/255。");
        }

        Directory.CreateDirectory(outputDirectory);
        var results = new List<string>(8);
        int width = threeDimensional ? 6400 : 3200;
        int height = 2000;

        if (threeDimensional)
        {
            // 参考目录中的 3D 1.B_W 与 2.W_B 实际像素完全相同。
            ExportSplit("1.B_W", ScreenSplitMode.ThreeDimensionalBlackRight);
            ExportSplit("2.W_B", ScreenSplitMode.ThreeDimensionalBlackRight);
            ExportSplit("3.B", ScreenSplitMode.ThreeDimensionalBlackLeft);
        }
        else
        {
            ExportSplit("1.B_W", ScreenSplitMode.TwoDimensionalBlackLeft);
            ExportSplit("2.W_B", ScreenSplitMode.TwoDimensionalBlackRight);
            ExportSplit("3.B", ScreenSplitMode.TwoDimensionalBlackBoth);
        }

        ExportSolid("4.W", PatternType.FullWhite);
        ExportSolid("5.B", PatternType.Black);
        ExportSolid("6.R", PatternType.FullRed);
        ExportSolid("7.G", PatternType.FullGreen);
        ExportSolid("8.B", PatternType.FullBlue);
        return results;

        void ExportSplit(string fileBaseName, ScreenSplitMode mode)
        {
            PatternSettings settings = PatternPresets.Create(PatternType.ScreenSplit);
            settings.CanvasWidth = width;
            settings.CanvasHeight = height;
            settings.ScreenSplitMode = mode;
            Export(fileBaseName, settings);
        }

        void ExportSolid(string fileBaseName, PatternType type)
        {
            PatternSettings settings = PatternPresets.Create(type);
            settings.CanvasWidth = width;
            settings.CanvasHeight = height;
            settings.PatternWidth = width;
            settings.PatternHeight = height;
            Export(fileBaseName, settings);
        }

        void Export(string fileBaseName, PatternSettings settings)
        {
            using var image = PatternGenerator.Generate(settings);
            string path = Path.Combine(outputDirectory, fileBaseName + ImageFileWriter.GetExtension(exportOptions.Format));
            results.Add(ImageFileWriter.Write(path, image, exportOptions));
        }
    }
}
