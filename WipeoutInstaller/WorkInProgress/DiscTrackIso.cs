namespace WipeoutInstaller.WorkInProgress;

public sealed class DiscTrackIso : DiscTrack
{
    public DiscTrackIso(Stream stream, int index, int position)
    {
        ISector[] sectors =
        {
            new SectorCooked2048(),
            new SectorCooked2324(),
            new SectorCooked2336()
        };

        Sector = sectors.Single(s => stream.Length % s.GetUserDataLength() == 0);

        Length = Convert.ToInt32(stream.Length / Sector.Size);

        Stream = stream;

        Index = index;

        Position = position;
    }

    private Stream Stream { get; }

    public override bool Audio { get; } = false;

    public override int Index { get; }

    public override int Length { get; }

    public override int Position { get; }

    public override ISector Sector { get; }

    public override ISector ReadSector(in int index)
    {
        var size = Sector.Size;

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