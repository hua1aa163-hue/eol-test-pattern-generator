namespace EolTestPatternGenerator.Models;

/// <summary>
/// 三个逻辑颜色通道写入实际R/G/B通道时的排列顺序。
/// </summary>
public enum RgbPixelOrder
{
    RGB = 0,
    RBG = 1,
    GRB = 2,
    GBR = 3,
    BRG = 4,
    BGR = 5
}
