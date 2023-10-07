namespace WipeoutInstaller.Iso9660;

public sealed class IsoStringA : IsoString
{
    internal IsoStringA(string value) : base(value)
    {
    }

    public IsoStringA(BinaryReader reader, int length)
        : base(reader, length, CharsA)
    {
    }
}