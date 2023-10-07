using System;
using System.Runtime.InteropServices;

namespace CDROMTools
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 2352)]
    public struct SectorReserved : ISector
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2352)]
        public readonly byte[] Data;

        public byte[] GetUserData()
        {
            return Data;
        }
    }
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 2352)]
    public struct SectorAudio : ISector
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2352)]
        public readonly byte[] Data;

        public byte[] GetUserData()
        {
            return Data;
        }

        public short[] ToInt16()
        {
            int length = Data.Length / 2;
            var shorts = new short[length];
            for (int i = 0; i < length; i++)
            {
                var int16 = BitConverter.ToInt16(Data, i * 2);
                shorts[i] = int16;
            }
            return shorts;
        }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 2352)]
    public struct SectorMode1 : ISector
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public readonly byte[] Sync;

        [FieldOffset(12)]
        public readonly SectorHeader Header;

        [FieldOffset(16)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2048)]
        public readonly byte[] UserData;

        [FieldOffset(2064)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly byte[] Edc;

        [FieldOffset(2068)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public readonly byte[] Intermediate;

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