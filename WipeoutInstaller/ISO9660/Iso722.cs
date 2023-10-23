using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.ISO9660;

public readonly struct Iso722 : IIsoNumber1<ushort>
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