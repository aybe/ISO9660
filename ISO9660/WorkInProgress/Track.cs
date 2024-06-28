using ISO9660.Physical;

namespace ISO9660.WorkInProgress;

public sealed class Track(int index, int Position, int Length, bool Audio) : ITrack
{
    public int Index { get; } = index;

    public int Position { get; } = Position;

    public int Length { get; } = Length;

    public bool Audio { get; } = Audio;

    public ISector Sector => throw new NotImplementedException();

    public Stream GetStream(in int sector)
    {
        throw new NotImplementedException();
    }

    public Task<ISector> ReadSectorAsync(in int index)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        // TODO
    }

    public async ValueTask DisposeAsync()
    {
        // TODO
    }

    public override string ToString()
    {
        return $"{nameof(Index)}: {Index}, {nameof(Position)}: {Position}, {nameof(Length)}: {Length}, {nameof(Audio)}: {Audio}";
    }
}