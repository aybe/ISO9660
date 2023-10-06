using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.Iso9660;

public readonly struct Iso722 : IIsoNumber1<ushort>
{
    public ushort Value { get; }

    public Iso722(BinaryReader reader)
    {
        Value = reader.Read<ushort>(Endianness.BE);
    }

    public override string ToString()
    {
        return $"{Value}";
    }
}