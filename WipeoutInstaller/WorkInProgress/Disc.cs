using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.WorkInProgress;

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