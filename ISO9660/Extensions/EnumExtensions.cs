namespace ISO9660.Extensions;

public static class EnumExtensions
{
    public static bool HasFlags<T>(this T value, T flags) where T : Enum
    {
        var a = Convert.ToUInt64(value);
        var b = Convert.ToUInt64(flags);
        var c = (a & b) == b;

        return c;
    }
}