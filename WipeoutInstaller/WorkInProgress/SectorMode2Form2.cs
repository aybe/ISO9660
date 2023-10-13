namespace WipeoutInstaller.WorkInProgress;

public unsafe struct SectorMode2Form2 : ISector, ISectorHeader
{
    public fixed byte Sync[ISector.SyncSize];

    public fixed byte Header[ISector.HeaderSize];

    public fixed byte SubHeader[ISector.SubHeaderSizeMode2Form2];

    public fixed byte UserData[ISector.UserDataSizeMode2Form2];

    public fixed byte ReservedOrEdc[ISector.EdcSize];

    public uint GetEdc()
    {
        return ISector.ReadUInt32LE(ref this, ISector.EdcPositionMode2Form2);
    }

    public uint GetEdcSum()
    {
        return ISector.GetEdcSum(ref this, ISector.SubHeaderPositionMode2Form2, ISector.SubHeaderSizeMode2Form2 + ISector.UserDataSizeMode2Form2);
    }

    public Span<byte> GetUserData()
    {
        return ISector.GetSlice(ref this, ISector.UserDataPositionMode2Form2, ISector.UserDataSizeMode2Form2);
    }

    SectorHeader ISectorHeader.Header => ISector.GetHeader(ref this, ISector.HeaderPosition, ISector.HeaderSize);
}