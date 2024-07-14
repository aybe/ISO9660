using System.Globalization;
using Whatever.Extensions;

namespace ISO9660.Logical;

public sealed class VolumeDescriptorDateTime(Stream stream)
{
    public string Year { get; } = stream.ReadStringAscii(4);

    public string MonthOfTheYear { get; } = stream.ReadStringAscii(2);

    public string DayOfTheMonth { get; } = stream.ReadStringAscii(2);

    public string HourOfTheDay { get; } = stream.ReadStringAscii(2);

    public string MinuteOfTheHour { get; } = stream.ReadStringAscii(2);

    public string SecondOfTheMinute { get; } = stream.ReadStringAscii(2);

    public string HundredthsOfASecond { get; } = stream.ReadStringAscii(2);

    public sbyte OffsetFromGreenwichMeanTime { get; } = stream.ReadIso712();

    public DateTimeOffset ToDateTimeOffset()
    {
        var a = int.Parse(Year, CultureInfo.InvariantCulture);
        var b = int.Parse(MonthOfTheYear, CultureInfo.InvariantCulture);
        var c = int.Parse(DayOfTheMonth, CultureInfo.InvariantCulture);
        var d = int.Parse(HourOfTheDay, CultureInfo.InvariantCulture);
        var e = int.Parse(MinuteOfTheHour, CultureInfo.InvariantCulture);
        var f = int.Parse(SecondOfTheMinute, CultureInfo.InvariantCulture);
        var g = int.Parse(HundredthsOfASecond, CultureInfo.InvariantCulture);
        var h = TimeSpan.FromMinutes(15 * OffsetFromGreenwichMeanTime);

        return DateTimeParser.TryParse(a, b, c, d, e, f, g * 10, h, out var result) ? result : DateTimeOffset.UnixEpoch;
    }

    public override string ToString()
    {
        return ToDateTimeOffset().ToString(CultureInfo.InvariantCulture);
    }
}