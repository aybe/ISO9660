using System.Runtime.InteropServices;

namespace CDROMTools
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 2352)]
    public struct SectorMode2Formless : ISector
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