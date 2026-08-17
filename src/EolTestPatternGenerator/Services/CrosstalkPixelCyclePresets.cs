using EolTestPatternGenerator.Models;

namespace EolTestPatternGenerator.Services;

/// <summary>
/// 将旧版 RGB/RBG/GRB/GBR/BRG/BGR 六种通道排列映射到新的逐像素周期模型。
/// </summary>
public static class CrosstalkPixelCyclePresets
{
    private static readonly RgbChannelMask[] LegacyRgbPixels =
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

    /// <summary>创建与旧版某种通道排列逐像素一致的 8 像素兼容周期。</summary>
    public static CrosstalkPixelCycle CreateLegacy(RgbPixelOrder order)
    {
        ValidateLegacyOrder(order);
        return new CrosstalkPixelCycle
        {
            Pixels = LegacyRgbPixels.Select(mask => Permute(mask, order)).ToList(),
            ColumnAdvance = -3,
            RowAdvance = 1,
            TiltAngleDegrees = CrosstalkPixelCycle.DefaultTiltAngleDegrees
        };
    }

    /// <summary>
    /// 将旧设置中的任意自定义周期收敛为最接近的固定 8 像素历史预设。
    /// 比较 R/G/B 通道差异时会重复候选 8 像素模板；位置不足时按全灭处理，
    /// 分数相同时按枚举顺序优先选择 RGB。原设置的有效倾斜角会完整保留。
    /// </summary>
    public static CrosstalkPixelCycle NormalizeToNearestLegacyPreset(CrosstalkPixelCycle? cycle)
    {
        RgbPixelOrder order = FindNearestLegacyOrder(cycle);
        CrosstalkPixelCycle normalized = CreateLegacy(order);
        if (cycle is null)
        {
            return normalized;
        }

        double tiltAngle = cycle.ResolveTiltAngleDegrees();
        if (!double.IsFinite(tiltAngle))
        {
            tiltAngle = CrosstalkPixelCycle.DefaultTiltAngleDegrees;
        }

        normalized.SetTiltAngleDegrees(Math.Clamp(
            tiltAngle,
            CrosstalkPixelCycle.MinimumTiltAngleDegrees,
            CrosstalkPixelCycle.MaximumTiltAngleDegrees));
        return normalized;
    }

    /// <summary>
    /// 返回与给定周期通道最接近的六种历史排列之一。
    /// 横纵步进和倾斜角不参与比较，因此改变倾角不会让界面丢失当前排列。
    /// </summary>
    public static RgbPixelOrder FindNearestLegacyOrder(CrosstalkPixelCycle? cycle)
    {
        if (cycle?.Pixels is not { Count: > 0 } pixels)
        {
            return RgbPixelOrder.RGB;
        }

        RgbPixelOrder bestOrder = RgbPixelOrder.RGB;
        int bestDifference = int.MaxValue;
        int comparisonLength = Math.Max(
            LegacyRgbPixels.Length,
            Math.Min(pixels.Count, CrosstalkPixelCycle.MaximumPeriodLength));

        foreach (RgbPixelOrder candidate in Enum.GetValues<RgbPixelOrder>())
        {
            int difference = 0;
            for (int index = 0; index < comparisonLength; index++)
            {
                RgbChannelMask actual = index < pixels.Count
                    ? pixels[index] & RgbChannelMask.All
                    : RgbChannelMask.None;
                RgbChannelMask expected = Permute(
                    LegacyRgbPixels[index % LegacyRgbPixels.Length],
                    candidate);
                difference += CountDifferentChannels(actual, expected);
            }

            if (difference < bestDifference)
            {
                bestDifference = difference;
                bestOrder = candidate;
            }
        }

        return bestOrder;
    }

