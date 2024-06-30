using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace ISO9660.Physical;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 2352)]
public unsafe struct SectorRawMode0 : ISector
{
    private const int UserDataLength = 2336;

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