using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem;

public readonly struct Iso721
{
    private ushort Value { get; }

    public Iso721(BinaryReader reader)
    {
        Value = reader.Read<ushort>(Endianness.LE);
    }

    public override string ToString()
    {
        return $"{Value}";
    }

    public static implicit operator ushort(Iso721 iso721)
    {
        return iso721.Value;
    }
}