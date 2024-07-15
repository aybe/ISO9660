using System.Runtime.CompilerServices;
using Whatever.Extensions;

namespace ISO9660.Physical;

public abstract class Track : DisposableAsync
{
    public bool Audio { get; protected init; }

    public int Index { get; protected init; }

    public int Length { get; protected init; }

    public int Position { get; protected init; }

    public ISector Sector { get; protected init; } = null!;

    protected Track()
    {
    }

    protected Track(bool audio, int index, int length, int position, ISector sector)
    {
        Audio    = audio;
        Index    = index;
        Length   = length;
        Position = position;
        Sector   = sector;
    }

    internal Stream GetStream(int sector)
    {
        ValidateSectorIndex(sector);

        return new TrackStream(this, sector);
    }

    public abstract ISector ReadSector(int index);

    public abstract Task<ISector> ReadSectorAsync(int index);

    protected ISector ReadSector(int index, Stream stream)
    {
        ReadSectorInit(stream, index);

        var sector = ISector.Read(Sector, stream);

        return sector;
    }

    protected Task<ISector> ReadSectorAsync(int index, Stream stream)
    {
        ReadSectorInit(stream, index);

        var sector = ISector.ReadAsync(Sector, stream);

        return sector;
    }

    private void ReadSectorInit(Stream stream, int index)
    {
        ValidateSectorIndex(index);

        stream.Position = index * Sector.Length;
    }

    private void ValidateSectorIndex(int index, [CallerArgumentExpression(nameof(index))] string indexName = null!)
    {
        if (index < Position || index >= Position + Length)
        {
            throw new ArgumentOutOfRangeException(indexName, index, null);
        }
    }

    public override string ToString()
    {
        return $"{nameof(Index)}: {Index}, {nameof(Position)}: {Position}, {nameof(Length)}: {Length}, {nameof(Audio)}: {Audio}";
    }
}