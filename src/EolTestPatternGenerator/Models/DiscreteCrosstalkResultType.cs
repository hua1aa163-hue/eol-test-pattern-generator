namespace EolTestPatternGenerator.Models;

/// <summary>
/// 离散光源图的两路图源组合方式。
/// </summary>
public enum DiscreteCrosstalkResultType
{
    /// <summary>第一路为白图，第二路为黑图；文件名标记为 WB。</summary>
    WhiteBlack,

    /// <summary>第一路为黑图，第二路为白图；文件名标记为 BW。</summary>
    BlackWhite,

    /// <summary>第一路为红图，第二路为蓝图；文件名标记为 RB。</summary>
    RedBlue,

    /// <summary>使用调用方提供的两张三通道图片；文件名标记为 USER。</summary>
    Custom
}
