using System.Runtime.InteropServices;
using CDROMTools.Utils;

namespace CDROMTools
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
    public struct SectorHeader
    {
        public readonly SectorAddress Address;

        /// <summary>
        ///     Not intended to be used directly, use <see cref="Block" /> and <see cref="Mode" /> instead.
        /// </summary>
        public readonly byte RawFlags;

        public SectorBlock Block
        {
            get
            {
                var b = RawFlags;
                var block = BitUtils.GetValue(ref b, 5, 0x7);
                var sectorBlock = (SectorBlock) block;
                return sectorBlock;
            }
        }

        public SectorMode Mode
        {
            get
            {
                var b = RawFlags;
                var mode = BitUtils.GetValue(ref b, 0, 0x3);
                var sectorMode = (SectorMode) mode;
                return sectorMode;
            }
        }

        public override string ToString()
        {
            return $"Block: {Block}, Mode: {Mode}";
        }
    }
}