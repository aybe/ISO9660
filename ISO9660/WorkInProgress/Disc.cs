using ISO9660.Extensions;

namespace ISO9660.WorkInProgress;

public sealed class Disc : Disposable
{
    public IList<DiscTrack> Tracks { get; } = new List<DiscTrack>();

    protected override void DisposeManaged()
    {
        foreach (var track in Tracks)
        {
            track.Dispose();
        }
    }
}