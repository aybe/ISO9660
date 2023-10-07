using System.Runtime.InteropServices;

namespace CDROMTools.Interop
{
    [StructLayout(LayoutKind.Explicit)]
    public struct LargeInteger
    {
        [FieldOffset(0)] public LargeIntegerPart Part1;
        [FieldOffset(0)] public LargeIntegerPart Part2;
        [FieldOffset(0)] public long QuadPart;

        public override string ToString()
        {
            return $"QuadPart: {QuadPart}";
        }
    }
}