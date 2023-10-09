namespace WipeoutInstaller.WorkInProgress;

public sealed class CueSheetTrackIndex
{
    public CueSheetTrackIndex(int number, Msf position)
    {
        Number   = number;
        Position = position;
    }

    public int Number { get; }

    public Msf Position { get; }

    public override string ToString()
    {
        return $"{nameof(Number)}: {Number}, {nameof(Position)}: {Position}";
    }
}