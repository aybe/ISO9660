namespace ISO9660.Tests.WorkInProgress;

public unsafe struct SectorMode1 : ISector, ISectorHeader
{
    public fixed byte Sync[ISector.SyncSize];

    public fixed byte Header[ISector.HeaderSize];

    public fixed byte UserData[ISector.UserDataSizeMode1];

    public fixed byte Edc[ISector.EdcSize];

    public fixed byte Intermediate[ISector.IntermediateSizeMode1];

    public fixed byte PParity[ISector.PParitySizeMode1];

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

    public readonly int GetUserDataPosition()
    {
        return ISector.UserDataPositionMode1;
    }

    SectorHeader ISectorHeader.Header => ISector.GetHeader(ref this, ISector.HeaderPosition, ISector.HeaderSize);
}