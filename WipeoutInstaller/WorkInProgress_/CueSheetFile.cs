namespace WipeoutInstaller.WorkInProgress;

public sealed class CueSheetFile
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

    public List<CueSheetTrack> Tracks { get; } = new();

    public string? Title { get; set; }

    public override string ToString()
    {
        return $"{nameof(Type)}: {Type}, {nameof(Tracks)}: {Tracks.Count}, {nameof(Name)}: {Name}";
    }
}