    /// <summary>
    /// 判断周期像素是否等于指定历史排列按当前长度截取或以全灭位置补齐后的结果。
    /// 这个严格匹配用于恢复短周期时保存的排列选择；例如周期为 1 时 RGB 与 RBG
    /// 的首位置相同，不能只靠最近差异重新推断用户原先选择。
    /// </summary>
    public static bool MatchesResizedLegacyOrder(CrosstalkPixelCycle? cycle, RgbPixelOrder order)
    {
        if (!Enum.IsDefined(order) ||
            cycle?.Pixels is not { Count: >= CrosstalkPixelCycle.MinimumPeriodLength and
                <= CrosstalkPixelCycle.MaximumPeriodLength } pixels)
        {
            return false;
        }

        CrosstalkPixelCycle expected = CreateLegacy(order);
        expected.Resize(pixels.Count);
        return pixels.SequenceEqual(expected.Pixels);
    }

    /// <summary>
    /// 检查一个自定义周期是否仍与旧版六种预设之一完全相同。
    /// 可用于界面在加载旧配置或用户恢复预设时显示对应名称。
    /// </summary>
    public static bool TryGetLegacyOrder(CrosstalkPixelCycle? cycle, out RgbPixelOrder order)
    {
        if (cycle is not null &&
            cycle.ColumnAdvance == -3 &&
            IsLegacyTilt(cycle) &&
            cycle.Pixels is not null &&
            cycle.Pixels.Count == LegacyRgbPixels.Length)
        {
            foreach (RgbPixelOrder candidate in Enum.GetValues<RgbPixelOrder>())
            {
                bool matches = true;
                for (int index = 0; index < LegacyRgbPixels.Length; index++)
                {
                    if (cycle.Pixels[index] != Permute(LegacyRgbPixels[index], candidate))
                    {
                        matches = false;
                        break;
                    }
                }

                if (matches)
                {
                    order = candidate;
                    return true;
                }
            }
        }

        order = default;
        return false;
    }

    /// <summary>
    /// 返回实际用于生成的周期。新字段缺失代表旧配置，此时按旧 PixelOrder 即时转换，
    /// 不修改传入配置，因而旧 JSON 可无损加载并在下一次保存时继续保持兼容。
    /// </summary>
    public static CrosstalkPixelCycle Resolve(PatternSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        return settings.PixelCycle ?? CreateLegacy(settings.PixelOrder);
    }

    private static RgbChannelMask Permute(RgbChannelMask logicalMask, RgbPixelOrder order)
    {
        bool logicalRed = logicalMask.HasFlag(RgbChannelMask.Red);
        bool logicalGreen = logicalMask.HasFlag(RgbChannelMask.Green);
        bool logicalBlue = logicalMask.HasFlag(RgbChannelMask.Blue);

        (bool red, bool green, bool blue) = order switch
        {
            RgbPixelOrder.RGB => (logicalRed, logicalGreen, logicalBlue),
            RgbPixelOrder.RBG => (logicalRed, logicalBlue, logicalGreen),
            RgbPixelOrder.GRB => (logicalGreen, logicalRed, logicalBlue),
            RgbPixelOrder.GBR => (logicalGreen, logicalBlue, logicalRed),
            RgbPixelOrder.BRG => (logicalBlue, logicalRed, logicalGreen),
            RgbPixelOrder.BGR => (logicalBlue, logicalGreen, logicalRed),
            _ => throw new ArgumentOutOfRangeException(nameof(order), order, null)
        };

        RgbChannelMask result = RgbChannelMask.None;
        if (red)
        {
            result |= RgbChannelMask.Red;
        }

        if (green)
        {
            result |= RgbChannelMask.Green;
        }

        if (blue)
        {
            result |= RgbChannelMask.Blue;
        }

        return result;
    }

    private static int CountDifferentChannels(RgbChannelMask left, RgbChannelMask right)
    {
        int difference = (int)(left ^ right);
        return ((difference & (int)RgbChannelMask.Red) != 0 ? 1 : 0) +
               ((difference & (int)RgbChannelMask.Green) != 0 ? 1 : 0) +
               ((difference & (int)RgbChannelMask.Blue) != 0 ? 1 : 0);
    }

    private static void ValidateLegacyOrder(RgbPixelOrder order)
    {
        if (!Enum.IsDefined(order))
        {
            throw new ArgumentOutOfRangeException(nameof(order), order, null);
        }
    }

    private static bool IsLegacyTilt(CrosstalkPixelCycle cycle)
    {
        try
        {
            return cycle.CalculateEffectiveRowAdvance() == 1.0d;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }
}
