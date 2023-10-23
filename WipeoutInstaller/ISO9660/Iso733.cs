using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.ISO9660;

public readonly struct Iso733 : IIsoNumber2<uint>
{
    public Iso733(BinaryReader reader)
    {
        Value1 = reader.Read<uint>(Endianness.LE);
        Value2 = reader.Read<uint>(Endianness.BE);
    }

    public uint Value1 { get; }

    public uint Value2 { get; }

    public override string ToString()
    {
        return $"{Value1}";
    }

    public static implicit operator int(Iso733 iso733)
    {
        return Convert.ToInt32(iso733.Value1);
    }

    public int ToInt32()
    {
        return Convert.ToInt32(Value1);
    }
}