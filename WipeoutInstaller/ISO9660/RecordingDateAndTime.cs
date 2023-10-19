namespace WipeoutInstaller.ISO9660;

public readonly struct RecordingDateAndTime
{
    public RecordingDateAndTime(BinaryReader reader)
    {
        NumberOfYearsSince1900      = new Iso711(reader);
        MonthOfTheYear              = new Iso711(reader);
        DayOfTheMonth               = new Iso711(reader);
        HourOfTheDay                = new Iso711(reader);
        MinuteOfTheHour             = new Iso711(reader);
        SecondOfTheMinute           = new Iso711(reader);
        OffsetFromGreenwichMeanTime = new Iso712(reader);
    }

    public Iso711 NumberOfYearsSince1900 { get; }

    public Iso711 MonthOfTheYear { get; }

    public Iso711 DayOfTheMonth { get; }

    public Iso711 HourOfTheDay { get; }

    public Iso711 MinuteOfTheHour { get; }

    public Iso711 SecondOfTheMinute { get; }

    public Iso712 OffsetFromGreenwichMeanTime { get; }

    public DateTimeOffset ToDateTimeOffset()
    {
        var a = 1900 + NumberOfYearsSince1900;
        var b = MonthOfTheYear;
        var c = DayOfTheMonth;
        var d = HourOfTheDay;
        var e = MinuteOfTheHour;
        var f = SecondOfTheMinute;
        var h = TimeSpan.FromMinutes(15 * OffsetFromGreenwichMeanTime.Value);

        return DateTimeParser.TryParse(a, b, c, d, e, f, 0, h, out var result) ? result : DateTimeOffset.UnixEpoch;
    }

    public override string ToString()
    {
        return ToDateTimeOffset().ToString();
    }
}