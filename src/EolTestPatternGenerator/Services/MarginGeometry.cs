using EolTestPatternGenerator.Models;

namespace EolTestPatternGenerator.Services;

/// <summary>
/// 在“旧坐标和尺寸”与“最外缘四边距”之间执行无损换算。
/// 所有中间值均使用 long，避免恶意或损坏配置触发 int 溢出。
/// </summary>
public static class MarginGeometry
{
    public static long CalculateWidth(int canvasWidth, int leftMargin, int rightMargin)
    {
        return (long)canvasWidth - leftMargin - rightMargin;
    }

    public static long CalculateHeight(int canvasHeight, int topMargin, int bottomMargin)
    {
        return (long)canvasHeight - topMargin - bottomMargin;
    }

    public static PixelRegion Resolve(
        int canvasWidth,
        int canvasHeight,
        RegionMargins margins,
        string description = "图案")
    {
        long rightExclusive = (long)canvasWidth - margins.Right;
        long bottomExclusive = (long)canvasHeight - margins.Bottom;
        long width = rightExclusive - margins.Left;
        long height = bottomExclusive - margins.Top;

        if (width < 1 || height < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(margins),
                $"{description}由四边距计算出的宽度和高度必须大于 0，当前为 {width} × {height}。 ");
        }

        // 绘制接口仍使用 int 坐标和尺寸，因此在这里统一拒绝无法精确表达的区域。
        if (rightExclusive is < int.MinValue or > int.MaxValue ||
            bottomExclusive is < int.MinValue or > int.MaxValue ||
            width > int.MaxValue ||
            height > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(margins),
                $"{description}的四边距计算结果超出支持的整数坐标范围。 ");
        }

        return new PixelRegion(margins.Left, margins.Top, (int)width, (int)height);
    }

    public static RegionMargins FromLegacyBounds(
        int canvasWidth,
        int canvasHeight,
        int x,
        int y,
        int width,
        int height,
        string description = "图案")
    {
        if (width < 1 || height < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(width),
                $"{description}的旧宽度和高度必须大于 0。 ");
        }

        long right = (long)canvasWidth - x - width;
        long bottom = (long)canvasHeight - y - height;
        if (right is < int.MinValue or > int.MaxValue ||
            bottom is < int.MinValue or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(width),
                $"{description}的旧坐标和尺寸无法转换为 int 四边距。 ");
        }

        return new RegionMargins(x, y, (int)right, (int)bottom);
    }
}
