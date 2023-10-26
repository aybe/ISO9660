namespace ISO9660.Tests.WorkInProgress;

public unsafe struct SectorMode2Form1 : ISector, ISectorHeader
{
    [UsedImplicitly]
    public fixed byte Sync[ISector.SyncSize];

    [UsedImplicitly]
    public fixed byte Header[ISector.HeaderSize];

    [UsedImplicitly]
    public fixed byte SubHeader[ISector.SubHeaderSizeMode2Form1];

    public fixed byte UserData[ISector.UserDataSizeMode2Form1];
    [UsedImplicitly]

    [UsedImplicitly]
    public fixed byte Edc[ISector.EdcSize];

    [UsedImplicitly]
    public fixed byte PParity[ISector.PParitySizeMode2Form1];

    [UsedImplicitly]
    public fixed byte QParity[ISector.QParitySizeMode2Form1];

    public Span<byte> AsByteSpan()
    {
        return ISector.AsByteSpan(ref this);
    }

    public Span<byte> GetUserData()
    {
        return ISector.GetSlice(ref this, ISector.UserDataPositionMode2Form1, ISector.UserDataSizeMode2Form1);
    }

    public readonly int GetUserDataLength()
    {
        return ISector.UserDataSizeMode2Form1;
    }

    SectorHeader ISectorHeader.Header => ISector.GetHeader(ref this, ISector.HeaderPosition, ISector.HeaderSize);
}