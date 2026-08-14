namespace EolTestPatternGenerator.Models;

/// <summary>
/// 图案最外缘到画布四条边的有符号像素距离。
/// 负值表示图案越过对应画布边缘，绘制阶段会裁剪到画布范围。
/// </summary>
public sealed class RegionMargins
{
    public RegionMargins()
    {
    }

    public RegionMargins(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public int Left { get; set; }

    public int Top { get; set; }

    public int Right { get; set; }

    public int Bottom { get; set; }

    public long CalculateWidth(int canvasWidth)
    {
        return Services.MarginGeometry.CalculateWidth(canvasWidth, Left, Right);
    }

    public long CalculateHeight(int canvasHeight)
    {
        return Services.MarginGeometry.CalculateHeight(canvasHeight, Top, Bottom);
    }

    public PixelRegion ComputeBounds(int canvasWidth, int canvasHeight, string description = "图案")
    {
        return Services.MarginGeometry.Resolve(canvasWidth, canvasHeight, this, description);
    }

    public RegionMargins Clone()
    {
        return new RegionMargins(Left, Top, Right, Bottom);
    }
}

/// <summary>
/// 由画布和四边距解析出的半开像素区域：右、下边界分别为 X + Width、Y + Height。
/// </summary>
public readonly record struct PixelRegion(
    int X,
    int Y,
    int Width,
    int Height)
{
    public long RightExclusive => (long)X + Width;

    public long BottomExclusive => (long)Y + Height;
}
