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
            RowAdvance = 1
        };
    }

    /// <summary>
    /// 检查一个自定义周期是否仍与旧版六种预设之一完全相同。
    /// 可用于界面在加载旧配置或用户恢复预设时显示对应名称。
    /// </summary>
    public static bool TryGetLegacyOrder(CrosstalkPixelCycle? cycle, out RgbPixelOrder order)
    {
        if (cycle is not null &&
            cycle.ColumnAdvance == -3 &&
            cycle.RowAdvance == 1 &&
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

    private static void ValidateLegacyOrder(RgbPixelOrder order)
    {
        if (!Enum.IsDefined(order))
        {
            throw new ArgumentOutOfRangeException(nameof(order), order, null);
        }
    }
}
