using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem;

public readonly struct Iso732
{
    public uint Value { get; }

    public Iso732(BinaryReader reader)
    {
        Value = reader.Read<uint>(Endianness.BE);
    }

    public override string ToString()
    {
        return $"{Value}";
    }
}