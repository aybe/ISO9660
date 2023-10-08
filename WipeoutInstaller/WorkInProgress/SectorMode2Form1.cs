using System.Diagnostics.CodeAnalysis;

namespace WipeoutInstaller.WorkInProgress;

[SuppressMessage("ReSharper", "UnassignedField.Global")]
public unsafe struct SectorMode2Form1
{
    private const int SyncSize = 12;

    public fixed byte Sync[SyncSize];

    public fixed byte Header[4];

    public fixed byte SubHeader[8];

    public fixed byte UserData[UserDataSize];

    private const int UserDataSize = 2048;

    public fixed byte Edc[4];

    public fixed byte ParityP[172];

    public fixed byte ParityQ[104];
}