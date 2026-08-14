namespace EolTestPatternGenerator.Models;

/// <summary>
/// 串扰像素排列的一个完整周期。<see cref="Pixels"/> 的元素序号就是周期内的像素序号，
/// 每个元素可独立选择 R/G/B 的任意组合，也可以选择 <see cref="RgbChannelMask.None"/>。
/// </summary>
public sealed class CrosstalkPixelCycle
{
    public const int MinimumPeriodLength = 1;

    public const double MinimumTiltAngleDegrees = -89.0d;

    public const double MaximumTiltAngleDegrees = 89.0d;

    /// <summary>
    /// 最初版串扰图的等价倾斜角。连续融合使用每行位移 3 × tan(theta)，
    /// 18.435° 得到约 1 个周期位置，并在生成时吸附为精确的 1 以保持旧样图逐像素不变。
    /// </summary>
    public const double DefaultTiltAngleDegrees = 18.435d;

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
    /// 旧 JSON 的整数纵向步进兼容字段。仅当 <see cref="TiltAngleDegrees"/> 缺失时用于
    /// 反算角度；新界面修改角度时也会写入最接近的整数，供旧版本程序合理回退。
    /// </summary>
    public int RowAdvance { get; set; } = 1;

    /// <summary>
    /// 倾斜角，单位为度，语义与非整数连续融合一致：图案局部 Y 每增加一行，
    /// 周期坐标移动 3 × tan(theta)。null 表示旧 JSON 尚未保存角度，此时由
    /// <see cref="RowAdvance"/> 反算，并按产品范围裁剪到 -89°～89°。
    /// </summary>
    public double? TiltAngleDegrees { get; set; }

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
            RowAdvance = RowAdvance,
            TiltAngleDegrees = TiltAngleDegrees
        };
    }

    /// <summary>
    /// 返回界面应显示的倾斜角。旧配置只有整数 RowAdvance 时用反正切换算，
    /// 超出新界面的产品范围时裁剪到 -89°～89°。
    /// </summary>
    public double ResolveTiltAngleDegrees()
    {
        if (TiltAngleDegrees is double savedAngle)
        {
            return savedAngle;
        }

        // 旧配置可能把整数纵向步进设到很大。换算后统一限制到新角度控件范围，
        // 使旧 JSON 在不经过界面时直接生成也不会因超过产品角度范围而失败。
        double legacyAngle = Math.Atan(RowAdvance / 3.0d) * 180.0d / Math.PI;
        return Math.Clamp(legacyAngle, MinimumTiltAngleDegrees, MaximumTiltAngleDegrees);
    }

    /// <summary>
    /// 设置新的倾斜角，并同步写入一个最接近的整数 RowAdvance 供旧版本程序回退读取。
    /// 新版生成器只以角度计算实际的非整数行位移。
    /// </summary>
    public void SetTiltAngleDegrees(double angleDegrees)
    {
        ValidateTiltAngle(angleDegrees);
        TiltAngleDegrees = angleDegrees;

        double effectiveAdvance = 3.0d * Math.Tan(angleDegrees * Math.PI / 180.0d);
        double roundedAdvance = Math.Round(effectiveAdvance, MidpointRounding.AwayFromZero);
        RowAdvance = roundedAdvance switch
        {
            < int.MinValue => int.MinValue,
            > int.MaxValue => int.MaxValue,
            _ => (int)roundedAdvance
        };
    }

    /// <summary>
    /// 计算实际的连续行位移。接近整数的结果吸附到整数，既消除 tan/atan 的浮点误差，
    /// 也确保默认 18.435° 与最初版 RowAdvance=1 的输出逐像素一致。
    /// </summary>
    public double CalculateEffectiveRowAdvance()
    {
        double angleDegrees = ResolveTiltAngleDegrees();
        ValidateTiltAngle(angleDegrees);

        double advance = 3.0d * Math.Tan(angleDegrees * Math.PI / 180.0d);
        if (!double.IsFinite(advance))
        {
            throw new ArgumentOutOfRangeException(
                nameof(TiltAngleDegrees),
                "倾斜角导致无效的每行位移。");
        }

        double nearestInteger = Math.Round(advance);
        return Math.Abs(advance - nearestInteger) <= 0.00001d
            ? nearestInteger
            : advance;
    }

    private static void ValidateTiltAngle(double angleDegrees)
    {
        if (!double.IsFinite(angleDegrees) ||
            angleDegrees < MinimumTiltAngleDegrees ||
            angleDegrees > MaximumTiltAngleDegrees)
        {
            throw new ArgumentOutOfRangeException(
                nameof(TiltAngleDegrees),
                "串扰倾斜角必须是 -89° 到 89° 范围内的有限数值。");
        }
    }
}
