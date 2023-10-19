namespace WipeoutInstaller.WorkInProgress;

public unsafe struct SectorMode2Form1 : ISector, ISectorHeader
{
    public fixed byte Sync[ISector.SyncSize];

    public fixed byte Header[ISector.HeaderSize];

    public fixed byte SubHeader[ISector.SubHeaderSizeMode2Form1];

    public fixed byte UserData[ISector.UserDataSizeMode2Form1];

    public fixed byte Edc[ISector.EdcSize];

    public fixed byte PParity[ISector.PParitySizeMode2Form1];

    public fixed byte QParity[ISector.QParitySizeMode2Form1];

    public uint GetEdc()
    {
        return ISector.ReadUInt32LE(ref this, ISector.EdcPositionMode2Form1);
    }

    public uint GetEdcSum()
    {
        return ISector.GetEdcSum(ref this, ISector.SubHeaderPositionMode2Form1, ISector.SubHeaderSizeMode2Form1 + ISector.UserDataSizeMode2Form1);
    }

    public Span<byte> GetUserData()
    {
        return ISector.GetSlice(ref this, ISector.UserDataPositionMode2Form1, ISector.UserDataSizeMode2Form1);
    }

    public readonly int GetUserDataLength()
    {
        return ISector.UserDataSizeMode2Form1;
    }

    public readonly int GetUserDataPosition()
    {
        return ISector.UserDataPositionMode2Form1;
    }

    SectorHeader ISectorHeader.Header => ISector.GetHeader(ref this, ISector.HeaderPosition, ISector.HeaderSize);
}