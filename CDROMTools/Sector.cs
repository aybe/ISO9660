using System.Runtime.InteropServices;

namespace CDROMTools
{
    /// <summary>
    /// Represents an 'non-typed' sector, not intended to be used directly.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 2352)]
    public struct Sector : ISector
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public readonly byte[] Sync;

        [FieldOffset(12)]
        public readonly SectorHeader Header;

        [FieldOffset(16)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2336)]
        public readonly byte[] UserData;

        public byte[] GetUserData()
        {
            return UserData;
        }
    }
}