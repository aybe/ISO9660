namespace ISO9660.Physical;

internal sealed class TrackRaw : Track
{
    public TrackRaw(int index, int position, int length, bool audio)
    {
        Index = index;

        Position = position;

        Length = length;

        Audio = audio;

        throw new NotImplementedException(); // TODO Sector
    }

    public override int Index { get; }

    public override int Position { get; }

    public override int Length { get; }

    public override bool Audio { get; }

    public override ISector Sector { get; }

    protected override ValueTask DisposeAsyncCore()
    {
        throw new NotImplementedException();
    }

    protected override void DisposeManaged()
    {
        throw new NotImplementedException();
    }

    public override ISector ReadSector(in int index)
    {
        throw new NotImplementedException();
    }

    public override Task<ISector> ReadSectorAsync(in int index)
    {
        throw new NotImplementedException();
    }
}