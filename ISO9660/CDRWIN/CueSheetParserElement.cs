namespace ISO9660.CDRWIN;

internal sealed class CueSheetParserElement
{
    public CueSheetParserElement(int indent, CueSheetElement target)
    {
        Indent = indent;
        Target = target;
    }

    public int Indent { get; }

    public CueSheetElement Target { get; }

    public override string ToString()
    {
        return $"{nameof(Indent)}: {Indent}, {nameof(Target)}: {Target.GetType().Name}";
    }
}