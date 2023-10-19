namespace WipeoutInstaller.WorkInProgress;

public sealed class CueSheetTrack
{
    public CueSheetTrack(CueSheetFile file, int index, CueSheetTrackType type)
    {
        File  = file;
        Index = index;
        Type  = type;
    }

    public CueSheetFile File { get; }

    public int Index { get; }

    public CueSheetTrackType Type { get; }

    public CueSheetTrackIndex? Index0 => Indices.SingleOrDefault(s => s.Number is 0);

    public CueSheetTrackIndex Index1 => Indices.Single(s => s.Number is 1);

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