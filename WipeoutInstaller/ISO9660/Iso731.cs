using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.Iso9660;

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
}