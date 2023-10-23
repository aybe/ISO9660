using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.ISO9660;

public readonly struct Iso721 : IIsoNumber1<ushort>
{
    public ushort Value { get; }

    public Iso721(BinaryReader reader)
    {
        Value = reader.Read<ushort>(Endianness.LE);
    }

    public override string ToString()
    {
        return $"{Value}";
    }
}