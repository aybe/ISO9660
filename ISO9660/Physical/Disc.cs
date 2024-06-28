using System.Collections.ObjectModel;
using ISO9660.GoldenHawk;
using Whatever.Extensions;

namespace ISO9660.Physical;

public sealed partial class Disc : DisposableAsync, IDisc
{
    private Disc(IReadOnlyList<ITrack> tracks)
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
        var tracks = sheet.Files.SelectMany(s => s.Tracks).Select(s => new TrackCue(s)).ToList().AsReadOnly();

        var disc = new Disc(tracks);

        return disc;
    }

    public static Disc FromIso(string path)
    {
        var stream = File.OpenRead(path);

        var track = new TrackIso(stream, 1, 0);

        var disc = new Disc(new ReadOnlyObservableCollection<ITrack>([track]));

        return disc;
    }
}