using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem;

public readonly struct Iso712
{
    public sbyte Value { get; }

    public Iso712(BinaryReader reader)
    {
        Value = reader.Read<sbyte>();
    }

    public override string ToString()
    {
        return $"{Value}";
    }
}