namespace CDROMTools.Iso9660
{
    public static class IsoDateTimeUtils
    {
        public static int GmtFromOffset(Iso712 i)
        {
            var gmt = i/(60/15);
            return gmt;
        }

        public static string MeridianFromHour(int hour)
        {
            var meridian = hour%12 > 0 ? "PM" : "AM";
            return meridian;
        }
    }
}