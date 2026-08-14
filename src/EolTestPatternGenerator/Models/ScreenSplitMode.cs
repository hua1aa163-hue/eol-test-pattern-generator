namespace EolTestPatternGenerator.Models;

/// <summary>
/// 1号屏参考图的黑白区域组合。前三项对应普通三张图，后两项保留旧3D兼容。
/// </summary>
public enum ScreenSplitMode
{
    TwoDimensionalBlackLeft = 0,
    TwoDimensionalBlackRight = 1,
    TwoDimensionalBlackBoth = 2,
    ThreeDimensionalBlackRight = 3,
    ThreeDimensionalBlackLeft = 4
}
