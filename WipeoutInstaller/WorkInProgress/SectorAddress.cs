namespace WipeoutInstaller.WorkInProgress;

public readonly struct SectorAddress
{
    public readonly byte Minute, Second, Frame;

    public override string ToString()
    {
        return $"{Minute:D2}:{Second:D2}.{Frame:D2}";
    }
}