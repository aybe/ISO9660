namespace ISO9660.Logical;

public static class DateTimeParser
{
    public static bool TryParse(
        in int year, in int month, in int day, in int hour, in int minute, in int second, int millisecond, in TimeSpan offset, out DateTimeOffset result)
    {
        result = default;

        var min = DateTimeOffset.MinValue;
        var max = DateTimeOffset.MaxValue;

        if (!IsBetween(year, min.Year, max.Year))
        {
            return false;
        }

        if (!IsBetween(month, min.Month, max.Month))
        {
            return false;
        }

        if (!IsBetween(day, min.Day, max.Day))
        {
            return false;
        }

        if (!IsBetween(hour, min.Hour, max.Hour))
        {
            return false;
        }

        if (!IsBetween(minute, min.Minute, max.Minute))
        {
            return false;
        }

        if (!IsBetween(second, min.Second, max.Second))
        {
            return false;
        }

        if (!IsBetween(millisecond, min.Millisecond, max.Millisecond))
        {
            return false;
        }

        result = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, offset);

        return true;
    }

    private static bool IsBetween<T>(T value, T min, T max) where T : IComparable<T>
    {
        return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
    }
}