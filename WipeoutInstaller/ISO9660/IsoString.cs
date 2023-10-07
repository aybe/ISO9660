using System.Diagnostics;
using System.Text;
using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.Iso9660;

[DebuggerDisplay("{Value}")]
public abstract class IsoString // TODO rename to Iso76, pass flags for allowed chars
{
    public const string ACharacters = """ !"%&'()*+,-./0123456789:;<=>?ABCDEFGHIJKLMNOPQRSTUVWXYZ_"""; // TODO
    public const string DCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_";                         // TODO
    public const string Separator1 = ".";                                                              // TODO
    public const string Separator2 = ";";                                                              // TODO
    protected const string CharsA = ACharacters;// """ !"%&'()*+,-./0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_""";
    protected const string CharsD = DCharacters + " " + "\u0000" + "\u0001" + Separator1 + Separator2; // "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_" + " "; // BUG isn't SPC missing?

    private protected IsoString(string value)
    {
        Value = value;
    }

    protected IsoString(BinaryReader reader, int length, string chars)
    {
        var value = reader.ReadStringAscii(length);

        if (value.Any(c => !chars.Contains(c)))
        {
            var message = $"The string '{value}' ({string.Join(", ", Encoding.ASCII.GetBytes(value))}) contains invalid characters" +
                          $"{Environment.NewLine}" +
                          $"The allowed characters are: '{chars}'.";
            //BUG fails on XA attributes
            //Debug.WriteLine(
            //    $"Invalid characters: {string.Join(", ", Encoding.ASCII.GetBytes(value).Select(s => s.ToString("X2")))}");
            throw new InvalidDataException(message);
        }

        Value = value;
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