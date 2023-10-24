using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem;

public readonly struct Iso722
{
    private ushort Value { get; }

    public Iso722(BinaryReader reader)
    {
        Value = reader.Read<ushort>(Endianness.BE);
    }

    public override string ToString()
    {
        return $"{Value}";
    }

    public static implicit operator ushort(Iso722 iso722)
    {
        return iso722.Value;
    }
}