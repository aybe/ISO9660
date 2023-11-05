namespace ISO9660.Logical;

public readonly struct DirectoryRecordDateTime
{
    public DirectoryRecordDateTime(BinaryReader reader)
    {
        NumberOfYearsSince1900      = reader.ReadIso711();
        MonthOfTheYear              = reader.ReadIso711();
        DayOfTheMonth               = reader.ReadIso711();
        HourOfTheDay                = reader.ReadIso711();
        MinuteOfTheHour             = reader.ReadIso711();
        SecondOfTheMinute           = reader.ReadIso711();
        OffsetFromGreenwichMeanTime = reader.ReadIso712();
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