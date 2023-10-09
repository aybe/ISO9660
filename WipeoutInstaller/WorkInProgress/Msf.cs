namespace WipeoutInstaller.WorkInProgress;

public readonly struct Msf : IEquatable<Msf>
{
    public static readonly Msf Zero = new();

    public readonly byte Minutes, Seconds, Frames;

    public Msf(byte minutes, byte seconds, byte frames)
    {
        Minutes = minutes;
        Seconds = seconds;
        Frames  = frames;
    }

    public override string ToString()
    {
        return $"{Minutes:D2}:{Seconds:D2}.{Frames:D2}";
    }

    public bool Equals(Msf other)
    {
        return Minutes == other.Minutes && Seconds == other.Seconds && Frames == other.Frames;
    }

    public override bool Equals(object? obj)
    {
        return obj is Msf other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Minutes, Seconds, Frames);
    }

    public static bool operator ==(Msf left, Msf right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Msf left, Msf right)
    {
        return !left.Equals(right);
    }
}