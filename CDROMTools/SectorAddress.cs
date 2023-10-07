using System.Runtime.InteropServices;

namespace CDROMTools
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 3)]
    public struct SectorAddress
    {
        public readonly byte Min;
        public readonly byte Sec;
        public readonly byte Frame;

        public override string ToString()
        {
            return $"{Min:D2}:{Sec:D2}.{Frame:D2}";
        }
    }
}