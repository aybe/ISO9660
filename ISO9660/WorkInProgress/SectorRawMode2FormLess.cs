using JetBrains.Annotations;

namespace ISO9660.WorkInProgress;

public unsafe struct SectorRawMode2FormLess : ISector
{
    private const int UserDataLength = 2336;

    private const int UserDataPosition = 16;

    [UsedImplicitly]
    public fixed byte Sync[12];

    [UsedImplicitly]
    public fixed byte Header[4];

    [UsedImplicitly]
    public fixed byte UserData[UserDataLength];

    public readonly int Length => 2352;

    public Span<byte> GetData()
    {
        return ISector.GetSpan(ref this, 0, Length);
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