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

    public Stream GetStream(in int sector)
    {
        ValidateSectorIndex(sector);

        return new TrackStream(this, sector);
    }

    public abstract ISector ReadSector(in int index);

    public abstract Task<ISector> ReadSectorAsync(in int index);

    protected ISector ReadSector(in int index, in Stream stream)
    {
        ReadSectorInit(stream, index);

        var sector = ISector.Read(Sector, stream);

        return sector;
    }

    protected Task<ISector> ReadSectorAsync(in int index, in Stream stream)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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