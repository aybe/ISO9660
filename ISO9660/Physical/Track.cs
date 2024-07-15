using System.Runtime.CompilerServices;
using Whatever.Extensions;

namespace ISO9660.Physical;

public abstract class Track(bool audio, int index, int length, int position, ISector sector)
    : DisposableAsync
{
    public bool Audio { get; } = audio;

    public int Index { get; } = index;

    public int Length { get; } = length;

    public int Position { get; } = position;

    public ISector Sector { get; } = sector;

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