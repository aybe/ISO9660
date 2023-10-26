using JetBrains.Annotations;

namespace ISO9660.Tests.WorkInProgress;

public unsafe struct SectorMode2Form1 : ISector
{
    private const int UserDataLength = 2048;

    private const int UserDataPosition = 24;

    [UsedImplicitly]
    public fixed byte Sync[ISector.SyncSize];

    [UsedImplicitly]
    public fixed byte Header[ISector.HeaderSize];

    [UsedImplicitly]
    public fixed byte SubHeader[8];

    [UsedImplicitly]
    public fixed byte UserData[UserDataLength];

    [UsedImplicitly]
    public fixed byte Edc[4];

    [UsedImplicitly]
    public fixed byte PParity[172];

    [UsedImplicitly]
    public fixed byte QParity[104];

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