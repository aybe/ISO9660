namespace WipeoutInstaller.XA;

public readonly struct SectorSubHeaderBlock
{
    public readonly SectorSubHeader Header1;

    public readonly SectorSubHeader Header2;

    public bool IsValid => EqualityComparer<SectorSubHeader>.Default.Equals(Header1, Header2);
}