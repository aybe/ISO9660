namespace ISO9660.Tests.WorkInProgress;

public unsafe struct SectorMode2Form2 : ISector, ISectorHeader
{
    public fixed byte Sync[ISector.SyncSize];

    public fixed byte Header[ISector.HeaderSize];

    public fixed byte SubHeader[ISector.SubHeaderSizeMode2Form2];

    public fixed byte UserData[ISector.UserDataSizeMode2Form2];

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

    public readonly int GetUserDataPosition()
    {
        return ISector.UserDataPositionMode2Form2;
    }

    SectorHeader ISectorHeader.Header => ISector.GetHeader(ref this, ISector.HeaderPosition, ISector.HeaderSize);
}