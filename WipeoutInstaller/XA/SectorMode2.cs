using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using WipeoutInstaller.WorkInProgress;

namespace WipeoutInstaller.XA;

[UsedImplicitly]
[StructLayout(LayoutKind.Sequential, Size = SectorConstants.Size)]
[SuppressMessage("ReSharper", "UnassignedField.Global")]
[SuppressMessage("ReSharper", "UnassignedReadonlyField")]
public unsafe struct SectorMode2
{
    public fixed byte Sync[SectorConstants.SyncSize];

    public SectorHeader Header;

    public SectorSubHeaderBlock SubHeaderBlock;

    public fixed byte UserData[SectorConstants.UserDataSize];

    public readonly override string ToString()
    {
        return $"{nameof(Header)}: {Header}";
    }

    public byte[] GetSync()
    {
        fixed (byte* b = Sync)
        {
            return ToArray(b, SectorConstants.SyncSize);
        }
    }

    public byte[] GetUserData()
    {
        fixed (byte* b = UserData)
        {
            return ToArray(b, SectorConstants.UserDataSize);
        }
    }

    private static byte[] ToArray(byte* data, int size)
    {
        return new Span<byte>(data, size).ToArray();
    }
}