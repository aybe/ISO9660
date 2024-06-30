using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace ISO9660.Physical;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 2352)]
public unsafe struct SectorRawMode1 : ISector
{
    private const int UserDataLength = 2048;

    private const int UserDataPosition = 16;

    [FieldOffset(0)]
    [UsedImplicitly]
    public fixed byte Sync[12];

    [FieldOffset(12)]
    [UsedImplicitly]
    public fixed byte Header[4];

    [FieldOffset(16)]
    [UsedImplicitly]
    public fixed byte UserData[UserDataLength];

    [FieldOffset(2064)]
    [UsedImplicitly]
    public fixed byte Edc[4];

    [FieldOffset(2068)]
    [UsedImplicitly]
    public fixed byte Intermediate[8];

    [FieldOffset(2076)]
    [UsedImplicitly]
    public fixed byte PParity[172];

    [FieldOffset(2248)]
    [UsedImplicitly]
    public fixed byte QParity[104];

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