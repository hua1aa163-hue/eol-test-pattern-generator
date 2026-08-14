using System.Text.Json.Serialization;
using EolTestPatternGenerator.Services;

namespace EolTestPatternGenerator.Models;

/// <summary>
/// “显示器”图卡的可编辑画布和左右区域参数。两个区域均以各自最外缘四边距定义。
/// </summary>
public sealed class ScreenOneSettings
{
    /// <summary>导出图像的像素宽度。</summary>
    public int CanvasWidth { get; set; } = 3200;

    /// <summary>导出图像的像素高度。</summary>
    public int CanvasHeight { get; set; } = 2000;

    public int LeftRegionLeftMargin { get; set; } = 50;

    public int LeftRegionTopMargin { get; set; } = 50;

    public int LeftRegionRightMargin { get; set; } = 1650;

    public int LeftRegionBottomMargin { get; set; } = 50;

    public int RightRegionLeftMargin { get; set; } = 1650;

    public int RightRegionTopMargin { get; set; } = 50;

    public int RightRegionRightMargin { get; set; } = 50;

    public int RightRegionBottomMargin { get; set; } = 50;

    /// <summary>左区域由当前画布和四边距计算出的宽度，保留 long 以便界面显示无效输入。</summary>
    [JsonIgnore]
    public long CalculatedLeftWidth =>
        MarginGeometry.CalculateWidth(CanvasWidth, LeftRegionLeftMargin, LeftRegionRightMargin);

    /// <summary>左区域由当前画布和四边距计算出的高度。</summary>
    [JsonIgnore]
    public long CalculatedLeftHeight =>
        MarginGeometry.CalculateHeight(CanvasHeight, LeftRegionTopMargin, LeftRegionBottomMargin);

    /// <summary>右区域由当前画布和四边距计算出的宽度。</summary>
    [JsonIgnore]
    public long CalculatedRightWidth =>
        MarginGeometry.CalculateWidth(CanvasWidth, RightRegionLeftMargin, RightRegionRightMargin);

    /// <summary>右区域由当前画布和四边距计算出的高度。</summary>
    [JsonIgnore]
    public long CalculatedRightHeight =>
        MarginGeometry.CalculateHeight(CanvasHeight, RightRegionTopMargin, RightRegionBottomMargin);

    public PixelRegion GetLeftRegion()
    {
        return MarginGeometry.Resolve(
            CanvasWidth,
            CanvasHeight,
            new RegionMargins(
                LeftRegionLeftMargin,
                LeftRegionTopMargin,
                LeftRegionRightMargin,
                LeftRegionBottomMargin),
            "显示器左区域");
    }

    public PixelRegion GetRightRegion()
    {
        return MarginGeometry.Resolve(
            CanvasWidth,
            CanvasHeight,
            new RegionMargins(
                RightRegionLeftMargin,
                RightRegionTopMargin,
                RightRegionRightMargin,
                RightRegionBottomMargin),
            "显示器右区域");
    }

    public void SetLeftRegionFromLegacyBounds(int x, int y, int width, int height)
    {
        RegionMargins margins = MarginGeometry.FromLegacyBounds(
            CanvasWidth,
            CanvasHeight,
            x,
            y,
            width,
            height,
            "显示器左区域");
        LeftRegionLeftMargin = margins.Left;
        LeftRegionTopMargin = margins.Top;
        LeftRegionRightMargin = margins.Right;
        LeftRegionBottomMargin = margins.Bottom;
    }

    public void SetRightRegionFromLegacyBounds(int x, int y, int width, int height)
    {
        RegionMargins margins = MarginGeometry.FromLegacyBounds(
            CanvasWidth,
            CanvasHeight,
            x,
            y,
            width,
            height,
            "显示器右区域");
        RightRegionLeftMargin = margins.Left;
        RightRegionTopMargin = margins.Top;
        RightRegionRightMargin = margins.Right;
        RightRegionBottomMargin = margins.Bottom;
    }

