using System.Text.Json.Serialization;
using EolTestPatternGenerator.Services;

namespace EolTestPatternGenerator.Models;

public sealed class PatternSettings
{
    /// <summary>选择底图算法；白框是独立叠加层，不占用图卡类型。</summary>
    public PatternType PatternType { get; set; } = PatternType.Border;

    public int CanvasWidth { get; set; } = 1920;

    public int CanvasHeight { get; set; } = 1080;

    /// <summary>图案最左侧外缘到画布左边缘的有符号距离。</summary>
    public int LeftMargin { get; set; } = 71;

    /// <summary>图案最上侧外缘到画布上边缘的有符号距离。</summary>
    public int TopMargin { get; set; } = 226;

    /// <summary>图案最右侧外缘到画布右边缘的有符号距离。</summary>
    public int RightMargin { get; set; } = 72;

    /// <summary>图案最下侧外缘到画布下边缘的有符号距离。</summary>
    public int BottomMargin { get; set; } = 227;

    /// <summary>由当前画布和外缘四边距实时计算出的图案宽度。</summary>
    [JsonIgnore]
    public long CalculatedOuterWidth =>
        MarginGeometry.CalculateWidth(CanvasWidth, LeftMargin, RightMargin);

    /// <summary>由当前画布和外缘四边距实时计算出的图案高度。</summary>
    [JsonIgnore]
    public long CalculatedOuterHeight =>
        MarginGeometry.CalculateHeight(CanvasHeight, TopMargin, BottomMargin);

    /// <summary>
    /// 点阵首末圆心的横向跨度；非点阵图卡等于外缘宽度。
    /// 圆的像素包围盒宽度为圆心跨度 + 2 × 半径 + 1。
    /// </summary>
    [JsonIgnore]
    public long CalculatedCenterSpanWidth => IsDotGrid
        ? CalculatedOuterWidth - (2L * DotRadius) - 1L
        : CalculatedOuterWidth;

    /// <summary>点阵首末圆心的纵向跨度；非点阵图卡等于外缘高度。</summary>
    [JsonIgnore]
    public long CalculatedCenterSpanHeight => IsDotGrid
        ? CalculatedOuterHeight - (2L * DotRadius) - 1L
        : CalculatedOuterHeight;

    [JsonIgnore]
    public bool IsDotGrid => PatternType is PatternType.NinePointGrid or PatternType.DistortionGrid;

    public int Phase { get; set; } = 1;

    /// <summary>
    /// 旧版六种通道排列的兼容字段；仅当 <see cref="PixelCycle"/> 为 null 时生效。
    /// 默认 RGB 保持旧样图逐像素兼容。
    /// </summary>
    public RgbPixelOrder PixelOrder { get; set; } = RgbPixelOrder.RGB;

    /// <summary>
    /// 新版逐像素串扰周期。旧配置没有此字段时保持为 null，生成器会按
    /// <see cref="PixelOrder"/> 转换为对应的历史 8 像素兼容预设。
    /// </summary>
    public CrosstalkPixelCycle? PixelCycle { get; set; }

    public int DotRadius { get; set; } = 4;

    public int Rows { get; set; } = 3;

    public int Columns { get; set; } = 3;

    public int LineWidth { get; set; } = 5;

    public ScreenSplitMode ScreenSplitMode { get; set; } = ScreenSplitMode.TwoDimensionalBlackLeft;

    /// <summary>
    /// 导入图片图卡的底图路径。由生成器以 Unicode 安全方式读取。
    /// </summary>
    public string SourceImagePath { get; set; } = string.Empty;

    /// <summary>所有底图生成完毕后绘制的可选白框。</summary>
    public BorderOverlaySettings BorderOverlay { get; set; } = new();

    /// <summary>解析图案最外缘区域，并统一校验派生尺寸和整数溢出。</summary>
    public PixelRegion GetOuterRegion()
    {
        return MarginGeometry.Resolve(
            CanvasWidth,
            CanvasHeight,
            new RegionMargins(LeftMargin, TopMargin, RightMargin, BottomMargin));
    }

    /// <summary>
    /// 将 v1 的坐标/尺寸语义无损换算为 v2 外缘四边距。
    /// 点阵的旧 X/Y 是首个圆心，旧 Width/Height 是首末圆心跨度。
    /// </summary>
    public void SetLegacyBounds(int x, int y, int width, int height)
    {
        long outerX = x;
        long outerY = y;
        long outerWidth = width;
        long outerHeight = height;

        if (IsDotGrid)
        {
            outerX -= DotRadius;
            outerY -= DotRadius;
            outerWidth += (2L * DotRadius) + 1L;
            outerHeight += (2L * DotRadius) + 1L;
        }

        if (outerX is < int.MinValue or > int.MaxValue ||
            outerY is < int.MinValue or > int.MaxValue ||
            outerWidth is < 1 or > int.MaxValue ||
            outerHeight is < 1 or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "旧图案坐标和尺寸无法转换为四边距。");
        }

