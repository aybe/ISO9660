namespace WipeoutInstaller.WorkInProgress;

public unsafe struct SectorMode1
{
    public fixed byte Sync[12];
    public fixed byte Header[4];
    public fixed byte UserData[2048];
    public fixed byte EDC[4];
    public fixed byte Intermediate[8];
    public fixed byte ParityP[172];
    public fixed byte ParityQ[104];
}