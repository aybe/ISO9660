using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem;

public readonly struct Iso733
{
    private uint Value1 { get; }

    private uint Value2 { get; }

    public Iso733(BinaryReader reader)
    {
        Value1 = reader.Read<uint>(Endianness.LE);
        Value2 = reader.Read<uint>(Endianness.BE);
    }

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

    public static implicit operator uint(Iso733 iso733)
    {
        return iso733.Value1;
    }
}