using System.Text.Json.Serialization;
using EolTestPatternGenerator.Services;

namespace EolTestPatternGenerator.Models;

/// <summary>
/// 在底图完成后叠加的白色矩形框。四边距允许为负值，越过画布的部分由生成器裁剪。
/// </summary>
public sealed class BorderOverlaySettings
{
    private int _leftMargin = 71;
    private int _topMargin = 226;
    private int _rightMargin = 72;
    private int _bottomMargin = 227;
    private int _legacyWidth = 1777;
    private int _legacyHeight = 627;
    private int _canvasWidthHint = 1920;
    private int _canvasHeightHint = 1080;
    private bool _usesLegacyWidth;
    private bool _usesLegacyHeight;

    public bool Enabled { get; set; }

    /// <summary>白框最左侧外缘到画布左边缘的有符号距离。</summary>
    public int LeftMargin
    {
        get => _leftMargin;
        set => _leftMargin = value;
    }

    /// <summary>白框最上侧外缘到画布上边缘的有符号距离。</summary>
    public int TopMargin
    {
        get => _topMargin;
        set => _topMargin = value;
    }

    /// <summary>白框最右侧外缘到画布右边缘的有符号距离。</summary>
    public int RightMargin
    {
        get => _rightMargin;
        set
        {
            _rightMargin = value;
            _usesLegacyWidth = false;
        }
    }

    /// <summary>白框最下侧外缘到画布下边缘的有符号距离。</summary>
    public int BottomMargin
    {
        get => _bottomMargin;
        set
        {
            _bottomMargin = value;
            _usesLegacyHeight = false;
        }
    }

    public int LineWidth { get; set; } = 5;

    /// <summary>按指定画布计算出的白框宽度；无效或溢出的输入仍以 long 返回供界面提示。</summary>
    public long CalculateWidth(int canvasWidth)
    {
        return _usesLegacyWidth
            ? _legacyWidth
            : MarginGeometry.CalculateWidth(canvasWidth, LeftMargin, RightMargin);
    }

    /// <summary>按指定画布计算出的白框高度；无效或溢出的输入仍以 long 返回供界面提示。</summary>
    public long CalculateHeight(int canvasHeight)
    {
        return _usesLegacyHeight
            ? _legacyHeight
            : MarginGeometry.CalculateHeight(canvasHeight, TopMargin, BottomMargin);
    }

    /// <summary>解析当前白框区域，并用 long 中间值校验负尺寸和整数溢出。</summary>
    public PixelRegion GetRegion(int canvasWidth, int canvasHeight)
    {
        RegionMargins margins = GetEffectiveMargins(canvasWidth, canvasHeight);
        return MarginGeometry.Resolve(canvasWidth, canvasHeight, margins, "白框");
    }

    /// <summary>一次性设置四边距，同时清除仅供旧界面过渡使用的坐标尺寸状态。</summary>
    public void SetMargins(RegionMargins margins)
    {
        ArgumentNullException.ThrowIfNull(margins);
        _leftMargin = margins.Left;
        _topMargin = margins.Top;
        _rightMargin = margins.Right;
        _bottomMargin = margins.Bottom;
        _usesLegacyWidth = false;
        _usesLegacyHeight = false;
    }

    /// <summary>
    /// 将旧版 X/Y/Width/Height 输入转换成四边距。配置迁移和旧设计器控件均可调用。
    /// </summary>
    public void SetLegacyBounds(
        int canvasWidth,
        int canvasHeight,
        int x,
        int y,
        int width,
        int height)
    {
        SetMargins(MarginGeometry.FromLegacyBounds(
            canvasWidth,
            canvasHeight,
            x,
            y,
            width,
            height,
            "白框"));
        AttachCanvasSize(canvasWidth, canvasHeight);
    }

    /// <summary>
    /// 旧窗体仍以 X/Y/Width/Height 构造对象时，保存前由配置根对象补足画布尺寸并转成 v2 四边距。
    /// </summary>
    internal void NormalizeLegacyGeometry(int canvasWidth, int canvasHeight)
    {
        if (_usesLegacyWidth)
        {
            long right = (long)canvasWidth - LeftMargin - _legacyWidth;
            if (right is < int.MinValue or > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(canvasWidth), "白框横向坐标和宽度无法转换为四边距。");
            }

            _rightMargin = (int)right;
            _usesLegacyWidth = false;
        }

        if (_usesLegacyHeight)
        {
            long bottom = (long)canvasHeight - TopMargin - _legacyHeight;
            if (bottom is < int.MinValue or > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(canvasHeight), "白框纵向坐标和高度无法转换为四边距。");
            }

            _bottomMargin = (int)bottom;
            _usesLegacyHeight = false;
        }

        AttachCanvasSize(canvasWidth, canvasHeight);
    }

    internal void AttachCanvasSize(int canvasWidth, int canvasHeight)
    {
        _canvasWidthHint = canvasWidth;
        _canvasHeightHint = canvasHeight;
    }

    public BorderOverlaySettings Clone()
    {
        return (BorderOverlaySettings)MemberwiseClone();
    }

    public RegionMargins GetMargins()
    {
        return new RegionMargins(LeftMargin, TopMargin, RightMargin, BottomMargin);
    }

    private RegionMargins GetEffectiveMargins(int canvasWidth, int canvasHeight)
    {
        int right = RightMargin;
        int bottom = BottomMargin;

        if (_usesLegacyWidth)
        {
            long value = (long)canvasWidth - LeftMargin - _legacyWidth;
            if (value is < int.MinValue or > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(canvasWidth), "白框横向坐标和宽度超出支持范围。");
            }

            right = (int)value;
        }

        if (_usesLegacyHeight)
        {
            long value = (long)canvasHeight - TopMargin - _legacyHeight;
            if (value is < int.MinValue or > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(canvasHeight), "白框纵向坐标和高度超出支持范围。");
            }

            bottom = (int)value;
        }

        return new RegionMargins(LeftMargin, TopMargin, right, bottom);
    }

    // 以下四个属性只用于旧窗体在整体界面改版期间保持可编译；v2 JSON 不再保存坐标和尺寸。
    [JsonIgnore]
    public int X
    {
        get => LeftMargin;
        set => LeftMargin = value;
    }

    [JsonIgnore]
    public int Y
    {
        get => TopMargin;
        set => TopMargin = value;
    }

    [JsonIgnore]
    public int Width
    {
        get
        {
            long width = CalculateWidth(_canvasWidthHint);
            return width is < int.MinValue or > int.MaxValue ? 0 : (int)width;
        }
        set
        {
            _legacyWidth = value;
            _usesLegacyWidth = true;
        }
    }

    [JsonIgnore]
    public int Height
    {
        get
        {
            long height = CalculateHeight(_canvasHeightHint);
            return height is < int.MinValue or > int.MaxValue ? 0 : (int)height;
        }
        set
        {
            _legacyHeight = value;
            _usesLegacyHeight = true;
        }
    }
}
