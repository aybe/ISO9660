using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem;

public readonly struct Iso731 : IIsoNumber1<uint>
{
    public uint Value { get; }

    public Iso731(BinaryReader reader)
    {
        Value = reader.Read<uint>(Endianness.LE);
    }

    public override string ToString()
    {
        return $"{Value}";
    }

    public static implicit operator int(Iso731 iso731)
    {
        return Convert.ToInt32(iso731.Value);
    }

    public int ToInt32()
    {
        return Convert.ToInt32(Value);
    }
}