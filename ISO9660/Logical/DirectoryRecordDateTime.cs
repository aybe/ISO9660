namespace ISO9660.Logical;

public sealed class DirectoryRecordDateTime
{
    public DirectoryRecordDateTime(Stream stream)
    {
        NumberOfYearsSince1900      = stream.ReadIso711();
        MonthOfTheYear              = stream.ReadIso711();
        DayOfTheMonth               = stream.ReadIso711();
        HourOfTheDay                = stream.ReadIso711();
        MinuteOfTheHour             = stream.ReadIso711();
        SecondOfTheMinute           = stream.ReadIso711();
        OffsetFromGreenwichMeanTime = stream.ReadIso712();
    }

    public byte NumberOfYearsSince1900 { get; }

    public byte MonthOfTheYear { get; }

    public byte DayOfTheMonth { get; }

    public byte HourOfTheDay { get; }

    public byte MinuteOfTheHour { get; }

    public byte SecondOfTheMinute { get; }

    public sbyte OffsetFromGreenwichMeanTime { get; }

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
        return ToDateTimeOffset().ToString();
    }
}