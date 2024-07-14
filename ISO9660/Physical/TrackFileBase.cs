namespace ISO9660.Physical;

/// <summary>
///     Base class for a track read from a file.
/// </summary>
internal abstract class TrackFileBase : Track
{
    protected Stream Stream { get; init; } = null!;

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