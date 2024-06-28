using ISO9660.GoldenHawk;
using System.Collections.ObjectModel;

namespace ISO9660.Physical;

public interface IDisc : IDisposable, IAsyncDisposable
{
    IReadOnlyList<ITrack> Tracks { get; }

    public static IDisc FromCue(string path)
    {
        var sheet = CueSheetParser.Parse(path);

        var tracks = sheet.Files.SelectMany(s => s.Tracks).Select(s => new TrackCue(s)).ToList().AsReadOnly();

        var disc = new Disc(tracks);

        return disc;
    }

    public static IDisc FromIso(string path)
    {
        var stream = File.OpenRead(path);

        var track = new TrackIso(stream, 1, 0);

        var disc = new Disc(new ReadOnlyObservableCollection<ITrack>([track]));

        return disc;
    }
}