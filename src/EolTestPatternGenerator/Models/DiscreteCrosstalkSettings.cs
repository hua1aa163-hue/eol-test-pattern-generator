namespace EolTestPatternGenerator.Models;

/// <summary>
/// “Lighttool Source Creator”风格离散光源图的全部计算参数。
/// </summary>
public sealed class DiscreteCrosstalkSettings
{
    /// <summary>输出图片的像素宽度。</summary>
    public int CanvasWidth { get; set; } = 1920;

    /// <summary>输出图片的像素高度。</summary>
    public int CanvasHeight { get; set; } = 1080;

    /// <summary>串扰条纹相对于 RGB 子像素阵列的倾斜角，单位为度。</summary>
    public double TiltAngleDegrees { get; set; } = 18.43;

    /// <summary>一个相位周期包含的子像素数量。</summary>
    public int SubpixelPeriod { get; set; } = 8;

    /// <summary>周期内每一路图源连续点亮的子像素数量。</summary>
    public int LitSubpixelCount { get; set; } = 4;

    /// <summary>指定相位组；-1 表示依次生成 0 到“周期-1”的全部组。</summary>
    public int Group { get; set; }

    /// <summary>单个像素的横向物理尺寸，单位为微米。</summary>
    public double PixelSizeXMicrometers { get; set; } = 57.6;

    /// <summary>单个像素的纵向物理尺寸，单位为微米。</summary>
    public double PixelSizeYMicrometers { get; set; } = 57.6;

    /// <summary>用于输出文件名的屏幕尺寸，单位为英寸。</summary>
    public double ScreenSizeInches { get; set; } = 5.0;

    /// <summary>中心及相邻分区的宽度，单位为子像素。</summary>
    public double PartitionWidthSubpixels { get; set; } = 5760d;

    /// <summary>分区中心线随纵坐标倾斜的策略角度，单位为度。</summary>
    public double StrategyAngleDegrees { get; set; }

    /// <summary>所有分区中心线的横向平移量，单位为子像素。</summary>
    public double TranslationSubpixels { get; set; }

    /// <summary>相邻分区的相位修正方向。</summary>
    public DiscreteCrosstalkDirection Direction { get; set; } = DiscreteCrosstalkDirection.LeftPositive;

    /// <summary>选择内置图源或调用方提供的两张自定义图片。</summary>
    public DiscreteCrosstalkResultType ResultType { get; set; } = DiscreteCrosstalkResultType.WhiteBlack;

    /// <summary>true 输出一个混合光源 TXT；false 分别输出 R、G、B 三个 TXT。</summary>
    public bool SingleLightSourceFile { get; set; } = true;

    /// <summary>创建参数快照，供预览或后台批量导出使用。</summary>
    public DiscreteCrosstalkSettings Clone() => (DiscreteCrosstalkSettings)MemberwiseClone();
}
