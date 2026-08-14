using EolTestPatternGenerator.Models;

namespace EolTestPatternGenerator.Services;

/// <summary>
/// 导出主窗口提供的十张基础图卡；相移、分屏和导入图片由各自界面负责。
/// </summary>
public static class MainPatternBatchExporter
{
    // 相移8张和1号屏三张各自有独立参数窗口，这里只保留主窗口的固定批次。
    private static readonly PatternType[] ExportOrder =
    {
        PatternType.Border,
        PatternType.NinePointGrid,
        PatternType.DistortionGrid,
        PatternType.CorrectionCross,
        PatternType.WhiteRectangle,
        PatternType.Black,
        PatternType.FullWhite,
        PatternType.FullRed,
        PatternType.FullGreen,
        PatternType.FullBlue
    };

    public static IReadOnlyList<string> ExportAll(
        string outputDirectory,
        int canvasWidth,
        int canvasHeight,
        int dotRadius,
        int lineWidth,
        ImageExportOptions exportOptions,
        IReadOnlyDictionary<PatternType, PatternSettings>? profiles = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);
        ArgumentNullException.ThrowIfNull(exportOptions);

        // 批次包含精确纯色图，拒绝会引入中间通道值的有损编码。
        if (exportOptions.Format is not (ImageFormatKind.Png or ImageFormatKind.Bmp or ImageFormatKind.Tiff))
        {
            throw new ArgumentException(
                "主图卡包含必须保持精确 0/255 像素的纯色图，只允许导出 PNG、BMP 或 TIFF 无损格式。",
                nameof(exportOptions));
        }

        Directory.CreateDirectory(outputDirectory);
        var exportedPaths = new List<string>(ExportOrder.Length);

        foreach (PatternType type in ExportOrder)
        {
            PatternSettings settings = profiles?.TryGetValue(type, out PatternSettings? profile) == true
                ? profile!.Clone()
                : PatternPresets.Create(type);

            settings.PatternType = type;
            settings.CanvasWidth = canvasWidth;
            settings.CanvasHeight = canvasHeight;

            if (type is PatternType.NinePointGrid or PatternType.DistortionGrid)
            {
                settings.DotRadius = dotRadius;
            }

            if (type is PatternType.Border or PatternType.CorrectionCross)
            {
                settings.LineWidth = lineWidth;
            }

            using var image = PatternGenerator.Generate(settings);
            string outputPath = Path.Combine(
                outputDirectory,
                PatternFileNames.Get(settings, exportOptions.Format));
            exportedPaths.Add(ImageFileWriter.Write(outputPath, image, exportOptions));
        }

        return exportedPaths;
    }
}
