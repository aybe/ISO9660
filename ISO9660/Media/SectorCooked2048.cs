using JetBrains.Annotations;

namespace ISO9660.Media;

public unsafe struct SectorCooked2048 : ISector
{
    private const int UserDataLength = 2048;

    private const int UserDataPosition = 0;

    [UsedImplicitly]
    public fixed byte UserData[UserDataLength];

    public readonly int Length => 2048;

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