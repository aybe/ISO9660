using System.Runtime.InteropServices;

namespace ISO9660.Physical;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
public readonly struct SectorMode2SubHeader : IEquatable<SectorMode2SubHeader>
{
    public readonly byte FileNumber;

    public readonly byte ChannelNumber;

    public readonly SectorMode2SubMode SubMode;

    public readonly byte CodingInformation;

    public bool Equals(SectorMode2SubHeader other)
    {
        return
            FileNumber == other.FileNumber &&
            ChannelNumber == other.ChannelNumber &&
            SubMode == other.SubMode &&
            CodingInformation == other.CodingInformation;
    }

    public override bool Equals(object? obj)
    {
        return obj is SectorMode2SubHeader other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FileNumber, ChannelNumber, (int)SubMode, CodingInformation);
    }

    public static bool operator ==(SectorMode2SubHeader left, SectorMode2SubHeader right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SectorMode2SubHeader left, SectorMode2SubHeader right)
    {
        return !left.Equals(right);
    }
}