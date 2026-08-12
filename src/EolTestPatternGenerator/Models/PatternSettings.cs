namespace EolTestPatternGenerator.Models;

public sealed class PatternSettings
{
    public PatternType PatternType { get; set; } = PatternType.Border;

    public int CanvasWidth { get; set; } = 1920;

    public int CanvasHeight { get; set; } = 1080;

    /// <summary>
    /// 矩形图卡为图案左上角 X；点阵图卡为第一个圆心 X。
    /// </summary>
    public int PatternX { get; set; } = 71;

    /// <summary>
    /// 矩形图卡为图案左上角 Y；点阵图卡为第一个圆心 Y。
    /// </summary>
    public int PatternY { get; set; } = 226;

    /// <summary>
    /// 矩形图卡为像素宽度；点阵图卡为首末圆心的 X 距离。
    /// </summary>
    public int PatternWidth { get; set; } = 1777;

    /// <summary>
    /// 矩形图卡为像素高度；点阵图卡为首末圆心的 Y 距离。
    /// </summary>
    public int PatternHeight { get; set; } = 627;

    public int Phase { get; set; } = 1;

    public int DotRadius { get; set; } = 4;

    public int Rows { get; set; } = 3;

    public int Columns { get; set; } = 3;

    public int LineWidth { get; set; } = 5;

    public PatternSettings Clone()
    {
        return (PatternSettings)MemberwiseClone();
    }
}
