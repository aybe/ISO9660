namespace ISO9660.CDRWIN;

public sealed class CueSheet
{
    public List<string> Comments { get; } = new();

    public ulong? Catalog { get; set; }

    public string? Performer { get; set; }

    public string? Title { get; set; }

    public List<CueSheetFile> Files { get; set; } = new();
}