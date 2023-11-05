using Whatever.Extensions;

namespace ISO9660.Logical;

public sealed class VolumeDescriptorDateTime
{
    public VolumeDescriptorDateTime(BinaryReader reader)
    {
        Year = reader.ReadStringAscii(4);

        MonthOfTheYear = reader.ReadStringAscii(2);

        DayOfTheMonth = reader.ReadStringAscii(2);

        HourOfTheDay = reader.ReadStringAscii(2);

        MinuteOfTheHour = reader.ReadStringAscii(2);

        SecondOfTheMinute = reader.ReadStringAscii(2);

        HundredthsOfASecond = reader.ReadStringAscii(2);

        OffsetFromGreenwichMeanTime = reader.ReadIso712();
    }

    public string Year { get; }

    public string MonthOfTheYear { get; }

    public string DayOfTheMonth { get; }

    public string HourOfTheDay { get; }

    public string MinuteOfTheHour { get; }

    public string SecondOfTheMinute { get; }

    public string HundredthsOfASecond { get; }

    public sbyte OffsetFromGreenwichMeanTime { get; }

    public DateTimeOffset ToDateTimeOffset()
    {
        var a = int.Parse(Year);
        var b = int.Parse(MonthOfTheYear);
        var c = int.Parse(DayOfTheMonth);
        var d = int.Parse(HourOfTheDay);
        var e = int.Parse(MinuteOfTheHour);
        var f = int.Parse(SecondOfTheMinute);
        var g = int.Parse(HundredthsOfASecond);
        var h = TimeSpan.FromMinutes(15 * OffsetFromGreenwichMeanTime);

        return DateTimeParser.TryParse(a, b, c, d, e, f, g * 10, h, out var result) ? result : DateTimeOffset.UnixEpoch;
    }

    public override string ToString()
    {
        return ToDateTimeOffset().ToString();
    }
}