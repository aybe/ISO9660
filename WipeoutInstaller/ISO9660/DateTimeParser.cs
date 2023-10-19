namespace WipeoutInstaller.ISO9660;

public static class DateTimeParser
{
    public static bool TryParse(
        in int year, in int month, in int day, in int hour, in int minute, in int second, in TimeSpan offset,
        out DateTimeOffset result)
    {
        result = default;

        var min = DateTimeOffset.MinValue;
        var max = DateTimeOffset.MaxValue;

        if (!IsInRange(year, min.Year, max.Year))
        {
            return false;
        }

        if (!IsInRange(month, min.Month, max.Month))
        {
            return false;
        }

        if (!IsInRange(day, min.Day, max.Day))
        {
            return false;
        }

        if (!IsInRange(hour, min.Hour, max.Hour))
        {
            return false;
        }

        if (!IsInRange(minute, min.Minute, max.Minute))
        {
            return false;
        }

        if (!IsInRange(second, min.Second, max.Second))
        {
            return false;
        }

        result = new DateTimeOffset(year, month, day, hour, minute, second, offset);

        return true;
    }

    private static bool IsInRange<T>(T value, T min, T max) where T : IComparable<T>
    {
        return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
    }
}