using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem;

public readonly struct Iso731
{
    private uint Value { get; }

    public Iso731(BinaryReader reader)
    {
        Value = reader.Read<uint>(Endianness.LE);
    }

    public override string ToString()
    {
        return $"{Value}";
    }

    public int ToInt32()
    {
        return Convert.ToInt32(Value);
    }

    public static implicit operator uint(Iso731 iso731)
    {
        return iso731.Value;
    }
}