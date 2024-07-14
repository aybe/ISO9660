namespace ISO9660.GoldenHawk;

public sealed class CueSheetFile(CueSheet sheet, string name, CueSheetFileType type)
    : CueSheetElement
{
    public CueSheet Sheet { get; } = sheet;

    public string Name { get; } = name;

    public CueSheetFileType Type { get; } = type;

    public IList<CueSheetTrack> Tracks { get; } = new List<CueSheetTrack>();

    public string? Title { get; set; }

    public override string ToString()
    {
        return $"{nameof(Type)}: {Type}, {nameof(Tracks)}: {Tracks.Count}, {nameof(Name)}: {Name}";
    }
}