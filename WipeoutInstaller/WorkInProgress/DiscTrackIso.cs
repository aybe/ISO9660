namespace WipeoutInstaller.WorkInProgress;

public sealed class DiscTrackIso : DiscTrack
{
    public DiscTrackIso(Stream stream)
    {
        Stream = stream;
    }

    private Stream Stream { get; }

    public override bool Audio { get; } = false;

    public override int Index { get; } = 1;

    public override int Length
    {
        get
        {
            var size = GetSectorSize();

            var length = Stream.Length / size;

            return Convert.ToInt32(length);
        }
    }

    public override int Position { get; } = 0;

    private int GetSectorSize()
    {
        var length = Stream.Length;

        const int size2048 = SectorCooked2048.UserDataSize;
        const int size2324 = SectorCooked2324.UserDataSize;
        const int size2336 = SectorCooked2336.UserDataSize;

        var size = true switch
        {
            true when length % size2048 is 0 => size2048,
            true when length % size2324 is 0 => size2324,
            true when length % size2336 is 0 => size2336,
            _                                => throw new NotSupportedException()
        };

        return size;
    }

    public override ISector ReadSector(in int index)
    {
        var size = GetSectorSize();

        var position = index * size;

        Stream.Position = position;

        var sector = size switch
        {
            SectorCooked2048.UserDataSize => ISector.Read<SectorCooked2048>(Stream),
            SectorCooked2324.UserDataSize => ISector.Read<SectorCooked2324>(Stream),
            SectorCooked2336.UserDataSize => ISector.Read<SectorCooked2336>(Stream),
            _                             => throw new NotSupportedException()
        };

        return sector;
    }
}