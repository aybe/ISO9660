using System.Runtime.InteropServices;

namespace CDROMTools.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct OverlappedOffset
    {
        public uint Offset;
        public uint OffsetHigh;
    }
}