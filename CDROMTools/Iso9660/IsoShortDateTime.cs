using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CDROMTools.Iso9660
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 7)]
    public struct IsoShortDateTime
    {
        public readonly Iso711 Year;
        public readonly Iso711 Month;
        public readonly Iso711 Day;
        public readonly Iso711 Hour;
        public readonly Iso711 Minute;
        public readonly Iso711 Second;
        public readonly Iso712 OffsetGMT;

        public IsoShortDateTime(BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            Year = new Iso711(reader);
            Month = new Iso711(reader);
            Day = new Iso711(reader);
            Hour = new Iso711(reader);
            Minute = new Iso711(reader);
            Second = new Iso711(reader);
            OffsetGMT = new Iso712(reader);
        }

        public override string ToString()
        {
            var month = Month.Value.ToString("D2");
            var day = Day.Value.ToString("D2");
            var year = (1900 + Year.Value).ToString();
            var hour = Hour.Value.ToString("D2");
            var minute = Minute.Value.ToString("D2");
            var second = Second.Value.ToString("D2");
            var meridian = IsoDateTimeUtils.MeridianFromHour(Hour);
            var gmt = IsoDateTimeUtils.GmtFromOffset(OffsetGMT);
            return $"{month}/{day}/{year} {hour}:{minute}:{second} {meridian} GMT+{gmt}";
        }
    }
}