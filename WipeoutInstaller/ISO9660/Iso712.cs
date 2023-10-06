using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.Iso9660;

public readonly struct Iso712 : IIsoNumber1<sbyte>
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