namespace WipeoutInstaller.WorkInProgress;

public unsafe struct SectorAudio : ISector
{
    public fixed byte Data[ISector.UserDataSizeAudio];

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
        return ISector.GetSlice(ref this, ISector.UserDataPositionAudio, ISector.UserDataSizeAudio);
    }

    public readonly int GetUserDataLength()
    {
        return ISector.UserDataSizeAudio;
    }

    public readonly int GetUserDataPosition()
    {
        return ISector.UserDataPositionAudio;
    }
}