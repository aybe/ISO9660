namespace ISO9660.Tests.WorkInProgress;

public unsafe struct SectorCooked2336 : ISector
{
    public const int UserDataPosition = 0;

    public const int UserDataSize = 2336;

    public fixed byte UserData[UserDataSize];

    public readonly int Size => 2336;

    public Span<byte> AsByteSpan()
    {
        return ISector.AsByteSpan(ref this);
    }

    public Span<byte> GetUserData()
    {
        return ISector.GetSlice(ref this, UserDataPosition, UserDataSize);
    }

    public readonly int GetUserDataLength()
    {
        return UserDataSize;
    }
}