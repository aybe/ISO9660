namespace WipeoutInstaller.Iso9660;

public sealed class RecordingDateAndTime
{
    public RecordingDateAndTime(BinaryReader reader)
    {
        NumberOfYearsSince1900      = new Iso711(reader);
        MonthOfTheYear              = new Iso711(reader);
        DayOfTheMonth               = new Iso711(reader);
        HourOfTheDay                = new Iso711(reader);
        MinuteOfTheHour             = new Iso711(reader);
        SecondOfTheMinute           = new Iso711(reader);
        OffsetFromGreenwichMeanTime = new Iso711(reader);
    }

    public Iso711 NumberOfYearsSince1900 { get; }

    public Iso711 MonthOfTheYear { get; }

    public Iso711 DayOfTheMonth { get; }

    public Iso711 HourOfTheDay { get; }

    public Iso711 MinuteOfTheHour { get; }

    public Iso711 SecondOfTheMinute { get; }

    public Iso711 OffsetFromGreenwichMeanTime { get; }
}