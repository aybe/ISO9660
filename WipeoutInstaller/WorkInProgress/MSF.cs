namespace WipeoutInstaller.WorkInProgress;

public readonly struct MSF : IComparable<MSF>, IEquatable<MSF>
{
    public static MSF Min { get; } = new(0, 0, 0);

    public static MSF Max { get; } = new(99, 59, 74);

    public readonly byte M, S, F;

    public MSF(int m, int s, int f)
    {
        if (m is < 0 or > 99)
        {
            throw new ArgumentOutOfRangeException(nameof(m), m, null);
        }

        if (s is < 0 or > 59)
        {
            throw new ArgumentOutOfRangeException(nameof(s), s, null);
        }

        if (f is < 0 or > 74)
        {
            throw new ArgumentOutOfRangeException(nameof(f), s, null);
        }

        M = (byte)m;
        S = (byte)s;
        F = (byte)f;
    }


    public LBA ToLBA()
    {
        return new LBA(M * 60 * 75 + S * 75 + F - 150);
    }

    public int CompareTo(MSF other)
    {
        var m = M.CompareTo(other.M);

        if (m != 0)
        {
            return m;
        }

        var s = S.CompareTo(other.S);

        if (s != 0)
        {
            return s;
        }

        var f = F.CompareTo(other.F);

        if (f != 0)
        {
            return f;
        }

        return 0;
    }

    public bool Equals(MSF other)
    {
        return M == other.M && S == other.S && F == other.F;
    }

    public override bool Equals(object? obj)
    {
        return obj is MSF other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(M, S, F);
    }

    public override string ToString()
    {
        return $"{M:D2}:{S:D2}.{F:D2}";
    }

    public static bool operator ==(MSF x, MSF y)
    {
        return x.Equals(y);
    }

    public static bool operator !=(MSF x, MSF y)
    {
        return !x.Equals(y);
    }

    public static bool operator <(MSF x, MSF y)
    {
        return x.CompareTo(y) < 0;
    }

    public static bool operator >(MSF x, MSF y)
    {
        return x.CompareTo(y) > 0;
    }

    public static bool operator <=(MSF x, MSF y)
    {
        return x.CompareTo(y) <= 0;
    }

    public static bool operator >=(MSF x, MSF y)
    {
        return x.CompareTo(y) >= 0;
    }

    public static MSF operator +(MSF x, MSF y)
    {
        return (x.ToLBA() + y.ToLBA()).ToMSF();
    }

    public static MSF operator -(MSF x, MSF y)
    {
        return (x.ToLBA() - y.ToLBA()).ToMSF();
    }

    public static MSF operator ++(MSF x)
    {
        return (x.ToLBA() + 1).ToMSF();
    }

    public static MSF operator --(MSF x)
    {
        return (x.ToLBA() - 1).ToMSF();
    }

    public static MSF operator +(MSF x, int y)
    {
        return (x.ToLBA() + y).ToMSF();
    }

    public static MSF operator -(MSF x, int y)
    {
        return (x.ToLBA() - y).ToMSF();
    }
}