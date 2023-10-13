using System.Runtime.InteropServices;

namespace WipeoutInstaller.WorkInProgress;

[StructLayout(LayoutKind.Sequential, Size = 3)]
public readonly struct SectorAddress
{
    public readonly byte Min, Sec, Frac;

    public override string ToString()
    {
        return $"{nameof(Min)}: {Min}, " +
               $"{nameof(Sec)}: {Sec}, " +
               $"{nameof(Frac)}: {Frac}";
    }
}