using System.Text;
using Whatever.Extensions;

namespace ISO9660.Physical;

public abstract class Track : Disposable
{
    public abstract bool Audio { get; }

    public abstract int Index { get; }

    public abstract int Length { get; }

    public abstract int Position { get; }

    public abstract ISector Sector { get; }

    public BinaryReader GetBinaryReader(in int sector)
    {
        if (sector < Position || sector >= Position + Length)
        {
            throw new ArgumentOutOfRangeException(nameof(sector), sector, null);
        }

        return new BinaryReader(new TrackStream(this, sector), Encoding.Default, true);
    }

    public abstract ISector ReadSector(in int index);

    protected ISector ReadSector(in int index, in Stream stream)
    {
        if (index < Position || index >= Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, null);
        }

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

    public override string ToString()
    {
        return $"{nameof(Index)}: {Index}, {nameof(Position)}: {Position}, {nameof(Length)}: {Length}, {nameof(Audio)}: {Audio}";
    }
}