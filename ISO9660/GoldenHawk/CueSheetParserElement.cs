namespace ISO9660.GoldenHawk;

internal sealed class CueSheetParserElement(int indent, CueSheetElement target)
{
    public int Indent { get; } = indent;

    public CueSheetElement Target { get; } = target;

    public override string ToString()
    {
        return $"{nameof(Indent)}: {Indent}, {nameof(Target)}: {Target.GetType().Name}";
    }
}