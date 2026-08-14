namespace EolTestPatternGenerator.Models;

/// <summary>
/// 跨越中心分区后，相邻分区对相位的修正方向。
/// </summary>
public enum DiscreteCrosstalkDirection
{
    /// <summary>与原程序默认勾选状态一致：左侧相位按正方向递增。</summary>
    LeftPositive = 1,

    /// <summary>反转每个相邻分区的相位修正方向。</summary>
    RightPositive = -1
}
