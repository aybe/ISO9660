using System.Diagnostics.CodeAnalysis;
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

        Sector = sectors.Single(s => stream.Length % s.GetUserDataLength() == 0);

        Length = (stream.Length / Sector.Length).ToInt32();

        Stream = stream;

        Index = index;

        Position = position;
    }

    private Stream Stream { get; }

    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public override bool Audio { get; }

    public override int Index { get; }

    public override int Length { get; }

    public override int Position { get; }

    public override ISector Sector { get; }

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

    public override Task<ISector> ReadSectorAsync(in int index)
    {
        return ReadSectorAsync(index, Stream);
    }
}