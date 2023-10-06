namespace WipeoutInstaller.Iso9660;

public sealed class IsoStringA : IsoString
{
    private const string Chars = """ !"%&'()*+,-./0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_""";

    public IsoStringA(BinaryReader reader, int length)
        : base(reader, length, Chars)
    {
    }
}