namespace WipeoutInstaller.WorkInProgress;

public unsafe struct SectorMode0
{
    public fixed byte Sync[12];
    public fixed byte Header[4];
    public fixed byte Zero[2336];
}