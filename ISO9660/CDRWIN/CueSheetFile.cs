namespace ISO9660.CDRWIN;

public sealed class CueSheetFile : CueSheetElement
{
    public CueSheetFile(CueSheet sheet, string name, CueSheetFileType type)
    {
        Sheet = sheet;
        Name  = name;
        Type  = type;
    }

    public CueSheet Sheet { get; }

    public string Name { get; }

    public CueSheetFileType Type { get; }

    public IList<CueSheetTrack> Tracks { get; } = new List<CueSheetTrack>();

    public string? Title { get; set; }

    public override string ToString()
    {
        return $"{nameof(Type)}: {Type}, {nameof(Tracks)}: {Tracks.Count}, {nameof(Name)}: {Name}";
    }
}