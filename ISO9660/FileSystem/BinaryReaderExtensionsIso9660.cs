using System.Diagnostics.CodeAnalysis;
using System.Text;
using ISO9660.Extensions;

namespace ISO9660.FileSystem;

public static class BinaryReaderExtensionsIso9660
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

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static string ReadIsoString(this BinaryReader reader, in int length, IsoStringFlags flags)
        // the Linux retards were whining for Rock Ridge extensions yet don't use it, handle the shit
        // fun fact: even if we'd implement it, it would fail as they cram garbage as early as in PVD
    {
        var ascii = reader.ReadStringAscii(length);

        var chars = new StringBuilder(); // we also add space because few morons love padding with it

        if (flags.HasFlags(IsoStringFlags.ACharacters))
        {
            chars.Append("!\"%&\'()*+,-./0123456789:;<=>?ABCDEFGHIJKLMNOPQRSTUVWXYZ_ ");
        }

        if (flags.HasFlags(IsoStringFlags.DCharacters))
        {
            chars.Append("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_ ");
        }

        if (flags.HasFlags(IsoStringFlags.Separator1))
        {
            chars.Append('.');
        }

        if (flags.HasFlags(IsoStringFlags.Separator2))
        {
            chars.Append(';');
        }

        if (flags.HasFlags(IsoStringFlags.Byte00)) // directory .
        {
            chars.Append('\u0000');
        }

        if (flags.HasFlags(IsoStringFlags.Byte01)) // directory ..
        {
            chars.Append('\u0001');
        }

        var valid = chars.ToString();

        var input = ascii;

        if (!Check(input, chars))
        {
            input = input.ToUpperInvariant();
        }

        if (!Check(input, chars))
        {
            chars.Append("-\0.+");
        }

        if (!Check(input, chars))
        {
            throw new InvalidDataException(
                $"The string '{ascii}' ({string.Join(", ", Encoding.ASCII.GetBytes(ascii))}) contains invalid characters." +
                $"{Environment.NewLine}" +
                $"The allowed characters are: '{valid}'.");
        }

        return ascii;

        static bool Check(string input, StringBuilder chars)
        {
            return input.All(chars.ToString().Contains);
        }
    }
}