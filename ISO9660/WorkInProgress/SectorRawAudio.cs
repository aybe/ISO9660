using JetBrains.Annotations;

namespace ISO9660.WorkInProgress;

public unsafe struct SectorRawAudio : ISector
{
    private const int UserDataLength = 2352;

    private const int UserDataPosition = 0;

    [UsedImplicitly]
    public fixed byte UserData[UserDataLength];

    public readonly int Length => 2352;

    public Span<byte> GetData()
    {
        return ISector.GetSpan(ref this);
    }

    public Span<byte> GetUserData()
    {
        return ISector.GetSpan(ref this, UserDataPosition, UserDataLength);
    }

    public readonly int GetUserDataLength()
    {
        return UserDataLength;
    }
}