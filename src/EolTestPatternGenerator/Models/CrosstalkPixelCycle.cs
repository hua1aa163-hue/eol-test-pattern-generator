namespace EolTestPatternGenerator.Models;

/// <summary>
/// 串扰像素排列的一个完整周期。<see cref="Pixels"/> 的元素序号就是周期内的像素序号，
/// 每个元素可独立选择 R/G/B 的任意组合，也可以选择 <see cref="RgbChannelMask.None"/>。
/// </summary>
public sealed class CrosstalkPixelCycle
{
    public const int MinimumPeriodLength = 1;

    /// <summary>
    /// 为避免错误配置产生过大的编辑界面和配置文件，周期最多允许 1024 个像素。
    /// </summary>
    public const int MaximumPeriodLength = 1024;

    /// <summary>
    /// 周期内各像素点亮的实际通道。列表长度就是周期像素数。
    /// </summary>
    public List<RgbChannelMask> Pixels { get; set; } =
    [
        RgbChannelMask.Red,
        RgbChannelMask.Red | RgbChannelMask.Green,
        RgbChannelMask.All,
        RgbChannelMask.All,
        RgbChannelMask.Green | RgbChannelMask.Blue,
        RgbChannelMask.Blue,
        RgbChannelMask.None,
        RgbChannelMask.None
    ];

    /// <summary>
    /// 图案局部 X 坐标每增加 1，周期位置增加多少。历史 8 像素兼容预设使用 -3。
    /// </summary>
    public int ColumnAdvance { get; set; } = -3;

    /// <summary>
    /// 图案局部 Y 坐标每增加 1，周期位置增加多少。历史 8 像素兼容预设使用 1。
    /// </summary>
    public int RowAdvance { get; set; } = 1;

    /// <summary>当前周期像素数；修改周期请调用 <see cref="Resize"/>。</summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public int PeriodLength => Pixels?.Count ?? 0;

    /// <summary>
    /// 调整周期长度。扩展出来的位置默认全灭，缩短时保留前面的周期位置。
    /// </summary>
    public void Resize(int periodLength)
    {
        if (periodLength is < MinimumPeriodLength or > MaximumPeriodLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(periodLength),
                $"周期像素数必须在 {MinimumPeriodLength} 到 {MaximumPeriodLength} 之间。");
        }

        Pixels ??= [];
        if (Pixels.Count > periodLength)
        {
            Pixels.RemoveRange(periodLength, Pixels.Count - periodLength);
        }
        else
        {
            while (Pixels.Count < periodLength)
            {
                Pixels.Add(RgbChannelMask.None);
            }
        }
    }

    public CrosstalkPixelCycle Clone()
    {
        return new CrosstalkPixelCycle
        {
            Pixels = Pixels is null ? [] : [.. Pixels],
            ColumnAdvance = ColumnAdvance,
            RowAdvance = RowAdvance
        };
    }
}
