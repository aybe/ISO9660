using System.Runtime.InteropServices;

namespace CDROMTools.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LargeIntegerPart
    {
        public uint LowPart;
        public int HighPart;

        public override string ToString()
        {
            return $"LowPart: {LowPart}, HighPart: {HighPart}";
        }
    }
}