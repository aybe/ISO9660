namespace WipeoutInstaller.WorkInProgress;

public unsafe struct SectorMode2Form2
{
    public fixed byte Sync[12];
    public fixed byte Header[4];
    public fixed byte SubHeader[8];
    public fixed byte UserData[2324];
    public fixed byte ReservedOrEDC[4];
}