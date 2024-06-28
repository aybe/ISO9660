using ISO9660.GoldenHawk;
using System.Collections.ObjectModel;

namespace ISO9660.Physical;

public interface IDisc : IDisposable, IAsyncDisposable
{
    IReadOnlyList<ITrack> Tracks { get; }

    public static IDisc Open(string path)
    {
        var extension = Path.GetExtension(path);

        switch (extension.ToLowerInvariant())
        {
            case ".cue":
                return OpenCue(path);
            case ".iso":
                return OpenIso(path);
        }

        var info = new DriveInfo(path);

        if (info.DriveType is not DriveType.CDRom)
        {
            throw new ArgumentOutOfRangeException(nameof(path), path, "CD-ROM drive expected.");
        }

        if (OperatingSystem.IsWindows())
        {
            // TODO
        }

        throw new PlatformNotSupportedException();
    }

    private static IDisc OpenCue(string path)
    {
        var sheet = CueSheetParser.Parse(path);

        var tracks = sheet.Files.SelectMany(s => s.Tracks).Select(s => new TrackCue(s)).ToList().AsReadOnly();

        var disc = new Disc(tracks);

        return disc;
    }

    private static IDisc OpenIso(string path)
    {
        var stream = File.OpenRead(path);

        var track = new TrackIso(stream, 1, 0);

        var disc = new Disc(new ReadOnlyObservableCollection<ITrack>([track]));

        return disc;
    }
}