namespace ISO9660.Common;

public readonly struct LBA : IComparable<LBA>, IEquatable<LBA>
{
    private readonly int Value;

    public LBA(int value)
    {
        Value = value;
    }

    public MSF ToMSF()
    {
        var value = Value - 150;

        var msf = new MSF(value / (60 * 75), value / 75 % 60, value % 75);

        return msf;
    }

    public int CompareTo(LBA other)
    {
        return Value.CompareTo(other.Value);
    }

    public bool Equals(LBA other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is LBA other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }

    public override string ToString()
    {
        return Value.ToString();
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
        return new LBA(x.Value + y.Value);
    }

    public static LBA operator -(LBA x, LBA y)
    {
        return new LBA(x.Value - y.Value);
    }

    public static LBA operator ++(LBA x)
    {
        return new LBA(x.Value + 1);
    }

    public static LBA operator --(LBA x)
    {
        return new LBA(x.Value - 1);
    }

    public static LBA operator +(LBA x, int y)
    {
        return new LBA(x.Value + y);
    }

    public static LBA operator -(LBA x, int y)
    {
        return new LBA(x.Value - y);
    }

    public static implicit operator int(LBA lba)
    {
        return lba.Value;
    }
}