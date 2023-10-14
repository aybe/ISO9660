namespace WipeoutInstaller.WorkInProgress;

internal sealed class DiscTrackCueBin : DiscTrack
{
    public DiscTrackCueBin(Disc disc, string cueSheetDirectory, CueSheetFile cueSheetFile, CueSheetTrack cueSheetTrack)
        : base(disc)
    {
        CueSheetDirectory = cueSheetDirectory;
        CueSheetTrack     = cueSheetTrack;
        TypeInternal      = cueSheetTrack.Type;  // TODO inline
        Index             = cueSheetTrack.Index; // TODO inline
        Position          = cueSheetTrack.Indices.Single(s => s.Number is 1).Position;
        Type              = GetTrackType(cueSheetTrack.Type);
        Stream            = File.OpenRead(Path.Combine(cueSheetDirectory, cueSheetFile.Name));
    }

    private string CueSheetDirectory { get; }

    private CueSheetTrack CueSheetTrack { get; }

    private CueSheetTrackType TypeInternal { get; }

    protected override void DisposeManaged()
    {
        Stream.Dispose();
    }

    private static DiscTrackType GetTrackType(CueSheetTrackType trackType)
    {
        return trackType switch // TODO implement other track types
        {
            CueSheetTrackType.Audio    => DiscTrackType.Audio,
            CueSheetTrackType.Mode1Raw => DiscTrackType.Data,
            CueSheetTrackType.Mode2Raw => DiscTrackType.Data,
            _                          => throw new NotSupportedException(trackType.ToString())
        };
    }

    public override ISector ReadSector(int index)
    {
        if (index < 0 || index >= Stream.Length / ISector.Size)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, null);
        }


        Stream.Position = index * ISector.Size;

        return TypeInternal switch
        {
            CueSheetTrackType.Mode1Raw => ISector.Read<SectorMode1>(Stream),
            CueSheetTrackType.Mode2Raw => ISector.Read<SectorMode2Form1>(Stream),
            _                          => throw new NotSupportedException($"Track mode not supported: {TypeInternal}.")
        };
    }

    public override int GetPosition() // pretty complex non-sense
    {
        var files = CueSheetTrack.File.Sheet.Files;

        var list = new LinkedList<CueSheetTrack>(files.SelectMany(s => s.Tracks));

        var position = files.Count is 1 ? GetPositionSingleFile(list) : GetPositionMultiFile(list);

        position = GetPositionEcma130(list, position);

        return position;
    }

    private int GetPositionEcma130(LinkedList<CueSheetTrack> list, int position)
    {
        for (var node = list.First; node != null; node = node.Next)
        {
            var track = node.Value;

            var a = track.Index is 1;
            var b = track.Index is not 1 && node.Previous is { } previous && previous.Value.Type != track.Type;
            var c = track.Type is not CueSheetTrackType.Audio && node.Next is { Value.Type: CueSheetTrackType.Audio };

            if (a || b || c) // 20.2 User Data Area
            {
                position += 150;
            }

            if (track == CueSheetTrack)
            {
                break;
            }
        }

        return position;
    }

    private int GetPositionMultiFile(LinkedList<CueSheetTrack> list)
    {
        var position = -150; // pre-gap

        for (var node = list.First; node != null; node = node.Next)
        {
            var track = node.Value;

            if (track == CueSheetTrack)
            {
                break;
            }

            var path = Path.Combine(CueSheetDirectory, track.File.Name);
            var info = new FileInfo(path);
            var size = info.Length / ISector.Size;

            position += (int)size;
        }

        return position;
    }

    private int GetPositionSingleFile(LinkedList<CueSheetTrack> list)
    {
        var position = 0;

        for (var node = list.First; node != null; node = node.Next)
        {
            var track = node.Value;

            var index0 = track.Indices.SingleOrDefault(s => s.Number is 0);
            var index1 = track.Indices.Single(s => s.Number is 1);

            position = index1.Position.ToLBA(); // absolute position

            if (index0 is not null)
            {
                if (track.Index is not 1) // pre-gap
                {
                    position -= position - index0.Position.ToLBA();
                }
            }

            if (track == CueSheetTrack)
            {
                break;
            }
        }

        return position;
    }
}