namespace WipeoutInstaller;

public static class ArrayExtensions
{
    public static BinaryReader ToBinaryReader(this byte[] array)
    {
        return new BinaryReader(new MemoryStream(array));
    }
}