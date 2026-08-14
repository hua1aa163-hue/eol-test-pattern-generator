namespace EolTestPatternGenerator.Models;

/// <summary>
/// 显示器参考图的黑白区域组合。前三项对应当前三张图，后两项仅用于旧配置兼容。
/// </summary>
public enum ScreenSplitMode
{
    TwoDimensionalBlackLeft = 0,
    TwoDimensionalBlackRight = 1,
    TwoDimensionalBlackBoth = 2,
    ThreeDimensionalBlackRight = 3,
    ThreeDimensionalBlackLeft = 4
}
