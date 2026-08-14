namespace EolTestPatternGenerator.Models;

/// <summary>
/// “1号屏图卡”的可编辑画布和左右区域参数。
/// </summary>
public sealed class ScreenOneSettings
{
    /// <summary>导出图像的像素宽度。</summary>
    public int CanvasWidth { get; set; } = 3200;

    /// <summary>导出图像的像素高度。</summary>
    public int CanvasHeight { get; set; } = 2000;

    /// <summary>左区域左上角的 X 坐标。</summary>
    public int LeftX { get; set; } = 50;

    /// <summary>左区域左上角的 Y 坐标。</summary>
    public int LeftY { get; set; } = 50;

    /// <summary>左区域的像素宽度。</summary>
    public int LeftWidth { get; set; } = 1500;

    /// <summary>左区域的像素高度。</summary>
    public int LeftHeight { get; set; } = 1900;

    /// <summary>右区域左上角的 X 坐标。</summary>
    public int RightX { get; set; } = 1650;

    /// <summary>右区域左上角的 Y 坐标。</summary>
    public int RightY { get; set; } = 50;

    /// <summary>右区域的像素宽度。</summary>
    public int RightWidth { get; set; } = 1500;

    /// <summary>右区域的像素高度。</summary>
    public int RightHeight { get; set; } = 1900;

    /// <summary>
    /// 创建与三张参考图一致的 3200 × 2000 默认布局。
    /// </summary>
    public static ScreenOneSettings CreateReferenceDefault() => new();

    /// <summary>
    /// 创建参数副本，供后台导出使用，避免导出过程中界面更改影响结果。
    /// </summary>
    public ScreenOneSettings Clone() => (ScreenOneSettings)MemberwiseClone();
}
