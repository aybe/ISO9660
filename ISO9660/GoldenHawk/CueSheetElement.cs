namespace ISO9660.GoldenHawk;

public abstract class CueSheetElement
{
    public IList<string> Comments { get; } = new List<string>();
}