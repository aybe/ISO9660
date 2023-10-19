namespace WipeoutInstaller.WorkInProgress;

public unsafe struct SectorMode2FormLess : ISector, ISectorHeader
{
    public fixed byte Sync[ISector.SyncSize];

    public fixed byte Header[ISector.HeaderSize];

    public fixed byte UserData[ISector.UserDataSizeMode2FormLess];

    public Span<byte> AsByteSpan()
    {
        return ISector.AsByteSpan(ref this);
    }

    public uint GetEdc()
    {
        throw new NotSupportedException();
    }

    public uint GetEdcSum()
    {
        throw new NotSupportedException();
    }

    public Span<byte> GetUserData()
    {
        return ISector.GetSlice(ref this, ISector.UserDataPositionMode2FormLess, ISector.UserDataSizeMode2FormLess);
    }

    public readonly int GetUserDataLength()
    {
        return ISector.UserDataSizeMode2FormLess;
    }

    public readonly int GetUserDataPosition()
    {
        return ISector.UserDataPositionMode2FormLess;
    }

    SectorHeader ISectorHeader.Header => ISector.GetHeader(ref this, ISector.HeaderPosition, ISector.HeaderSize);
}