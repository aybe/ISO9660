using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.Iso9660;

public readonly struct Iso721 : IIsoNumber1<ushort>
{
    public ushort Value { get; }

    public Iso721(BinaryReader reader)
    {
        Value = reader.Read<ushort>(Endianness.LE);
    }

    public override string ToString()
    {
        return $"{Value}";
    }
}