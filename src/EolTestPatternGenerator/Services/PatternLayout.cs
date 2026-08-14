using EolTestPatternGenerator.Models;

namespace EolTestPatternGenerator.Services;

public static class PatternLayout
{
    public static void Center(PatternSettings settings)
    {
        if (settings.PatternType is PatternType.Black or PatternType.FullWhite or PatternType.FullRed or
            PatternType.FullGreen or PatternType.FullBlue or PatternType.ImportedImage)
        {
            settings.NormalizeFullCanvasMargins();
            return;
        }

        PixelRegion region = settings.GetOuterRegion();
        // 点阵沿用旧版向上取整规则；矩形沿用旧版向下取整规则，确保参考预设逐像素不变。
        double horizontalGap = (settings.CanvasWidth - (long)region.Width) / 2.0;
        double verticalGap = (settings.CanvasHeight - (long)region.Height) / 2.0;
        int left = settings.IsDotGrid
            ? (int)Math.Ceiling(horizontalGap)
            : (int)Math.Floor(horizontalGap);
        int top = settings.IsDotGrid
            ? (int)Math.Ceiling(verticalGap)
            : (int)Math.Floor(verticalGap);
        long right = (long)settings.CanvasWidth - left - region.Width;
        long bottom = (long)settings.CanvasHeight - top - region.Height;
        if (right is < int.MinValue or > int.MaxValue || bottom is < int.MinValue or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "居中后的四边距超出支持范围。");
        }

        settings.LeftMargin = left;
        settings.TopMargin = top;
        settings.RightMargin = (int)right;
        settings.BottomMargin = (int)bottom;
    }
}
