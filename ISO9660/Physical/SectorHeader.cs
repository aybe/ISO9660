using System.Runtime.InteropServices;

namespace ISO9660.Physical;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
public readonly struct SectorHeader
{
    public readonly byte Minute, Second, Frame;

    private readonly byte ModePrivate; // TODO block identifier

    public SectorMode Mode => (SectorMode)(ModePrivate & 0b111);

    public override string ToString()
    {
        return $"{Minute:X2}:{Second:X2}.{Frame:X2}, {Mode}";
    }
}