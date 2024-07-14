namespace ISO9660.GoldenHawk;

public sealed class CueSheetTrack(CueSheetFile file, int index, CueSheetTrackType type)
    : CueSheetElement
{
    public CueSheetFile File { get; } = file;

    public int Index { get; } = index;

    public CueSheetTrackType Type { get; } = type;

    public CueSheetTrackIndex? Index0 => Indices.SingleOrDefault(s => s.Number is 0);

    public CueSheetTrackIndex Index1 => Indices.Single(s => s.Number is 1);

    public IList<CueSheetTrackIndex> Indices { get; } = new List<CueSheetTrackIndex>();

    public CueSheetTrackFlags Flags { get; set; } = CueSheetTrackFlags.None;

    public string? Title { get; set; }

    public string? Performer { get; set; }

    public MSF? PreGap { get; set; }

    public string? Isrc { get; set; }

    public override string ToString()
    {
        return $"{nameof(Index)}: {Index}, {nameof(Type)}: {Type}, {nameof(Indices)}: {Indices.Count}, {nameof(Flags)}: {Flags}, {nameof(PreGap)}: {PreGap}";
    }
}