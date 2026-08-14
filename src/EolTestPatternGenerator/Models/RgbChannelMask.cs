namespace EolTestPatternGenerator.Models;

/// <summary>
/// 一个周期位置需要点亮的实际 RGB 通道。组合值表示多个通道同时点亮，
/// <see cref="None"/> 表示该位置全灭；生成器只会写入 0 或 255。
/// </summary>
[Flags]
public enum RgbChannelMask
{
    None = 0,
    Red = 1,
    Green = 2,
    Blue = 4,
    All = Red | Green | Blue
}
