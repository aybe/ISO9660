namespace ISO9660.GoldenHawk;

public sealed class CueSheetTrackIndex(int number, MSF position)
    : CueSheetElement
{
    public int Number { get; } = number;

    public MSF Position { get; } = position;

    public override string ToString()
    {
        return $"{nameof(Number)}: {Number}, {nameof(Position)}: {Position}";
    }
}