namespace EolTestPatternGenerator.Models;

public sealed class ImageExportOptions
{
    public ImageFormatKind Format { get; set; } = ImageFormatKind.Png;

    public int Quality { get; set; } = 95;
}
