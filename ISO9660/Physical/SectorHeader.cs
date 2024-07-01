using System.Runtime.InteropServices;

namespace ISO9660.Physical;

[StructLayout(LayoutKind.Sequential)]
public readonly struct SectorHeader
{
    public readonly byte Minute, Second, Frame;

    public readonly SectorMode Mode; // TODO block identifier

    public override string ToString()
    {
        return $"{Minute:X2}:{Second:X2}.{Frame:X2}, {Mode}";
    }
}