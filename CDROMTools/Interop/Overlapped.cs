using System;
using System.Runtime.InteropServices;

namespace CDROMTools.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Overlapped
    {
        public uint Internal;
        public uint InternalHigh;
        public OverlappedOffsetShim OffsetShim;
        public IntPtr hEvent;
    }
}