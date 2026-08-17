namespace EolTestPatternGenerator.Projection;

/// <summary>
/// Windows 的 Win+P 投影拓扑。<see cref="None"/> 表示保持当前显示器拓扑，
/// 只更新桌面壁纸。
/// </summary>
public enum DisplayTopology
{
    None = 0,
    Internal = 1,
    Clone = 2,
    External = 3,
    Extend = 4
}
