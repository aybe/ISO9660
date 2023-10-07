using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CDROMTools.Iso9660
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 17, CharSet = CharSet.Ansi)]
    public struct IsoLongDateTime
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly char[] Year;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public readonly char[] Month;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public readonly char[] Day;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public readonly char[] Hour;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public readonly char[] Minute;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public readonly char[] Second;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public readonly char[] SecondHundredths;

        public readonly Iso712 OffsetGMT;

        public IsoLongDateTime(BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            var ascii = Encoding.ASCII;
            Year = ascii.GetChars(reader.ReadBytes(4));
            Month = ascii.GetChars(reader.ReadBytes(2));
            Day = ascii.GetChars(reader.ReadBytes(2));
            Hour = ascii.GetChars(reader.ReadBytes(2));
            Minute = ascii.GetChars(reader.ReadBytes(2));
            Second = ascii.GetChars(reader.ReadBytes(2));
            SecondHundredths = ascii.GetChars(reader.ReadBytes(2));
            OffsetGMT = new Iso712(reader);
        }

        public override string ToString()
        {
            // for http://stackoverflow.com/questions/31562791/what-makes-the-visual-studio-debugger-stop-evaluating-a-tostring-override
            // and zero values
            // ...
            var year = int.Parse(new string(Year));
            var month = int.Parse(new string(Month));
            var day = int.Parse(new string(Day));
            var hour = int.Parse(new string(Hour));
            var min = int.Parse(new string(Minute));
            var sec = int.Parse(new string(Second));
            var ms = int.Parse(new string(SecondHundredths)) * 10;
            var meridian = IsoDateTimeUtils.MeridianFromHour(hour);
            var gmt = IsoDateTimeUtils.GmtFromOffset(OffsetGMT);
            var value = $"{month:D2}/{day:D2}/{year:D4} {hour % 12:D2}:{min:D2}:{sec:D2}.{ms:D2} {meridian} GMT+{gmt}";
            return value;
        }
    }
}