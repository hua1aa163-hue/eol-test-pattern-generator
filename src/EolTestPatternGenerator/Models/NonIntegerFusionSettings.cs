namespace EolTestPatternGenerator.Models;

/// <summary>
/// MATLAB 非整数周期连续融合算法所需的参数。
/// </summary>
public sealed class NonIntegerFusionSettings
{
    /// <summary>内置图像源的画布宽度；自定义图像模式直接使用源图宽度。</summary>
    public int CanvasWidth { get; set; } = 1920;

    /// <summary>内置图像源的画布高度；自定义图像模式直接使用源图高度。</summary>
    public int CanvasHeight { get; set; } = 1080;

    /// <summary>MATLAB 中的子像素周期 spixtol。</summary>
    public double SubpixelPeriod { get; set; } = 10.0;

    /// <summary>
    /// MATLAB 中的缩放因子 m。它不缩放图像，只用于计算有效周期
    /// T = SubpixelPeriod * ScaleFactor。
    /// </summary>
    public double ScaleFactor { get; set; } = 1.0;

    /// <summary>MATLAB 中的倾斜角 theta，单位为度。</summary>
    public double ThetaDegrees { get; set; } = 18.435;

    /// <summary>MATLAB 中的循环移位 picnum，单位为子像素。</summary>
    public double PhaseShift { get; set; }

    /// <summary>
    /// 可调 Duty 模式下，左眼半周期内的点亮比例。
    /// 例如 0.25 表示点亮宽度为 T / 8，即整个周期的 12.5%。
    /// </summary>
    public double Duty { get; set; } = 0.25;

    /// <summary>左右眼图像来源。</summary>
    public NonIntegerFusionSourceMode SourceMode { get; set; } =
        NonIntegerFusionSourceMode.DutyWhiteBlack;

    /// <summary>融合前交换左右眼图像，与 MATLAB 的“反转左右眼”一致。</summary>
    public bool ReverseEyes { get; set; }

    /// <summary>创建参数副本，避免生成期间的界面修改影响结果。</summary>
    public NonIntegerFusionSettings Clone() => (NonIntegerFusionSettings)MemberwiseClone();
}
