using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.ISO9660;

public readonly struct Iso711 : IIsoNumber1<byte>
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