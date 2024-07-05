using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace ISO9660.Physical;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 2324)]
public unsafe struct SectorCooked2324 : ISector
{
    private const int UserDataLength = 2324;

    private const int UserDataPosition = 0;

    [FieldOffset(0)]
    [UsedImplicitly]
    public fixed byte UserData[UserDataLength];

    public readonly int Length => 2324;

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