using ISO9660.GoldenHawk;
using Whatever.Extensions;

namespace ISO9660.Physical;

internal sealed class TrackCue : TrackFileBase
{
    public TrackCue(CueSheetTrack track)
    {
        var sector = ISector.GetSectorTypeCue(track.Type);

        Audio = track.Type == CueSheetTrackType.Audio;

        Index = track.Index;

        Length = GetCueLength(track, sector.Length);

        Position = GetCuePosition(track, sector.Length);

        Stream = File.OpenRead(track.File.Name);

        Sector = sector;
    }

    private static int GetCueLength(CueSheetTrack track, int sectorSize)
    {
        var lengthStream = (new FileInfo(track.File.Name).Length / sectorSize).ToInt32();

        var length = 0;

        var files = track.File.Sheet.Files;

        if (files.Count == 1)
        {
            var tracks = new LinkedList<CueSheetTrack>(files.SelectMany(s => s.Tracks));

            for (var node = tracks.First; node != null; node = node.Next)
            {
                var prev = node.Previous;
                var next = node.Next;

                var value = node.Value;

                var pos1 = GetCuePosition(value, sectorSize);
                var pos2 = next != null ? GetCuePosition(next.Value, sectorSize) : lengthStream;

                length = pos2 - pos1;

                var a = value.Type is not CueSheetTrackType.Audio &&
                        prev != null && prev.Value.Type != value.Type; // 6.32.3.18 SCSI MMC-5

                var b = value.Type is not CueSheetTrackType.Audio &&
                        next != null && next.Value.Type != value.Type; // 6.32.3.19 SCSI MMC-5

                var c = next != null && value.Index0 != null; // more implicit convention crap

                if (a || b)
                {
                    length -= 150;
                }

                if (c)
                {
                    length -= 150;
                }

                if (value == track)
                {
                    break;
                }
            }
        }
        else
        {
            length = lengthStream - 150 - track.Index1.Position.ToLBA();
        }

        return length;
    }

    private static int GetCuePosition(CueSheetTrack track, int sectorSize)
    {
        var files = track.File.Sheet.Files;

        var tracks = new LinkedList<CueSheetTrack>(files.SelectMany(s => s.Tracks));

        var position = -150; // MSF 00:00.00 is LBA -150

        for (var node = tracks.First; node != null; node = node.Next)
        {
            var value = node.Value;

            if (files.Count == 1)
            {
                position = value.Indices[0].Position.ToLBA();

                if (value == track)
                {
                    break;
                }
            }
            else
            {
                if (value == track)
                {
                    break;
                }

                position += (new FileInfo(value.File.Name).Length / sectorSize).ToInt32();
            }
        }

        for (var node = tracks.First; node != null; node = node.Next) // ECMA-130 - 20.2 User Data Area
        {
            var value = node.Value;

            var a = value.Index is 1;
            var b = value.Index is not 1 && node.Previous is { } previous && previous.Value.Type != value.Type;
            var c = value.Type is not CueSheetTrackType.Audio && node.Next is { Value.Type: CueSheetTrackType.Audio };

            if (a || b || c)
            {
                position += 150;
            }

            if (value == track)
            {
                break;
            }
        }

        return position;
    }
}