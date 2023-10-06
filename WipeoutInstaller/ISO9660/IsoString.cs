using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.Iso9660;

public abstract class IsoString
{
    protected IsoString(BinaryReader reader, int length, string chars)
    {
        var value = reader.ReadStringAscii(length);

        if (value.Any(c => !chars.Contains(c)))
        {
            var message = $"The string '{value}' contains invalid characters" +
                          $"{Environment.NewLine}" +
                          $"The allowed characters are: '{chars}'.";
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