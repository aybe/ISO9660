using System.Text;
using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem;

public class IsoString // TODO rename to Iso76
    // we have to relax checking because there are a lot of retard ISOs out-there, e.g. commie Linux
    // they are non-compliant by cramming invalid chars as early as possible, e.g. system identifier
    // seems like they got no clue they have a dedicated shit solely for that: Rock Ridge extensions
{
    // NOTE: d-characters has no space but as strings are padded we do need it!

    private const string ACharacters =
        """!"%&'()*+,-./0123456789:;<=>?ABCDEFGHIJKLMNOPQRSTUVWXYZ_""" + Space;

    private const string DCharacters =
        "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_" + Space;

    private const string Separator1 =
        ".";

    private const string Separator2 =
        ";";

    private const string Space =
        " ";

    public const string Byte00 =
        "\u0000";

    public const string Byte01 =
        "\u0001";

    public IsoString(in BinaryReader reader, in int length, IsoStringFlags flags)
    {
        var ascii = reader.ReadStringAscii(length);

        var chars = new StringBuilder();

        if (flags.HasFlags(IsoStringFlags.ACharacters))
        {
            chars.Append(ACharacters);
        }

        if (flags.HasFlags(IsoStringFlags.DCharacters))
        {
            chars.Append(DCharacters);
        }

        if (flags.HasFlags(IsoStringFlags.Separator1))
        {
            chars.Append(Separator1);
        }

        if (flags.HasFlags(IsoStringFlags.Separator2))
        {
            chars.Append(Separator2);
        }

        if (flags.HasFlags(IsoStringFlags.Byte00))
        {
            chars.Append(Byte00);
        }

        if (flags.HasFlags(IsoStringFlags.Byte01))
        {
            chars.Append(Byte01);
        }

        var input = ascii;

        if (!CheckInputChars(input, chars))
        {
            input = input.ToUpperInvariant();
        }

        if (!CheckInputChars(input, chars))
        {
            chars.Append("-\0.+");
        }

        if (!CheckInputChars(input, chars))
        {
            var message = $"The string '{ascii}' ({string.Join(", ", Encoding.ASCII.GetBytes(ascii))}) contains invalid characters" +
                          $"{Environment.NewLine}" +
                          $"The allowed characters are: '{chars}'.";

            throw new InvalidDataException(message);
        }

        Value = ascii;
    }

    public string Value { get; }

    private static bool CheckInputChars(string input, StringBuilder chars)
    {
        var valid = chars.ToString();

        var check = input.All(valid.Contains);

        return check;
    }

    public override string ToString()
    {
        return Value switch // really nasty while debugging
        {
            Byte00 => ToCodePoint(Value, 0),
            Byte01 => ToCodePoint(Value, 0),
            _      => Value
        };
    }

    private static string ToCodePoint(string value, int index)
    {
        return $"\\u{char.ConvertToUtf32(value, index):X4}";
    }

    public static implicit operator string(IsoString isoString)
    {
        return isoString.Value;
    }
}