namespace ISO9660.CDRWIN;

public sealed class CueSheet : CueSheetElement
{
    public ulong? Catalog { get; set; }

    public string? Performer { get; set; }

    public string? Title { get; set; }

    public IList<CueSheetFile> Files { get; } = new List<CueSheetFile>();
}