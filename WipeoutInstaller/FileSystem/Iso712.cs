using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem;

public readonly struct Iso712
{
    private sbyte Value { get; }

    public Iso712(BinaryReader reader)
    {
        Value = reader.Read<sbyte>();
    }

    public override string ToString()
    {
        return $"{Value}";
    }

    public static implicit operator sbyte(Iso712 iso712)
    {
        return iso712.Value;
    }
}