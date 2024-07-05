using Whatever.Extensions;

namespace ISO9660.Physical;

internal sealed class TrackIso : Track
{
    public TrackIso(Stream stream, int index, int position)
    {
        ISector[] sectors =
        {
            new SectorCooked2048(),
            new SectorCooked2324(),
            new SectorCooked2336()
        };

        Audio = false;

        Sector = sectors.Single(s => stream.Length % s.GetUserDataLength() == 0);

        Length = (stream.Length / Sector.Length).ToInt32();

        Stream = stream;

        Index = index;

        Position = position;
    }

    private Stream Stream { get; }

    protected override async ValueTask DisposeAsyncCore()
    {
        await Stream.DisposeAsync().ConfigureAwait(false);
    }

    protected override void DisposeManaged()
    {
        Stream.Dispose();
    }

    public override ISector ReadSector(in int index)
    {
        return ReadSector(index, Stream);
    }

    public override Task<ISector> ReadSectorAsync(int index)
    {
        return ReadSectorAsync(index, Stream);
    }
}