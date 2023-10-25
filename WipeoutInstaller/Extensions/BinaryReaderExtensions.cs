using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;

namespace ISO9660.Tests.Extensions;

public static partial class BinaryReaderExtensions
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

public static partial class BinaryReaderExtensions
{
    public static byte ReadIso711(this BinaryReader reader)
    {
        var value = reader.Read<byte>();

        return value;
    }

    public static sbyte ReadIso712(this BinaryReader reader)
    {
        var value = reader.Read<sbyte>();

        return value;
    }

    public static ushort ReadIso721(this BinaryReader reader)
    {
        var value = reader.Read<ushort>(Endianness.LE);

        return value;
    }

    public static ushort ReadIso722(this BinaryReader reader)
    {
        var value = reader.Read<ushort>(Endianness.BE);

        return value;
    }

    public static ushort ReadIso723(this BinaryReader reader)
    {
        var value1 = reader.Read<ushort>(Endianness.LE);
        var value2 = reader.Read<ushort>(Endianness.BE);

        if (value1 != value2)
        {
            throw new InvalidDataException();
        }

        return value1;
    }

    public static uint ReadIso731(this BinaryReader reader)
    {
        var value = reader.Read<uint>(Endianness.LE);

        return value;
    }

    public static uint ReadIso732(this BinaryReader reader)
    {
        var value = reader.Read<uint>(Endianness.BE);

        return value;
    }

    public static uint ReadIso733(this BinaryReader reader)
    {
        var value1 = reader.Read<uint>(Endianness.LE);
        var value2 = reader.Read<uint>(Endianness.BE);

        if (value1 != value2)
        {
            throw new InvalidDataException();
        }

        return value1;
    }
}