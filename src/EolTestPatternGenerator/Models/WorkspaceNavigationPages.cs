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
    public const string CrosstalkPixelsGrid = "crosstalk-pixels-grid";
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
        3 => CrosstalkPixelsGrid,
        4 => ContinuousFusion,
        5 => DiscreteLightSource,
        6 => ImageToLightTools,
        7 => ImageToVideo,
        _ => throw new ArgumentOutOfRangeException(nameof(navigationIndex))
    };

    /// <summary>
    /// 解析上次打开的页面。旧 v2 配置没有稳定页面标识，其中序号 1 是串扰、
    /// 序号 2 是显示器，序号 3–6 是后续四页；仅在标识缺失时交换 1/2 并把
    /// 旧 3–6 后移一位，为新增的串扰像素排列2留出位置。保存后会写入稳定标识，
    /// 因而后续启动不会再次迁移。
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
                >= 3 and <= 6 => savedIndex + 1,
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
        CrosstalkPixelsGrid => 3,
        ContinuousFusion => 4,
        DiscreteLightSource => 5,
        ImageToLightTools => 6,
        ImageToVideo => 7,
        _ => -1
    };
}
