using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.Iso9660;

public readonly struct Iso733 : IIsoNumber2<uint>
{
    public Iso733(BinaryReader reader)
    {
        Value1 = reader.Read<uint>(Endianness.LE);
        Value2 = reader.Read<uint>(Endianness.BE);
    }

    public uint Value1 { get; }

    public uint Value2 { get; }

    public override string ToString()
    {
        return $"{Value1} | {Value2}";
    }
}