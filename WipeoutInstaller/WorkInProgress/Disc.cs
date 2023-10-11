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
    {
        result = default;

        var track = Tracks.FirstOrDefault();

        if (track?.Type is not DiscTrackType.Data)
        {
            return false;
        }

        try
        {
            result = new IsoImage(track.Stream);
        }
        catch (Exception)
        {
            // ignored
        }

        return result != null;
    }
}