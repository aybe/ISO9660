namespace WipeoutInstaller.WorkInProgress;

public unsafe struct SectorCooked2324 : ISector
{
    public const int UserDataPosition = 0;

    public const int UserDataSize = 2324;

    public fixed byte UserData[UserDataSize];

    readonly uint ISector.GetEdc()
    {
        return 0;
    }

    readonly uint ISector.GetEdcSum()
    {
        return 0;
    }

    public readonly int GetSize()
    {
        return 2324;
    }

    Span<byte> ISector.GetUserData()
    {
        return ISector.GetSlice(ref this, UserDataPosition, UserDataSize);
    }

    public readonly int GetUserDataLength()
    {
        return UserDataSize;
    }

    public readonly int GetUserDataPosition()
    {
        return UserDataPosition;
    }
}