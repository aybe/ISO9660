using JetBrains.Annotations;

namespace ISO9660.WorkInProgress;

public unsafe struct SectorRawMode2Form2 : ISector
{
    private const int UserDataLength = 2324;

    private const int UserDataPosition = 24;

    [UsedImplicitly]
    public fixed byte Sync[12];

    [UsedImplicitly]
    public fixed byte Header[4];

    [UsedImplicitly]
    public fixed byte SubHeader[8];

    [UsedImplicitly]
    public fixed byte UserData[UserDataLength];

    [UsedImplicitly]
    public fixed byte ReservedOrEdc[4];

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