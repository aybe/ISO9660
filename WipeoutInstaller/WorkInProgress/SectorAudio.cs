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
}