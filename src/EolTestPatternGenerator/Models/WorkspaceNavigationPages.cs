namespace EolTestPatternGenerator.Models;

/// <summary>
/// 工作台页面的稳定标识和显示顺序。配置保存稳定标识而不是只保存列表序号，
/// 以后调整左侧导航顺序时不会把用户上次打开的页面误解为另一项。
/// </summary>
public static class WorkspaceNavigationPages
{
    public const string BasicPatterns = "basic-patterns";
    public const string DisplayCards = "display-cards";
    public const string CrosstalkPixels = "crosstalk-pixels";
    public const string ContinuousFusion = "continuous-fusion";
    public const string DiscreteLightSource = "discrete-light-source";
    public const string ImageToLightTools = "image-to-lighttools";
    public const string ImageToVideo = "image-to-video";

    /// <summary>按当前左侧导航顺序把列表序号转换为稳定页面标识。</summary>
    public static string GetPageId(int navigationIndex) => navigationIndex switch
    {
        0 => BasicPatterns,
        1 => DisplayCards,
        2 => CrosstalkPixels,
        3 => ContinuousFusion,
        4 => DiscreteLightSource,
        5 => ImageToLightTools,
        6 => ImageToVideo,
        _ => throw new ArgumentOutOfRangeException(nameof(navigationIndex))
    };

    /// <summary>
    /// 解析上次打开的页面。旧 v2 配置没有稳定页面标识，其中序号 1 是串扰、
    /// 序号 2 是显示器；仅在标识缺失时交换这两个旧序号。保存后会写入稳定标识，
    /// 因而后续启动不会再次迁移已经采用新顺序的序号。
    /// </summary>
    public static int ResolveSelectedIndex(WorkspacePreferences preferences, int pageCount)
    {
        ArgumentNullException.ThrowIfNull(preferences);

        if (pageCount <= 0)
        {
            return -1;
        }

        int pageIdIndex = GetIndexByPageId(preferences.SelectedNavigationPageId);
        if (pageIdIndex >= 0)
        {
            return Math.Clamp(pageIdIndex, 0, pageCount - 1);
        }

        int savedIndex = preferences.SelectedNavigationIndex;
        if (string.IsNullOrWhiteSpace(preferences.SelectedNavigationPageId))
        {
            savedIndex = savedIndex switch
            {
                1 => 2,
                2 => 1,
                _ => savedIndex
            };
        }

        return Math.Clamp(savedIndex, 0, pageCount - 1);
    }

    private static int GetIndexByPageId(string? pageId) => pageId switch
    {
        BasicPatterns => 0,
        DisplayCards => 1,
        CrosstalkPixels => 2,
        ContinuousFusion => 3,
        DiscreteLightSource => 4,
        ImageToLightTools => 5,
        ImageToVideo => 6,
        _ => -1
    };
}
