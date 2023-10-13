namespace WipeoutInstaller.WorkInProgress;

public readonly struct LBA : IComparable<LBA>, IEquatable<LBA>
{
    public readonly int Position;

    public static LBA Min { get; } = MSF.Min.ToLBA();

    public static LBA Max { get; } = MSF.Max.ToLBA();

    public LBA(int position)
    {
        if (position < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(position), position, null);
        }

        Position = position;
    }

    public MSF ToMSF()
    {
        return new MSF(Position / (60 * 75), Position / 75 % 60, Position % 75);
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
        return Position.ToString();
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