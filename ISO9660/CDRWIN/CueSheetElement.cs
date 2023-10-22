namespace ISO9660.CDRWIN;

public abstract class CueSheetElement
{
    public IList<string> Comments { get; } = new List<string>();
}