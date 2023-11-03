using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;

namespace ISO9660.Extensions;

public static class BinaryReaderExtensions
{
    [PublicAPI]
    public static Endianness Endianness { get; } = BitConverter.IsLittleEndian ? Endianness.LE : Endianness.BE;

    public static T Read<T>(this BinaryReader reader, Endianness? endianness = null) where T : unmanaged
    {
        var count = Unsafe.SizeOf<T>();

        var bytes = reader.ReadBytes(count);

        if ((endianness ?? Endianness) != Endianness)
        {
            bytes.AsSpan().Reverse();
        }

        var value = MemoryMarshal.Read<T>(bytes);

        return value;
    }

    public static string ReadStringAscii(this BinaryReader reader, int length)
    {
        if (reader.BaseStream.Length - reader.BaseStream.Position < length)
        {
            throw new EndOfStreamException();
        }

        var bytes = reader.ReadBytes(length);

        var ascii = Encoding.ASCII.GetString(bytes);

        return ascii;
    }

    public static long Seek(this BinaryReader reader, long offset, SeekOrigin origin = SeekOrigin.Current)
    {
        return reader.BaseStream.Seek(offset, origin);
    }

    public static bool TryPeek<T>(this BinaryReader reader, Func<BinaryReader, T> func, out T result)
    {
        result = default!;

        var position = reader.BaseStream.Position;

        try
        {
            var value = func(reader);

            result = value;

            return true;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            reader.BaseStream.Position = position;
        }
    }
}