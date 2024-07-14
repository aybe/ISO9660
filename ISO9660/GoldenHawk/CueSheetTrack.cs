using Whatever.Extensions;

namespace ISO9660.GoldenHawk;

public sealed class CueSheetTrack(CueSheetFile file, int index, CueSheetTrackType type)
    : CueSheetElement
{
    public CueSheetFile File { get; } = file;

    public int Index { get; } = index;

    public CueSheetTrackType Type { get; } = type;

    public CueSheetTrackIndex? Index0 => Indices.SingleOrDefault(s => s.Number is 0);

    public CueSheetTrackIndex Index1 => Indices.Single(s => s.Number is 1);

    public IList<CueSheetTrackIndex> Indices { get; } = new List<CueSheetTrackIndex>();

    public CueSheetTrackFlags Flags { get; set; } = CueSheetTrackFlags.None;

    public string? Title { get; set; }

    public string? Performer { get; set; }

    public MSF? PreGap { get; set; }

    public string? Isrc { get; set; }

    public override string ToString()
    {
        return $"{nameof(Index)}: {Index}, {nameof(Type)}: {Type}, {nameof(Indices)}: {Indices.Count}, {nameof(Flags)}: {Flags}, {nameof(PreGap)}: {PreGap}";
    }

    public int GetLength(int sectorSize)
    {
        var lengthStream = (new FileInfo(File.Name).Length / sectorSize).ToInt32();

        var length = 0;

        var files = File.Sheet.Files;

        if (files.Count == 1)
        {
            var tracks = new LinkedList<CueSheetTrack>(files.SelectMany(s => s.Tracks));

            for (var node = tracks.First; node != null; node = node.Next)
            {
                var prev = node.Previous;
                var next = node.Next;

                var value = node.Value;

                var pos1 = value.GetPosition(sectorSize);
                var pos2 = next != null ? next.Value.GetPosition(sectorSize) : lengthStream;

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

                if (value == this)
                {
                    break;
                }
            }
        }
        else
        {
            length = lengthStream - 150 - Index1.Position.ToLBA();
        }

        return length;
    }

    public int GetPosition(int sectorSize)
    {
        var files = File.Sheet.Files;

        var tracks = new LinkedList<CueSheetTrack>(files.SelectMany(s => s.Tracks));

        var position = -150; // MSF 00:00.00 is LBA -150

        for (var node = tracks.First; node != null; node = node.Next)
        {
            var value = node.Value;

            if (files.Count == 1)
            {
                position = value.Indices[0].Position.ToLBA();

                if (value == this)
                {
                    break;
                }
            }
            else
            {
                if (value == this)
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

            if (value == this)
            {
                break;
            }
        }

        return position;
    }
}