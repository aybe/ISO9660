using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem;

public readonly struct Iso732
{
    private uint Value { get; }

    public Iso732(BinaryReader reader)
    {
        Value = reader.Read<uint>(Endianness.BE);
    }

    public override string ToString()
    {
        return $"{Value}";
    }

    public static implicit operator uint(Iso732 iso732)
    {
        return iso732.Value;
    }
}