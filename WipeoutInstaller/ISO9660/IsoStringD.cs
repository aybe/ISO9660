namespace WipeoutInstaller.Iso9660;

public sealed class IsoStringD : IsoString
{
    private const string Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_" + " "; // BUG isn't SPC missing?

    public IsoStringD(BinaryReader reader, int length)
        : base(reader, length, Chars)
    {
    }
}