using System.Diagnostics.CodeAnalysis;

namespace WipeoutInstaller.WorkInProgress;

public unsafe struct SectorMode1
[SuppressMessage("ReSharper", "UnassignedField.Global")]
{
    private const int SyncSize = 12;

    public fixed byte Sync[SyncSize];

    private const int HeaderSize = 4;

    public fixed byte Header[HeaderSize];

    private const int UserDataSize = 2048;

    public fixed byte UserData[UserDataSize];

    private const int EdcSize = 4;

    public fixed byte Edc[EdcSize];

    private const int IntermediateSize = 8;

    public fixed byte Intermediate[IntermediateSize];

    private const int PParitySize = 172;

    public fixed byte PParity[PParitySize];

    private const int QParitySize = 104;

    public fixed byte QParity[QParitySize];
}