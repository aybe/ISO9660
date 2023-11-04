using ISO9660.CDRWIN;
using Whatever.Extensions;

namespace ISO9660.Media;

public sealed partial class Disc : Disposable
{
    public IList<Track> Tracks { get; } = new List<Track>();

    protected override void DisposeManaged()
    {
        foreach (var track in Tracks)
        {
            track.Dispose();
        }
    }
}

public sealed partial class Disc
{
    public static Disc FromCue(string path)
    {
        var sheet = CueSheetParser.Parse(path);

        var cueSheet = FromCueSheet(sheet);

        return cueSheet;
    }

    public static Disc FromCueSheet(CueSheet sheet)
    {
        var disc = new Disc();

        foreach (var file in sheet.Files)
        {
            foreach (var track in file.Tracks)
            {
                disc.Tracks.Add(new TrackCue(track));
            }
        }

        return disc;
    }

    public static Disc FromIso(string path)
    {
        var stream = File.OpenRead(path);

        var track = new TrackIso(stream, 1, 0);

        var disc = new Disc();

        disc.Tracks.Add(track);

        return disc;
    }
}