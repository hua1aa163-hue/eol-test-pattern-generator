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
            _ => "图卡"
        };
    }
}
