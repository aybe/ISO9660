using System.Text;
using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.WorkInProgress;

public abstract class DiscTrack : Disposable
{
    public abstract bool Audio { get; }

    public abstract int Index { get; }

    public abstract int Length { get; }

    public abstract int Position { get; }

    public BinaryReader GetBinaryReader(in int sector)
    {
        if (sector < Position || sector >= Position + Length)
        {
            throw new ArgumentOutOfRangeException(nameof(sector), sector, null);
        }

        return new BinaryReader(new DiscTrackStream(this, sector), Encoding.Default, true);
    }

    public abstract ISector GetSector();

    public int GetSectorSize() // TODO this kinda sucks but that will do for now...
    {
        var sector = GetSector();

        var sectorSize = sector.GetSize();

        return sectorSize;
    }

    public abstract ISector ReadSector(in int index);

    public override string ToString()
    {
        return $"{nameof(Index)}: {Index}, {nameof(Position)}: {Position}, {nameof(Length)}: {Length}, {nameof(Audio)}: {Audio}";
    }
}