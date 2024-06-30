using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace ISO9660.Physical;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 2352)]
public unsafe struct SectorCooked2336 : ISector
{
    private const int UserDataLength = 2336;

    private const int UserDataPosition = 0;

    [FieldOffset(0)]
    [UsedImplicitly]
    public fixed byte UserData[UserDataLength];

    public readonly int Length => 2336;

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