namespace ISO9660.Tests.WorkInProgress;

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

    protected override void DisposeManaged()
    {
        Stream.Dispose();
    }

    public override ISector ReadSector(in uint index)
    {
        var size = Sector.Size;

        var position = index * size;

        Stream.Position = position;

        var sector = true switch
        {
            true when size == new SectorCooked2048().Size => ISector.Read<SectorCooked2048>(Stream),
            true when size == new SectorCooked2324().Size => ISector.Read<SectorCooked2324>(Stream),
            true when size == new SectorCooked2336().Size => ISector.Read<SectorCooked2336>(Stream),
            _                                             => throw new NotSupportedException()
        };

        return sector;
    }
}