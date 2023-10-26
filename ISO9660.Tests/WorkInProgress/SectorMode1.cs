namespace ISO9660.Tests.WorkInProgress;

public unsafe struct SectorMode1 : ISector, ISectorHeader
{
    [UsedImplicitly]
    public fixed byte Sync[ISector.SyncSize];

    [UsedImplicitly]
    public fixed byte Header[ISector.HeaderSize];

    public fixed byte UserData[ISector.UserDataSizeMode1];
    [UsedImplicitly]

    [UsedImplicitly]
    public fixed byte Edc[ISector.EdcSize];

    [UsedImplicitly]
    public fixed byte Intermediate[ISector.IntermediateSizeMode1];

    [UsedImplicitly]
    public fixed byte PParity[ISector.PParitySizeMode1];

    [UsedImplicitly]
    public fixed byte QParity[ISector.QParitySizeMode1];

    public Span<byte> AsByteSpan()
    {
        return ISector.AsByteSpan(ref this);
    }

    public Span<byte> GetUserData()
    {
        return ISector.GetSlice(ref this, ISector.UserDataPositionMode1, ISector.UserDataSizeMode1);
    }

    public readonly int GetUserDataLength()
    {
        return ISector.UserDataSizeMode1;
    }

    SectorHeader ISectorHeader.Header => ISector.GetHeader(ref this, ISector.HeaderPosition, ISector.HeaderSize);
}