namespace ISO9660.Physical;

internal sealed class TrackBin(bool audio, int index, int length, int position, ISector sector, Stream stream)
    : Track(audio, index, length, position, sector)
{
    private Stream Stream { get; } = stream;

    protected override async ValueTask DisposeAsyncCore()
    {
        await Stream.DisposeAsync().ConfigureAwait(false);
    }

    protected override void DisposeManaged()
    {
        Stream.Dispose();
    }

    public override ISector ReadSector(int index)
    {
        return ReadSector(index, Stream);
    }

    public override Task<ISector> ReadSectorAsync(int index)
    {
        return ReadSectorAsync(index, Stream);
    }
}