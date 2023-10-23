using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.ISO9660;

public readonly struct Iso723 : IIsoNumber2<ushort>
{
    public ushort Value1 { get; }

    public ushort Value2 { get; }

    public Iso723(BinaryReader reader)
    {
        Value1 = reader.Read<ushort>(Endianness.LE);
        Value2 = reader.Read<ushort>(Endianness.BE);
    }

    public override string ToString()
    {
        return $"{Value1}";
    }
}