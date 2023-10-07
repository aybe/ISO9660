using System.Runtime.InteropServices;

namespace CDROMTools
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 2352)]
    public struct SectorMode2Form1 : ISector
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public readonly byte[] Sync;

        [FieldOffset(12)]
        public readonly SectorHeader Header;

        [FieldOffset(16)]
        public readonly SectorMode2Form1SubHeader SubHeaderCopy1;

        [FieldOffset(20)]
        public readonly SectorMode2Form1SubHeader SubHeaderCopy2;

        [FieldOffset(24)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2048)]
        public readonly byte[] UserData;

        [FieldOffset(2072)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly byte[] Edc;
        
        [FieldOffset(2076)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 172)]
        public readonly byte[] ParityP;

        [FieldOffset(2248)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 104)]
        public readonly byte[] ParityQ;
        public byte[] GetUserData()
        {
            return UserData;
        }

    }
}