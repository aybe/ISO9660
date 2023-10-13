namespace WipeoutInstaller.WorkInProgress;

public class CueSheetFile
{
    public CueSheetFile(string name, CueSheetFileType type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; }

    public CueSheetFileType Type { get; }

    public List<CueSheetTrack> Tracks { get; } = new();

    public string? Title { get; set; }

    public override string ToString()
    {
        return $"{nameof(Type)}: {Type}, {nameof(Tracks)}: {Tracks.Count}, {nameof(Name)}: {Name}";
    }
}