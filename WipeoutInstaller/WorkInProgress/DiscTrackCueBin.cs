namespace WipeoutInstaller.WorkInProgress;

internal sealed class DiscTrackCueBin : DiscTrack
{
    public DiscTrackCueBin(string directory, CueSheetFile file, CueSheetTrack track)
    {
        TypeInternal = track.Type;
        Index        = track.Index;
        Position     = track.Indices.Single(s => s.Number is 1).Position;
        Type         = GetTrackType(track.Type);
        Stream       = File.OpenRead(Path.Combine(directory, file.Name));
    }

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
}