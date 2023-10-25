namespace ISO9660.Tests.Extensions;

public static class ArrayExtensions
{
    public static BinaryReader ToBinaryReader(this byte[] array)
    {
        return new BinaryReader(new MemoryStream(array));
    }
}