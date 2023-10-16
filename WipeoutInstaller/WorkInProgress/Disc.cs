using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.WorkInProgress;

public sealed class Disc : Disposable
{
    public List<DiscTrack> Tracks { get; } = new();

    protected override void DisposeManaged()
    {
        foreach (var track in Tracks)
        {
            track.Dispose();
        }
    }

    public ISector ReadSector(in int index)
    {
        foreach (var track in Tracks)
        {
            if (index < track.Position)
            {
                continue;
            }

            var sector = track.ReadSector(index);

            return sector;
        }

        throw new ArgumentOutOfRangeException(nameof(index), index, null);
    }
}