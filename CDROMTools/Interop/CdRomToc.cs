using System.Runtime.InteropServices;

namespace CDROMTools.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct CdRomToc
    {
        public ushort Length; // + 2 ?????????
        public byte FirstTrack;
        public byte LastTrack;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.MAXIMUM_NUMBER_TRACKS)]
        public TrackData[] TrackData;
    }
}