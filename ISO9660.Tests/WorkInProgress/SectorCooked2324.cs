namespace ISO9660.Tests.WorkInProgress;

public unsafe struct SectorCooked2324 : ISector
{
    public const int UserDataPosition = 0;

    public const int UserDataSize = 2324;

    public fixed byte UserData[UserDataSize];

    public readonly int Size => 2324;

    public Span<byte> AsByteSpan()
    {
        return ISector.AsByteSpan(ref this);
    }

    Span<byte> ISector.GetUserData()
    {
        return ISector.GetSlice(ref this, UserDataPosition, UserDataSize);
    }

    public readonly int GetUserDataLength()
    {
        return UserDataSize;
    }
}