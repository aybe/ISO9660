using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace WipeoutInstaller.XA;

[UsedImplicitly]
[StructLayout(LayoutKind.Sequential, Size = Size)]
[SuppressMessage("ReSharper", "UnassignedField.Global")]
[SuppressMessage("ReSharper", "UnassignedReadonlyField")]
public unsafe struct SectorMode2
{
    public const int SyncPosition = 0;

    public const int HeaderPosition = SyncPosition + SyncSize;

    public const int HeaderSize = 4;

    public const int SubHeaderPosition = HeaderPosition + HeaderSize;

    public const int SubHeaderSize = 8;

    public const int UserDataPosition = SubHeaderPosition + SubHeaderSize;

    public const int Size = 2352;

    public const int SyncSize = 12;

    public const int UserDataSize = 2328;

    public fixed byte Sync[SyncSize];

    public SectorHeader Header;

    public SectorSubHeaderBlock SubHeaderBlock;

    public fixed byte UserData[UserDataSize];

    public readonly override string ToString()
    {
        return $"{nameof(Header)}: {Header}";
    }

    public byte[] GetSync()
    {
        fixed (byte* b = Sync)
        {
            return ToArray(b, SyncSize);
        }
    }

    public byte[] GetUserData()
    {
        fixed (byte* b = UserData)
        {
            return ToArray(b, UserDataSize);
        }
    }

    private static byte[] ToArray(byte* data, int size)
    {
        return new Span<byte>(data, size).ToArray();
    }
}