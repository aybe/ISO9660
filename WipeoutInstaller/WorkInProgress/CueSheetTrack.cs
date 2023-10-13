namespace WipeoutInstaller.WorkInProgress;

public class CueSheetTrack
{
    public CueSheetTrack(int index, CueSheetTrackType type)
    {
        Index = index;
        Type  = type;
    }

    public int Index { get; }

    public CueSheetTrackType Type { get; }

    public List<CueSheetTrackIndex> Indices { get; } = new();

    public CueSheetTrackFlags Flags { get; set; }

    public string? Title { get; set; }

    public string? Performer { get; set; }

    public MSF? PreGap { get; set; }

    public string? Isrc { get; set; }

    public override string ToString()
    {
        return $"{nameof(Index)}: {Index}, {nameof(Type)}: {Type}, {nameof(Indices)}: {Indices.Count}, {nameof(Flags)}: {Flags}, {nameof(PreGap)}: {PreGap}";
    }
}