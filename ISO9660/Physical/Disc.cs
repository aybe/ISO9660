using System.Diagnostics.CodeAnalysis;
using ISO9660.GoldenHawk;
using Whatever.Extensions;

namespace ISO9660.Physical;

public sealed partial class Disc : DisposableAsync
{
    public IList<Track> Tracks { get; } = new List<Track>();

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

public sealed partial class Disc
{
    public static Disc FromCue(string path)
    {
        var sheet = CueSheetParser.Parse(path);

        var cueSheet = FromCueSheet(sheet);

        return cueSheet;
    }

    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
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

    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
    public static Disc FromIso(string path)
    {
        var stream = File.OpenRead(path);

        var track = new TrackIso(stream, 1, 0);

        var disc = new Disc();

        disc.Tracks.Add(track);

        return disc;
    }
}