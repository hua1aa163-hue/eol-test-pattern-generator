using EolTestPatternGenerator.Models;

namespace EolTestPatternGenerator.Services;

/// <summary>
/// 导出“基础图卡”页面提供的十张固定批次图卡；串扰像素排列、显示器和导入图片由各自页面负责。
/// </summary>
public static class MainPatternBatchExporter
{
    // 串扰像素排列和显示器三张图各自有专用参数页面，这里只保留基础图卡的固定批次。
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
                "基础图卡批次包含必须保持精确 0/255 像素的纯色图，只允许导出 PNG、BMP 或 TIFF 无损格式。",
                nameof(exportOptions));
        }

        Directory.CreateDirectory(outputDirectory);
        var exportedPaths = new List<string>(ExportOrder.Length);

        foreach (PatternType type in ExportOrder)
        {
            PatternSettings settings = CreateExportSettings(
                type,
                canvasWidth,
                canvasHeight,
                dotRadius,
                lineWidth,
                profiles);

            using var image = PatternGenerator.Generate(settings);
            string outputPath = Path.Combine(
                outputDirectory,
                PatternFileNames.Get(settings, exportOptions.Format));
            exportedPaths.Add(ImageFileWriter.Write(outputPath, image, exportOptions));
        }

        return exportedPaths;
    }

    /// <summary>
    /// 有窗体保存的分类快照时，半径和线宽也必须来自对应图卡；
    /// 只有无快照的旧调用路径才使用方法参数作为回退。
    /// </summary>
    internal static PatternSettings CreateExportSettings(
        PatternType type,
        int canvasWidth,
        int canvasHeight,
        int fallbackDotRadius,
        int fallbackLineWidth,
        IReadOnlyDictionary<PatternType, PatternSettings>? profiles)
    {
        PatternSettings? profile = null;
        bool hasProfile = profiles is not null &&
            profiles.TryGetValue(type, out profile) &&
            profile is not null;
        PatternSettings settings = hasProfile
            ? profile!.Clone()
            : PatternPresets.Create(type);

        settings.PatternType = type;
        settings.CanvasWidth = canvasWidth;
        settings.CanvasHeight = canvasHeight;

        if (!hasProfile && type is (PatternType.NinePointGrid or PatternType.DistortionGrid))
        {
            settings.DotRadius = fallbackDotRadius;
        }

        if (!hasProfile && type is (PatternType.Border or PatternType.CorrectionCross))
        {
            settings.LineWidth = fallbackLineWidth;
        }

        return settings;
    }
}