    public RegionMargins GetLeftMargins()
    {
        return new RegionMargins(
            LeftRegionLeftMargin,
            LeftRegionTopMargin,
            LeftRegionRightMargin,
            LeftRegionBottomMargin);
    }

    public void SetLeftMargins(RegionMargins margins)
    {
        ArgumentNullException.ThrowIfNull(margins);
        LeftRegionLeftMargin = margins.Left;
        LeftRegionTopMargin = margins.Top;
        LeftRegionRightMargin = margins.Right;
        LeftRegionBottomMargin = margins.Bottom;
    }

    public RegionMargins GetRightMargins()
    {
        return new RegionMargins(
            RightRegionLeftMargin,
            RightRegionTopMargin,
            RightRegionRightMargin,
            RightRegionBottomMargin);
    }

    public void SetRightMargins(RegionMargins margins)
    {
        ArgumentNullException.ThrowIfNull(margins);
        RightRegionLeftMargin = margins.Left;
        RightRegionTopMargin = margins.Top;
        RightRegionRightMargin = margins.Right;
        RightRegionBottomMargin = margins.Bottom;
    }

    /// <summary>创建与三张参考图一致的 3200 × 2000 默认布局。</summary>
    public static ScreenOneSettings CreateReferenceDefault() => new();

    /// <summary>创建参数副本，供后台导出使用，避免导出过程中界面更改影响结果。</summary>
    public ScreenOneSettings Clone() => (ScreenOneSettings)MemberwiseClone();

    // 旧窗体的临时兼容别名；v2 JSON 仅保存上方八个四边距字段。
    [JsonIgnore]
    public int LeftX
    {
        get => LeftRegionLeftMargin;
        set => LeftRegionLeftMargin = value;
    }

    [JsonIgnore]
    public int LeftY
    {
        get => LeftRegionTopMargin;
        set => LeftRegionTopMargin = value;
    }

    [JsonIgnore]
    public int LeftWidth
    {
        get => CheckedDimension(CalculatedLeftWidth, "显示器左区域宽度");
        set => LeftRegionRightMargin = CalculateOppositeMargin(CanvasWidth, LeftRegionLeftMargin, value, "显示器左区域宽度");
    }

    [JsonIgnore]
    public int LeftHeight
    {
        get => CheckedDimension(CalculatedLeftHeight, "显示器左区域高度");
        set => LeftRegionBottomMargin = CalculateOppositeMargin(CanvasHeight, LeftRegionTopMargin, value, "显示器左区域高度");
    }

    [JsonIgnore]
    public int RightX
    {
        get => RightRegionLeftMargin;
        set => RightRegionLeftMargin = value;
    }

    [JsonIgnore]
    public int RightY
    {
        get => RightRegionTopMargin;
        set => RightRegionTopMargin = value;
    }

    [JsonIgnore]
    public int RightWidth
    {
        get => CheckedDimension(CalculatedRightWidth, "显示器右区域宽度");
        set => RightRegionRightMargin = CalculateOppositeMargin(CanvasWidth, RightRegionLeftMargin, value, "显示器右区域宽度");
    }

    [JsonIgnore]
    public int RightHeight
    {
        get => CheckedDimension(CalculatedRightHeight, "显示器右区域高度");
        set => RightRegionBottomMargin = CalculateOppositeMargin(CanvasHeight, RightRegionTopMargin, value, "显示器右区域高度");
    }

    private static int CheckedDimension(long value, string description)
    {
        if (value is < int.MinValue or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(value), $"{description}超出支持的整数范围。");
        }

        return (int)value;
    }

    private static int CalculateOppositeMargin(int canvasSize, int leadingMargin, int size, string description)
    {
        long oppositeMargin = (long)canvasSize - leadingMargin - size;
        if (oppositeMargin is < int.MinValue or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(size), $"{description}无法转换为四边距。");
        }

        return (int)oppositeMargin;
    }
}
