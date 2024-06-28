using ISO9660.Physical;

namespace ISO9660;

public interface ITrack : IDisposable, IAsyncDisposable
{
    int Index { get; }

    int Position { get; }

    int Length { get; }

    bool Audio { get; }

    ISector Sector { get; }

    Stream GetStream(in int sector);

    Task<ISector> ReadSectorAsync(in int index);
}