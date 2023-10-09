namespace WipeoutInstaller.WorkInProgress;

public class CueSheetTrack
{
    public CueSheetTrack(int index, string type)
    {
        Index = index;
        Type  = type;
    }

    public int Index { get; }

    public string Type { get; }

    public List<CueSheetTrackIndex> Indices { get; } = new();

    public CueSheetTrackFlags Flags { get; set; }

    public string? Title { get; set; }

    public string? Performer { get; set; }

    public Msf? PreGap { get; set; }

    public string? Isrc { get; set; }

    public override string ToString()
    {
        return $"{nameof(Index)}: {Index}, {nameof(Type)}: {Type}";
    }
}