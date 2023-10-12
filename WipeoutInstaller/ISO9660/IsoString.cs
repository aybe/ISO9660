using System.Text;
using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.Iso9660;

public class IsoString // TODO rename to Iso76, pass flags for allowed chars
{
    private const string ACharacters = """!"%&'()*+,-./0123456789:;<=>?ABCDEFGHIJKLMNOPQRSTUVWXYZ_""" + Space;
    private const string DCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_" + Space;
    private const string Separator1 = ".";
    private const string Separator2 = ";";
    private const string Space = " ";
    public const string Byte00 = "\u0000";
    public const string Byte01 = "\u0001";

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

        if (ascii.Any(c => !chars.ToString().Contains(c)))
        {
            var message = $"The string '{ascii}' ({string.Join(", ", Encoding.ASCII.GetBytes(ascii))}) contains invalid characters" +
                          $"{Environment.NewLine}" +
                          $"The allowed characters are: '{chars}'.";

            throw new InvalidDataException(message);
        }

        Value = ascii;
    }

    public string Value { get; }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(IsoString isoString)
    {
        return isoString.Value;
    }
}