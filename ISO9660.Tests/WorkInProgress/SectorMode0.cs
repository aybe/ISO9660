namespace ISO9660.Tests.WorkInProgress;

public unsafe struct SectorMode0 : ISector, ISectorHeader
{
    public fixed byte Sync[ISector.SyncSize];

    public fixed byte Header[ISector.HeaderSize];

    public fixed byte UserData[ISector.UserDataSizeMode0];

    public Span<byte> AsByteSpan()
    {
        return ISector.AsByteSpan(ref this);
    }

    public Span<byte> GetUserData()
    {
        return ISector.GetSlice(ref this, ISector.UserDataPositionMode0, ISector.UserDataSizeMode0);
    }

    public readonly int GetUserDataLength()
    {
        return ISector.UserDataSizeMode0;
    }

    SectorHeader ISectorHeader.Header => ISector.GetHeader(ref this, ISector.HeaderPosition, ISector.HeaderSize);
}