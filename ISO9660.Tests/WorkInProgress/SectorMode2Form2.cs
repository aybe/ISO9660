namespace ISO9660.Tests.WorkInProgress;

public unsafe struct SectorMode2Form2 : ISector, ISectorHeader
{
    [UsedImplicitly]
    public fixed byte Sync[ISector.SyncSize];

    [UsedImplicitly]
    public fixed byte Header[ISector.HeaderSize];

    [UsedImplicitly]
    public fixed byte SubHeader[ISector.SubHeaderSizeMode2Form2];

    public fixed byte UserData[ISector.UserDataSizeMode2Form2];
    [UsedImplicitly]

    [UsedImplicitly]
    public fixed byte ReservedOrEdc[ISector.EdcSize];

    public Span<byte> AsByteSpan()
    {
        return ISector.AsByteSpan(ref this);
    }

    public Span<byte> GetUserData()
    {
        return ISector.GetSlice(ref this, ISector.UserDataPositionMode2Form2, ISector.UserDataSizeMode2Form2);
    }

    public readonly int GetUserDataLength()
    {
        return ISector.UserDataSizeMode2Form2;
    }

    SectorHeader ISectorHeader.Header => ISector.GetHeader(ref this, ISector.HeaderPosition, ISector.HeaderSize);
}