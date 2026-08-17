namespace EolTestPatternGenerator.Projection;

/// <summary>
/// 隔离 Windows 桌面壁纸与显示器拓扑操作，便于界面和实时控制器注入测试替身。
/// 实现可以同步调用系统 API；耗时编排由上层控制器负责。
/// </summary>
public interface IDesktopDisplayService
{
    /// <summary>读取当前桌面壁纸路径；系统读取失败时返回 <see langword="null"/>。</summary>
    string? GetCurrentWallpaper();

    /// <summary>应用 Win+P 投影拓扑；<see cref="DisplayTopology.None"/> 不执行切换。</summary>
    void ApplyTopology(DisplayTopology topology);

    /// <summary>
    /// 将指定图片设置为桌面壁纸；空字符串表示清除壁纸，路径不存在或系统调用失败时抛出异常。
    /// </summary>
    void SetWallpaper(string imagePath);
}
