using EolTestPatternGenerator.Models;

namespace EolTestPatternGenerator.Services;

public static class PatternPresets
{
    public static PatternSettings Create(PatternType type)
    {
        var settings = new PatternSettings
        {
            PatternType = type,
            CanvasWidth = 1920,
            CanvasHeight = 1080,
            DotRadius = 4,
            LineWidth = 5,
            Phase = 1
        };

        switch (type)
        {
            case PatternType.Border:
            case PatternType.PhaseStripes:
            case PatternType.WhiteRectangle:
                settings.PatternX = 71;
                settings.PatternY = 226;
                settings.PatternWidth = 1777;
                settings.PatternHeight = 627;
                settings.Rows = 1;
                settings.Columns = 1;
                break;

            case PatternType.NinePointGrid:
                settings.PatternX = 76;
                settings.PatternY = 201;
                settings.PatternWidth = 1768;
                settings.PatternHeight = 678;
                settings.Rows = 3;
                settings.Columns = 3;
                break;

            case PatternType.DistortionGrid:
                settings.PatternX = 76;
                settings.PatternY = 231;
                settings.PatternWidth = 1768;
                settings.PatternHeight = 618;
                settings.Rows = 7;
                settings.Columns = 27;
                break;

            case PatternType.CorrectionCross:
                settings.PatternX = 860;
                settings.PatternY = 440;
                settings.PatternWidth = 200;
                settings.PatternHeight = 200;
                settings.Rows = 1;
                settings.Columns = 1;
                break;

            case PatternType.Black:
            case PatternType.FullWhite:
            case PatternType.FullRed:
            case PatternType.FullGreen:
            case PatternType.FullBlue:
            case PatternType.ImportedImage:
                settings.PatternX = 0;
                settings.PatternY = 0;
                settings.PatternWidth = 1920;
                settings.PatternHeight = 1080;
                settings.Rows = 1;
                settings.Columns = 1;
                break;

            case PatternType.ScreenSplit:
                settings.CanvasWidth = 3200;
                settings.CanvasHeight = 2000;
                settings.PatternX = 50;
                settings.PatternY = 50;
                settings.PatternWidth = 3100;
                settings.PatternHeight = 1900;
                settings.Rows = 1;
                settings.Columns = 2;
                settings.ScreenSplitMode = ScreenSplitMode.TwoDimensionalBlackLeft;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        return settings;
    }
}
