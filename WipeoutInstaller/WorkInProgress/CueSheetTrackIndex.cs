namespace WipeoutInstaller.WorkInProgress;

public sealed class CueSheetTrackIndex
{
    public CueSheetTrackIndex(int number, MSF position)
    {
        Number   = number;
        Position = position;
    }

    public int Number { get; }

    public MSF Position { get; }

    public override string ToString()
    {
        return $"{nameof(Number)}: {Number}, {nameof(Position)}: {Position}";
    }
}