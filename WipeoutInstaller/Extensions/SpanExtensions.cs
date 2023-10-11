namespace WipeoutInstaller.Extensions;

public static class SpanExtensions
{
    public static BinaryReader ToBinaryReader(this Span<byte> span)
    {
        return new BinaryReader(new MemoryStream(span.ToArray()));
    }
}