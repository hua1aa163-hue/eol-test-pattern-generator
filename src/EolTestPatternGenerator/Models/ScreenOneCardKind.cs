namespace EolTestPatternGenerator.Models;

/// <summary>
/// “1号屏图卡”窗口支持的三种图卡。
/// 枚举顺序与窗口中的图卡下拉框顺序保持一致。
/// </summary>
public enum ScreenOneCardKind
{
    /// <summary>左区域为黑色，右区域为白色。</summary>
    BlackLeftWhiteRight = 0,

    /// <summary>左区域为白色，右区域为黑色。</summary>
    WhiteLeftBlackRight = 1,

    /// <summary>左右两个区域均为黑色。</summary>
    BlackBoth = 2
}
