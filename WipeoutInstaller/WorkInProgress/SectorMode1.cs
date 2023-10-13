namespace WipeoutInstaller.WorkInProgress;

public unsafe struct SectorMode1 : ISector, ISectorHeader
{
    public fixed byte Sync[ISector.SyncSize];

    public fixed byte Header[ISector.HeaderSize];

    public fixed byte UserData[ISector.UserDataSizeMode1];

    public fixed byte Edc[ISector.EdcSize];

    public fixed byte Intermediate[ISector.IntermediateSizeMode1];

    public fixed byte PParity[ISector.PParitySizeMode1];

    public fixed byte QParity[ISector.QParitySizeMode1];

    public uint GetEdc()
    {
        return ISector.ReadUInt32LE(ref this, ISector.EdcPositionMode1);
    }

    public uint GetEdcSum()
    {
        return ISector.GetEdcSum(ref this, ISector.SyncPosition, ISector.SyncSize + ISector.HeaderSize + ISector.UserDataSizeMode1);
    }

    public Span<byte> GetUserData()
    {
        return ISector.GetSlice(ref this, ISector.UserDataPositionMode1, ISector.UserDataSizeMode1);
    }

    SectorHeader ISectorHeader.Header => ISector.GetHeader(ref this, ISector.HeaderPosition, ISector.HeaderSize);
}