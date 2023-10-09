namespace WipeoutInstaller.WorkInProgress;

public class CueSheetFile
{
    public CueSheetFile(string name, string type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; }

    public string Type { get; }

    public List<CueSheetTrack> Tracks { get; } = new();

    public string? Title { get; set; }

    public override string ToString()
    {
        return $"{nameof(Name)}: {Name}, {nameof(Type)}: {Type}";
    }
}