namespace WipeoutInstaller.WorkInProgress;

public sealed class CueSheetTrackIndex
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