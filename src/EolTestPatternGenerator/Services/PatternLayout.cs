using EolTestPatternGenerator.Models;

namespace EolTestPatternGenerator.Services;

public static class PatternLayout
{
    public static void Center(PatternSettings settings)
    {
        if (settings.PatternType is PatternType.NinePointGrid or PatternType.DistortionGrid)
        {
            // 点阵以圆心为锚点。对偶数画布 + 奇数图案使用向上取整，
            // 使中心圆点落在样图的 (960, 540)。
            int actualWidth = settings.PatternWidth + (2 * settings.DotRadius) + 1;
            int actualHeight = settings.PatternHeight + (2 * settings.DotRadius) + 1;
            settings.PatternX = (int)Math.Ceiling((settings.CanvasWidth - actualWidth) / 2.0) + settings.DotRadius;
            settings.PatternY = (int)Math.Ceiling((settings.CanvasHeight - actualHeight) / 2.0) + settings.DotRadius;
            return;
        }

        if (settings.PatternType is PatternType.Black or PatternType.FullWhite or PatternType.FullRed or
            PatternType.FullGreen or PatternType.FullBlue)
        {
            settings.PatternX = 0;
            settings.PatternY = 0;
            settings.PatternWidth = settings.CanvasWidth;
            settings.PatternHeight = settings.CanvasHeight;
            return;
        }

        settings.PatternX = (int)Math.Floor((settings.CanvasWidth - settings.PatternWidth) / 2.0);
        settings.PatternY = (int)Math.Floor((settings.CanvasHeight - settings.PatternHeight) / 2.0);
    }
}
