using EolTestPatternGenerator.Models;

namespace EolTestPatternGenerator.Services;

public static class PatternFileNames
{
    public static string Get(PatternSettings settings)
    {
        return Get(settings, ImageFormatKind.Png);
    }

    public static string Get(PatternSettings settings, ImageFormatKind format)
    {
        return GetBaseName(settings) + ImageFileWriter.GetExtension(format);
    }

    private static string GetBaseName(PatternSettings settings)
    {
        return settings.PatternType switch
        {
            PatternType.Border => "0",
            PatternType.PhaseStripes => $"{settings.Phase}",
            PatternType.NinePointGrid => "9point_9pix_1777x687in1920x1080",
            PatternType.DistortionGrid => "畸变",
            PatternType.CorrectionCross => "上下校正图",
            PatternType.WhiteRectangle => "白",
            PatternType.Black => "黑",
            PatternType.FullWhite => "全白",
            PatternType.FullRed => "全红",
            PatternType.FullGreen => "全绿",
            PatternType.FullBlue => "全蓝",
            PatternType.ScreenSplit => GetScreenSplitName(settings.ScreenSplitMode),
            PatternType.ImportedImage => GetImportedImageName(settings),
            _ => "图卡"
        };
    }

    private static string GetImportedImageName(PatternSettings settings)
    {
        string sourceName = string.IsNullOrWhiteSpace(settings.SourceImagePath)
            ? "导入图片"
            : Path.GetFileNameWithoutExtension(settings.SourceImagePath.Trim());

        if (string.IsNullOrWhiteSpace(sourceName))
        {
            sourceName = "导入图片";
        }

        return settings.BorderOverlay.Enabled
            ? $"{sourceName}_白框"
            : $"{sourceName}_导入";
    }

    private static string GetScreenSplitName(ScreenSplitMode mode)
    {
        return mode switch
        {
            ScreenSplitMode.TwoDimensionalBlackLeft => "1.B_W",
            ScreenSplitMode.TwoDimensionalBlackRight => "2.W_B",
            ScreenSplitMode.TwoDimensionalBlackBoth => "3.B",
            ScreenSplitMode.ThreeDimensionalBlackRight => "1.B_W",
            ScreenSplitMode.ThreeDimensionalBlackLeft => "3.B",
            _ => "分屏图卡"
        };
    }
}
