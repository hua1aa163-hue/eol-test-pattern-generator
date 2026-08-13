namespace EolTestPatternGenerator.Models;

public sealed class BorderOverlaySettings
{
    public bool Enabled { get; set; }

    public int X { get; set; } = 71;

    public int Y { get; set; } = 226;

    public int Width { get; set; } = 1777;

    public int Height { get; set; } = 627;

    public int LineWidth { get; set; } = 5;

    public BorderOverlaySettings Clone()
    {
        return (BorderOverlaySettings)MemberwiseClone();
    }
}
