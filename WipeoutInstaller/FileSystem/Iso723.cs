using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem;

public readonly struct Iso723
{
    private ushort Value1 { get; }

    private ushort Value2 { get; }

    public Iso723(BinaryReader reader)
    {
        Value1 = reader.Read<ushort>(Endianness.LE);
        Value2 = reader.Read<ushort>(Endianness.BE);
    }

    public override string ToString()
    {
        return $"{Value1}";
    }

    public static implicit operator ushort(Iso723 iso723)
    {
        return iso723.Value1;
    }
}