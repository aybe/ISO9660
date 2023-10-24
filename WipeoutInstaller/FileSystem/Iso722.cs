using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem;

public readonly struct Iso722
{
    public ushort Value { get; }

    public Iso722(BinaryReader reader)
    {
        Value = reader.Read<ushort>(Endianness.BE);
    }

    public override string ToString()
    {
        return $"{Value}";
    }
}