namespace ISO9660.Tests.Extensions;

public static class IntegerExtensions
{
    public static int ToInt32(this long value)
    {
        return Convert.ToInt32(value);
    }
}