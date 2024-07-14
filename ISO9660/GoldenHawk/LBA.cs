using System.Globalization;
using JetBrains.Annotations;

namespace ISO9660.GoldenHawk;

public readonly struct LBA : IComparable<LBA>, IEquatable<LBA>
{
    public const int PreGap = 150;

    private const int MinPosition = -PreGap;

    private const int MaxPosition = 99 * 60 * 75 + 59 * 75 + 74 - PreGap;

    [PublicAPI]
    public static LBA Min { get; } = new(MinPosition);

    [PublicAPI]
    public static LBA Max { get; } = new(MaxPosition);

    private readonly int Position;

    public LBA(int position)
    {
        if (position is < MinPosition or > MaxPosition)
        {
            throw new ArgumentOutOfRangeException(nameof(position), position, null);
        }

        Position = position;
    }

    [PublicAPI]
    public MSF ToMSF()
    {
        var lba = Position - PreGap;

        var msf = new MSF(lba / (60 * 75), lba / 75 % 60, lba % 75);

        return msf;
    }

    public int CompareTo(LBA other)
    {
        return Position.CompareTo(other.Position);
    }

    public bool Equals(LBA other)
    {
        return Position == other.Position;
    }

    public override bool Equals(object? obj)
    {
        return obj is LBA other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Position);
    }

    public override string ToString()
    {
        return Position.ToString(CultureInfo.InvariantCulture);
    }

    public static bool operator ==(LBA x, LBA y)
    {
        return x.Equals(y);
    }

    public static bool operator !=(LBA x, LBA y)
    {
        return !x.Equals(y);
    }

    public static bool operator <(LBA x, LBA y)
    {
        return x.CompareTo(y) < 0;
    }

    public static bool operator >(LBA x, LBA y)
    {
        return x.CompareTo(y) > 0;
    }

    public static bool operator <=(LBA x, LBA y)
    {
        return x.CompareTo(y) <= 0;
    }

    public static bool operator >=(LBA x, LBA y)
    {
        return x.CompareTo(y) >= 0;
    }

    public static LBA operator +(LBA x, LBA y)
    {
        return new LBA(x.Position + y.Position);
    }

    public static LBA operator -(LBA x, LBA y)
    {
        return new LBA(x.Position - y.Position);
    }

    public static LBA operator ++(LBA x)
    {
        return new LBA(x.Position + 1);
    }

    public static LBA operator --(LBA x)
    {
        return new LBA(x.Position - 1);
    }

    public static LBA operator +(LBA x, int y)
    {
        return new LBA(x.Position + y);
    }

    public static LBA operator -(LBA x, int y)
    {
        return new LBA(x.Position - y);
    }

    public static implicit operator int(LBA lba)
    {
        return lba.Position;
    }
}