namespace ISO9660.WorkInProgress;

public sealed class Track(byte Number, int Position, int Length, TrackType Type)
{
    public byte Number { get; } = Number;

    public int Position { get; } = Position;

    public int Length { get; } = Length;

    public TrackType Type { get; } = Type;
}