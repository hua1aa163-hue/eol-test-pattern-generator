namespace EolTestPatternGenerator.Models;

public sealed class PatternSettings
{
    /// <summary>选择底图算法；白框是独立叠加层，不占用图卡类型。</summary>
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

    /// <summary>相移图的逻辑通道排列；默认RGB保持旧样图逐像素兼容。</summary>
    public RgbPixelOrder PixelOrder { get; set; } = RgbPixelOrder.RGB;

    public int DotRadius { get; set; } = 4;

    public int Rows { get; set; } = 3;

    public int Columns { get; set; } = 3;

    public int LineWidth { get; set; } = 5;

    public ScreenSplitMode ScreenSplitMode { get; set; } = ScreenSplitMode.TwoDimensionalBlackLeft;

    /// <summary>
    /// 导入图片图卡的底图路径。由生成器以 Unicode 安全方式读取。
    /// </summary>
    public string SourceImagePath { get; set; } = string.Empty;

    /// <summary>所有底图生成完毕后绘制的可选白框。</summary>
    public BorderOverlaySettings BorderOverlay { get; set; } = new();

    public PatternSettings Clone()
    {
        var clone = (PatternSettings)MemberwiseClone();
        clone.SourceImagePath = SourceImagePath;
        // 配置快照必须深拷贝可变的白框对象，避免不同图卡参数相互污染。
        clone.BorderOverlay = BorderOverlay.Clone();
        return clone;
    }
}
