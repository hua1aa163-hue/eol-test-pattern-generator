namespace EolTestPatternGenerator.Models;

/// <summary>
/// 非整数周期串扰融合时使用的左右眼图像来源。
/// </summary>
public enum NonIntegerFusionSourceMode
{
    /// <summary>左眼为全白图，右眼为全黑图。</summary>
    FullWhiteBlack = 0,

    /// <summary>
    /// 左眼仅在每个左眼半周期的起点处点亮 1 个子像素宽度，右眼为全黑图。
    /// </summary>
    FixedOneSubpixelWhiteBlack = 1,

    /// <summary>
    /// 左眼按 Duty 点亮左眼半周期的前部区域，右眼为全黑图。
    /// </summary>
    DutyWhiteBlack = 2,

    /// <summary>左眼为全红图，右眼为全蓝图。</summary>
    RedBlue = 3,

    /// <summary>使用调用方传入的自定义左右眼 Bitmap 或 Mat。</summary>
    CustomImages = 4
}
