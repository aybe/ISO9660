namespace WipeoutInstaller.Iso9660;

public sealed class IsoStringD : IsoString
{
    internal IsoStringD(string value) : base(value)
    {
    }

    public IsoStringD(BinaryReader reader, int length)
        : base(reader, length, CharsD)
    {
    }
}