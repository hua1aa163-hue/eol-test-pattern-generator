namespace EolTestPatternGenerator.Models;

public sealed record PatternChoice(PatternType Type, string Text)
{
    public override string ToString() => Text;
}
