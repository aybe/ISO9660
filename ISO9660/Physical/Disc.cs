using Whatever.Extensions;

namespace ISO9660.Physical;

internal sealed class Disc : DisposableAsync, IDisc
{
    public Disc(IReadOnlyList<ITrack> tracks)
    {
        Tracks = tracks;
    }

    public IReadOnlyList<ITrack> Tracks { get; }

    protected override async ValueTask DisposeAsyncCore()
    {
        foreach (var track in Tracks)
        {
            await track.DisposeAsync().ConfigureAwait(false);
        }
    }

    protected override void DisposeManaged()
    {
        foreach (var track in Tracks)
        {
            track.Dispose();
        }
    }
}