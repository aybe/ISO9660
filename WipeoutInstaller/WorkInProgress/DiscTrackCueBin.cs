namespace WipeoutInstaller.WorkInProgress;

internal sealed class DiscTrackCueBin : DiscTrack
{
    public DiscTrackCueBin(string cueSheetDirectory, CueSheetFile cueSheetFile, CueSheetTrack cueSheetTrack)
    {
        CueSheetDirectory = cueSheetDirectory;
        CueSheetTrack     = cueSheetTrack;
        Stream            = File.OpenRead(Path.Combine(cueSheetDirectory, cueSheetFile.Name));
    }

    private string CueSheetDirectory { get; }

    private CueSheetTrack CueSheetTrack { get; }

    public override int Index => CueSheetTrack.Index;

    public override MSF Position => CueSheetTrack.Index1.Position;

    public override DiscTrackType Type
    {
        get
        {
            var source = CueSheetTrack.Type;

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
        if (index < 0 || index >= Stream.Length / ISector.Size)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, null);
        }

        Stream.Position = index * ISector.Size;

        var type = CueSheetTrack.Type;

        var sector = type switch // TODO implement other track types
        {
            CueSheetTrackType.Mode1Raw => ISector.Read<SectorMode1>(Stream),
            CueSheetTrackType.Mode2Raw => ISector.Read<SectorMode2Form1>(Stream),
            _                          => throw new NotSupportedException($"Track mode not supported: {type}.")
        };

        return sector;
    }

    public override int GetPosition() // pretty complex non-sense
    {
        return GetPosition(CueSheetTrack, CueSheetDirectory);
    }

    private static int GetPosition(CueSheetTrack track, string directory)
    {
        var files = track.File.Sheet.Files;

        var tracks = new LinkedList<CueSheetTrack>(files.SelectMany(s => s.Tracks));

        var position = files.Count is 1 ? GetPositionSingleFile(track, tracks) : GetPositionMultiFile(track, tracks, directory);

        position = GetPositionEcma130(track, tracks, position);

        return position;
    }

    private static int GetPositionEcma130(CueSheetTrack track, LinkedList<CueSheetTrack> tracks, int position)
    {
        for (var node = tracks.First; node != null; node = node.Next)
        {
            var value = node.Value;

            var a = value.Index is 1;
            var b = value.Index is not 1 && node.Previous is { } previous && previous.Value.Type != value.Type;
            var c = value.Type is not CueSheetTrackType.Audio && node.Next is { Value.Type: CueSheetTrackType.Audio };

            if (a || b || c) // 20.2 User Data Area
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

    private static int GetPositionMultiFile(CueSheetTrack track, LinkedList<CueSheetTrack> tracks, string directory)
    {
        var position = -150; // pre-gap

        for (var node = tracks.First; node != null; node = node.Next)
        {
            var value = node.Value;

            if (value == track)
            {
                break;
            }

            var path = Path.Combine(directory, value.File.Name);

            var info = new FileInfo(path);

            var bytes = info.Length;

            var sectors = bytes / ISector.Size;
            var length = Convert.ToInt32(sectors);

            position += length;
        }

        return position;
    }

    private static int GetPositionSingleFile(CueSheetTrack track, LinkedList<CueSheetTrack> tracks)
    {
        var position = 0;

        for (var node = tracks.First; node != null; node = node.Next)
        {
            var value = node.Value;

            var index0 = value.Index0;
            var index1 = value.Index1;

            position = index1.Position.ToLBA(); // absolute position

            if (index0 is not null)
            {
                if (value.Index is not 1) // pre-gap
                {
                    position -= position - index0.Position.ToLBA();
                }
            }

            if (value == track)
            {
                break;
            }
        }

        return position;
    }
}