using System.Diagnostics.CodeAnalysis;
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

    public bool TryGetIso9660FileSystem([MaybeNullWhen(false)] out IsoImage result)
        // TODO this sucks, there should be extra methods Has... and TryRead...
    {
        result = default;

        var track = Tracks.FirstOrDefault();

        if (track?.Type is not DiscTrackType.Data)
        {
            return false;
        }

        result = new IsoImage(this);

        return true;
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