using JetBrains.Annotations;

namespace ISO9660.Tests.WorkInProgress;

public unsafe struct SectorMode0 : ISector
{
    
    private const int UserDataLength = 2336;
    private const int UserDataPosition = 16;

    [UsedImplicitly]
    public fixed byte Sync[ISector.SyncSize];

    [UsedImplicitly]
    public fixed byte Header[ISector.HeaderSize];

    [UsedImplicitly]
    public fixed byte UserData[UserDataLength];

    public Span<byte> AsByteSpan()
    {
        return ISector.AsByteSpan(ref this);
    }

    public Span<byte> GetUserData()
    {
        return ISector.GetSlice(ref this, UserDataPosition, UserDataLength);
    }

    public readonly int GetUserDataLength()
    {
        return UserDataLength;
    }
}