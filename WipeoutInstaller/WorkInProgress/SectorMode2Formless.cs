namespace WipeoutInstaller.WorkInProgress;

public unsafe struct SectorMode2Formless
{
    public fixed byte Sync[12];
    public fixed byte Header[4];
    public fixed byte UserData[2336];
}