using Whatever.Extensions;

namespace ISO9660;

public static class BinaryReaderExtensions
{
    public static T Read<T>(this BinaryReader reader, Endianness? endianness = null) where T : unmanaged
    {
        return reader.BaseStream.Read<T>(endianness);
    }

    public static string ReadStringAscii(this BinaryReader reader, int length)
    {
        return reader.BaseStream.ReadStringAscii(length);
    }

    public static long Seek(this BinaryReader reader, long offset, SeekOrigin origin = SeekOrigin.Current)
    {
        return reader.BaseStream.Seek(offset, origin);
    }
}