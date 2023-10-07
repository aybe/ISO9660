using System.Runtime.InteropServices;
using CDROMTools.Utils;

namespace CDROMTools.Interop
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
    public struct TrackData
    {
        public byte Reserved;
        public byte ControlAdr;
        public byte TrackNumber;
        public byte Reserved1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public byte[] Address;

        public SubChannelQControlBits Control
        {
            get { return (SubChannelQControlBits) BitUtils.GetValue(ref ControlAdr, 0, 0xF); }
        }
  public AdrSubChannelQField Adr
        {
            get { return (AdrSubChannelQField) BitUtils.GetValue(ref ControlAdr, 4, 0xF); }
        }

     //   public uint Adr => (ControlAdr & 0xF0u)/16;

        public override string ToString()
        {
            return $"TrackNumber: {TrackNumber}";
        }
    }
}