namespace ISO9660.WorkInProgress;

public sealed class Track(int index, int Position, int Length, bool Audio) : ITrack
{
    public int Index { get; } = index;

    public int Position { get; } = Position;

    public int Length { get; } = Length;

    public bool Audio { get; } = Audio;

    public void Dispose()
    {
        // TODO
    }

    public override string ToString()
    {
        return $"{nameof(Index)}: {Index}, {nameof(Position)}: {Position}, {nameof(Length)}: {Length}, {nameof(Audio)}: {Audio}";
    }
}