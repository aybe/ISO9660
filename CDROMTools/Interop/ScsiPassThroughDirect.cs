using System;
using System.Runtime.InteropServices;

namespace CDROMTools.Interop
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 44)]
    public struct ScsiPassThroughDirect
    {
        public ScsiPassThroughDirect(byte cdbLength = 16)
        {
            Length = 0;
            ScsiStatus = 0;
            PathId = 0;
            TargetId = 0;
            Lun = 0;
            CdbLength = cdbLength;
            SenseInfoLength = 0;
            DataIn = 0;
            DataTransferLength = 0;
            TimeOutValue = 0;
            DataBuffer = new IntPtr();
            SenseInfoOffset = 0;
            Cdb = new byte[16];
        }

        public ushort Length;
        public byte ScsiStatus;
        public byte PathId;
        public byte TargetId;
        public byte Lun;
        public byte CdbLength;
        public byte SenseInfoLength;
        public byte DataIn;
        public uint DataTransferLength;
        public uint TimeOutValue;
        public IntPtr DataBuffer;
        public uint SenseInfoOffset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Cdb;
    }
}