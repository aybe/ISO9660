namespace ISO9660.Tests.WorkInProgress;

public unsafe struct SectorMode2FormLess : ISector, ISectorHeader
{
    [UsedImplicitly]
    public fixed byte Sync[ISector.SyncSize];

    [UsedImplicitly]
    public fixed byte Header[ISector.HeaderSize];

    public fixed byte UserData[ISector.UserDataSizeMode2FormLess];
    [UsedImplicitly]

    public Span<byte> AsByteSpan()
    {
        return ISector.AsByteSpan(ref this);
    }

    public Span<byte> GetUserData()
    {
        return ISector.GetSlice(ref this, ISector.UserDataPositionMode2FormLess, ISector.UserDataSizeMode2FormLess);
    }

    public readonly int GetUserDataLength()
    {
        return ISector.UserDataSizeMode2FormLess;
    }

    SectorHeader ISectorHeader.Header => ISector.GetHeader(ref this, ISector.HeaderPosition, ISector.HeaderSize);
}