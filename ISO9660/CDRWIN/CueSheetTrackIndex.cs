using ISO9660.Common;

namespace ISO9660.CDRWIN;

public sealed class CueSheetTrackIndex : CueSheetElement
{
    public CueSheetTrackIndex(CueSheetTrack track, int number, MSF position)
    {
        Track    = track;
        Number   = number;
        Position = position;
    }

    public CueSheetTrack Track { get; }

    public int Number { get; }

    public MSF Position { get; }

    public override string ToString()
    {
        return $"{nameof(Number)}: {Number}, {nameof(Position)}: {Position}";
    }
}