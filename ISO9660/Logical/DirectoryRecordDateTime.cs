using System.Globalization;

namespace ISO9660.Logical;

public sealed class DirectoryRecordDateTime(Stream stream)
{
    public byte NumberOfYearsSince1900 { get; } = stream.ReadIso711();

    public byte MonthOfTheYear { get; } = stream.ReadIso711();

    public byte DayOfTheMonth { get; } = stream.ReadIso711();

    public byte HourOfTheDay { get; } = stream.ReadIso711();

    public byte MinuteOfTheHour { get; } = stream.ReadIso711();

    public byte SecondOfTheMinute { get; } = stream.ReadIso711();

    public sbyte OffsetFromGreenwichMeanTime { get; } = stream.ReadIso712();

    public DateTimeOffset ToDateTimeOffset()
    {
        var a = 1900 + NumberOfYearsSince1900;
        var b = MonthOfTheYear;
        var c = DayOfTheMonth;
        var d = HourOfTheDay;
        var e = MinuteOfTheHour;
        var f = SecondOfTheMinute;
        var h = TimeSpan.FromMinutes(15 * OffsetFromGreenwichMeanTime);

        return DateTimeParser.TryParse(a, b, c, d, e, f, 0, h, out var result) ? result : DateTimeOffset.UnixEpoch;
    }

    public override string ToString()
    {
        return ToDateTimeOffset().ToString(CultureInfo.InvariantCulture);
    }
}