        RegionMargins margins = MarginGeometry.FromLegacyBounds(
            CanvasWidth,
            CanvasHeight,
            (int)outerX,
            (int)outerY,
            (int)outerWidth,
            (int)outerHeight);
        LeftMargin = margins.Left;
        TopMargin = margins.Top;
        RightMargin = margins.Right;
        BottomMargin = margins.Bottom;
    }

    public RegionMargins GetMargins()
    {
        return new RegionMargins(LeftMargin, TopMargin, RightMargin, BottomMargin);
    }

    public void SetMargins(RegionMargins margins)
    {
        ArgumentNullException.ThrowIfNull(margins);
        LeftMargin = margins.Left;
        TopMargin = margins.Top;
        RightMargin = margins.Right;
        BottomMargin = margins.Bottom;
    }

    /// <summary>全屏纯色和导入图片不使用位置参数，始终固定为四边距 0。</summary>
    public void NormalizeFullCanvasMargins()
    {
        if (PatternType is PatternType.Black or PatternType.FullWhite or PatternType.FullRed or
            PatternType.FullGreen or PatternType.FullBlue or PatternType.ImportedImage)
        {
            LeftMargin = 0;
            TopMargin = 0;
            RightMargin = 0;
            BottomMargin = 0;
        }
    }

    /// <summary>
    /// 单列/单行点阵在对应方向只有一个圆心，因此首末圆心跨度必须为 0。
    /// 归一化时保留首圆心（即左/上外缘），只重算右/下边距。
    /// </summary>
    public bool NormalizeSingleAxisDotSpans()
    {
        if (!IsDotGrid)
        {
            return false;
        }

        bool changed = false;
        if (Columns == 1 && CalculatedCenterSpanWidth != 0)
        {
            PatternWidth = 0;
            changed = true;
        }

        if (Rows == 1 && CalculatedCenterSpanHeight != 0)
        {
            PatternHeight = 0;
            changed = true;
        }

        return changed;
    }

    /// <summary>
    /// 修改点半径时保留首圆心和首末圆心跨度，再由新半径反算真实外缘。
    /// 单列/单行的对应跨度始终按 0 处理。
    /// </summary>
    public void SetDotRadiusPreservingCenterGeometry(int dotRadius)
    {
        if (!IsDotGrid)
        {
            throw new InvalidOperationException("只有点阵图卡可以按圆心几何同步半径。");
        }

        int firstCenterX = PatternX;
        int firstCenterY = PatternY;
        int centerSpanWidth = Columns == 1 ? 0 : PatternWidth;
        int centerSpanHeight = Rows == 1 ? 0 : PatternHeight;

        // 先在副本上完成所有 long/int 边界校验，失败时不破坏原配置。
        PatternSettings candidate = Clone();
        candidate.DotRadius = dotRadius;
        candidate.SetLegacyBounds(firstCenterX, firstCenterY, centerSpanWidth, centerSpanHeight);
        candidate.NormalizeSingleAxisDotSpans();

        DotRadius = candidate.DotRadius;
        SetMargins(candidate.GetMargins());
    }

    // 以下四个属性用于旧窗体和旧批量导出代码过渡；v2 JSON 只保存外缘四边距。
    [JsonIgnore]
    public int PatternX
    {
        get => CheckedInt((long)LeftMargin + (IsDotGrid ? DotRadius : 0), "图案 X");
        set => LeftMargin = CheckedInt((long)value - (IsDotGrid ? DotRadius : 0), "图案左边距");
    }

    [JsonIgnore]
    public int PatternY
    {
        get => CheckedInt((long)TopMargin + (IsDotGrid ? DotRadius : 0), "图案 Y");
        set => TopMargin = CheckedInt((long)value - (IsDotGrid ? DotRadius : 0), "图案上边距");
    }

    [JsonIgnore]
    public int PatternWidth
    {
        get => CheckedInt(CalculatedCenterSpanWidth, IsDotGrid ? "圆心水平跨度" : "图案宽度");
        set
        {
            long outerWidth = (long)value + (IsDotGrid ? (2L * DotRadius) + 1L : 0L);
            RightMargin = CalculateOppositeMargin(CanvasWidth, LeftMargin, outerWidth, "图案宽度");
        }
    }

    [JsonIgnore]
    public int PatternHeight
    {
        get => CheckedInt(CalculatedCenterSpanHeight, IsDotGrid ? "圆心垂直跨度" : "图案高度");
        set
        {
            long outerHeight = (long)value + (IsDotGrid ? (2L * DotRadius) + 1L : 0L);
            BottomMargin = CalculateOppositeMargin(CanvasHeight, TopMargin, outerHeight, "图案高度");
        }
    }

    public PatternSettings Clone()
    {
        var clone = (PatternSettings)MemberwiseClone();
        clone.SourceImagePath = SourceImagePath;
        clone.PixelCycle = PixelCycle?.Clone();
        // 配置快照必须深拷贝可变的白框对象，避免不同图卡参数相互污染。
        clone.BorderOverlay = BorderOverlay.Clone();
        return clone;
    }

    private static int CalculateOppositeMargin(int canvasSize, int leadingMargin, long size, string description)
    {
        long result = (long)canvasSize - leadingMargin - size;
        return CheckedInt(result, $"{description}对应的末端边距");
    }

    private static int CheckedInt(long value, string description)
    {
        if (value is < int.MinValue or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(value), $"{description}超出支持的整数范围。");
        }

        return (int)value;
    }
}
