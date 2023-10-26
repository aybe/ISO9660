namespace ISO9660.Tests.WorkInProgress;

public unsafe struct SectorAudio : ISector
{
    public fixed byte Data[ISector.UserDataSizeAudio];

    public Span<byte> AsByteSpan()
    {
        return ISector.AsByteSpan(ref this);
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