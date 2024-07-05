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
        if (sector < Position || sector >= Position + Length) // TODO DRY
        {
            throw new ArgumentOutOfRangeException(nameof(sector), sector, null);
        }

        return new TrackStream(this, sector);
    }

    public abstract ISector ReadSector(in int index);

    public abstract Task<ISector> ReadSectorAsync(in int index);

    protected ISector ReadSector(in int index, in Stream stream)
    {
        ValidateSectorIndex(index, nameof(index));

        stream.Position = index * Sector.Length;

        var sector = Sector switch
        {
            SectorCooked2048       => ISector.Read<SectorCooked2048>(stream),
            SectorCooked2324       => ISector.Read<SectorCooked2324>(stream),
            SectorCooked2336       => ISector.Read<SectorCooked2336>(stream),
            SectorRawAudio         => ISector.Read<SectorRawAudio>(stream),
            SectorRawMode0         => ISector.Read<SectorRawMode0>(stream),
            SectorRawMode1         => ISector.Read<SectorRawMode1>(stream),
            SectorRawMode2Form1    => ISector.Read<SectorRawMode2Form1>(stream),
            SectorRawMode2Form2    => ISector.Read<SectorRawMode2Form2>(stream),
            SectorRawMode2FormLess => ISector.Read<SectorRawMode2FormLess>(stream),
            _                      => throw new NotSupportedException(Sector.GetType().Name)
        };

        return sector;
    }

    protected Task<ISector> ReadSectorAsync(in int index, in Stream stream)
    {
        ValidateSectorIndex(index, nameof(index));

        stream.Position = index * Sector.Length;

        var sector = Sector switch
        {
            SectorCooked2048       => ISector.ReadAsync<SectorCooked2048>(stream),
            SectorCooked2324       => ISector.ReadAsync<SectorCooked2324>(stream),
            SectorCooked2336       => ISector.ReadAsync<SectorCooked2336>(stream),
            SectorRawAudio         => ISector.ReadAsync<SectorRawAudio>(stream),
            SectorRawMode0         => ISector.ReadAsync<SectorRawMode0>(stream),
            SectorRawMode1         => ISector.ReadAsync<SectorRawMode1>(stream),
            SectorRawMode2Form1    => ISector.ReadAsync<SectorRawMode2Form1>(stream),
            SectorRawMode2Form2    => ISector.ReadAsync<SectorRawMode2Form2>(stream),
            SectorRawMode2FormLess => ISector.ReadAsync<SectorRawMode2FormLess>(stream),
            _                      => throw new NotSupportedException(Sector.GetType().Name)
        };

        return sector;
    }

    private void ValidateSectorIndex(int index, string indexName) // TODO DRY
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