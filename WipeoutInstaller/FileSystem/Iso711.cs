using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem;

public readonly struct Iso711
{
    public byte Value { get; }

    public Iso711(BinaryReader reader)
    {
        Value = reader.Read<byte>();
    }

    public override string ToString()
    {
        return $"{Value}";
    }

    public static implicit operator int(Iso711 iso711)
    {
        return iso711.Value;
    }
}