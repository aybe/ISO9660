using ISO9660.GoldenHawk;
using Whatever.Extensions;

namespace ISO9660.Physical;

internal sealed class TrackFileCue : TrackFileBase
{
    public TrackFileCue(CueSheetTrack track)
    {
        var sector = ISector.GetSectorTypeCue(track.Type);

        Audio = track.Type == CueSheetTrackType.Audio;

        Index = track.Index;

        Length = track.GetLength(sector.Length);

        Position = track.GetPosition(sector.Length);

        Stream = File.OpenRead(track.File.Name);

        Sector = sector;
    }
}