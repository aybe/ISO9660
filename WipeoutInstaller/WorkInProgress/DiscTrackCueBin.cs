namespace WipeoutInstaller.WorkInProgress;

internal sealed class DiscTrackCueBin : DiscTrack
{
    private const int PreGapSize = 150;

    public DiscTrackCueBin(CueSheetTrack track)
    {
        Track  = track;
        Stream = File.OpenRead(track.File.Name);
    }

    private CueSheetTrack Track { get; }

    public override int Index => Track.Index;

    public override int Length => GetLength(Track);

    public override int Position => GetPosition(Track);

    public override DiscTrackType Type
    {
        get
        {
            var source = Track.Type;

            var target = source switch // TODO implement other track types
            {
                CueSheetTrackType.Audio    => DiscTrackType.Audio,
                CueSheetTrackType.Mode1Raw => DiscTrackType.Data,
                CueSheetTrackType.Mode2Raw => DiscTrackType.Data,
                _                          => throw new NotSupportedException(source.ToString())
            };

            return target;
        }
    }

    protected override void DisposeManaged()
    {
        Stream.Dispose();
    }

    public override ISector ReadSector(in int index)
    {
        if (index < 0 || index >= Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, null);
        }

        Stream.Position = index * ISector.Size;

        var type = Track.Type;

        var sector = type switch // TODO implement other track types
        {
            CueSheetTrackType.Mode1Raw => ISector.Read<SectorMode1>(Stream),
            CueSheetTrackType.Mode2Raw => ISector.Read<SectorMode2Form1>(Stream),
            _                          => throw new NotSupportedException($"Track mode not supported: {type}.")
        };

        return sector;
    }

    private static int GetLength(CueSheetTrack track)
    {
        var lengthStream = Convert.ToInt32(new FileInfo(track.File.Name).Length / ISector.Size);

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

                var pos1 = GetPosition(value);
                var pos2 = next != null ? GetPosition(next.Value) : lengthStream;

                length = pos2 - pos1;

                var a = value.Type is not CueSheetTrackType.Audio &&
                        prev != null && prev.Value.Type != value.Type; // 6.32.3.18 SCSI MMC-5

                var b = value.Type is not CueSheetTrackType.Audio &&
                        next != null && next.Value.Type != value.Type; // 6.32.3.19 SCSI MMC-5

                var c = next != null && value.Index0 != null; // more implicit convention crap

                if (a || b)
                {
                    length -= PreGapSize;
                }

                if (c)
                {
                    length -= PreGapSize;
                }

                if (value == track)
                {
                    break;
                }
            }
        }
        else
        {
            length = lengthStream - PreGapSize - track.Index1.Position.ToLBA();
        }

        return length;
    }

    private static int GetPosition(CueSheetTrack track)
    {
        var files = track.File.Sheet.Files;

        var tracks = new LinkedList<CueSheetTrack>(files.SelectMany(s => s.Tracks));

        var position = -PreGapSize; // MSF 00:00.00 is LBA -150

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

                position += Convert.ToInt32(new FileInfo(value.File.Name).Length / ISector.Size);
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
                position += PreGapSize;
            }

            if (value == track)
            {
                break;
            }
        }

        return position;
    }
}