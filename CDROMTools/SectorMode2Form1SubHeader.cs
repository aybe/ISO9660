using System.Runtime.InteropServices;

namespace CDROMTools
{
    [StructLayout(LayoutKind.Sequential,Pack = 1,Size = 4)]
    public struct SectorMode2Form1SubHeader
    {
        public byte FileNumber;
        public byte ChannelNumber;
        public SectorMode2Form1SubHeaderSubMode SubMode;
        public byte CodingInformation;

        public override string ToString()
        {
            return $"SubMode: {SubMode}";
        }
    }
}