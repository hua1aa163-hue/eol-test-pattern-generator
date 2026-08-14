namespace EolTestPatternGenerator.Models;

/// <summary>图片序列中的一项；同一路径可以重复出现，以支持任意播放顺序。</summary>
public sealed class StillVideoItemSettings
{
    public string ImagePath { get; set; } = string.Empty;

    /// <summary>该图片的静止播放秒数；编码时按累计时间边界分帧且至少写入一帧。</summary>
    public double DurationSeconds { get; set; } = 10d;

    public StillVideoItemSettings Clone()
    {
        return new StillVideoItemSettings
        {
            ImagePath = ImagePath ?? string.Empty,
            DurationSeconds = DurationSeconds
        };
    }
}

/// <summary>“图片转视频”页面需要跨进程保留的全部用户输入。</summary>
public sealed class StillVideoPreferences
{
    public List<StillVideoItemSettings> Items { get; set; } = new();

    public int OutputWidth { get; set; } = 1920;

    public int OutputHeight { get; set; } = 1080;

    public int FramesPerSecond { get; set; } = 30;

    public StillVideoFormat OutputFormat { get; set; } = StillVideoFormat.Mp4Mpeg4;

    public string OutputPath { get; set; } = string.Empty;

    public string LastInputDirectory { get; set; } = string.Empty;

    public string LastOutputDirectory { get; set; } = string.Empty;

    public StillVideoPreferences Clone()
    {
        return new StillVideoPreferences
        {
            OutputWidth = OutputWidth,
            OutputHeight = OutputHeight,
            FramesPerSecond = FramesPerSecond,
            OutputFormat = OutputFormat,
            OutputPath = OutputPath ?? string.Empty,
            LastInputDirectory = LastInputDirectory ?? string.Empty,
            LastOutputDirectory = LastOutputDirectory ?? string.Empty,
            Items = (Items ?? new List<StillVideoItemSettings>())
                .Where(item => item is not null)
                .Select(item => item.Clone())
                .ToList()
        };
    }

    internal void RestoreMissingSections()
    {
        Items ??= new List<StillVideoItemSettings>();
        Items.RemoveAll(item => item is null);
        foreach (StillVideoItemSettings item in Items)
        {
            item.ImagePath ??= string.Empty;
        }

        OutputPath ??= string.Empty;
        LastInputDirectory ??= string.Empty;
        LastOutputDirectory ??= string.Empty;
    }
}

/// <summary>一次编码使用的不可变参数快照。</summary>
public sealed class StillVideoEncodingOptions
{
    public required IReadOnlyList<StillVideoItemSettings> Items { get; init; }

    public required string OutputPath { get; init; }

    public int OutputWidth { get; init; } = 1920;

    public int OutputHeight { get; init; } = 1080;

    public int FramesPerSecond { get; init; } = 30;

    public StillVideoFormat OutputFormat { get; init; } = StillVideoFormat.Mp4Mpeg4;
}
