namespace WipeoutInstaller.Extensions;

public static class IntegerExtensions
{
    public static int ToInt32(this long value)
    {
        return Convert.ToInt32(value);
    }
}