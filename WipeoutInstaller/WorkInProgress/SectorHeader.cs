namespace ISO9660.Tests.WorkInProgress;

public readonly struct SectorHeader
{
    public readonly SectorAddress Address;

    public readonly SectorMode Mode;

    public override string ToString()
    {
        return $"{nameof(Address)}: {Address}, {nameof(Mode)}: {Mode}";
    }
}