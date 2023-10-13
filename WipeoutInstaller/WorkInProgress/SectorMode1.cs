using System.Diagnostics.CodeAnalysis;

namespace WipeoutInstaller.WorkInProgress;

[SuppressMessage("ReSharper", "UnassignedField.Global")]
public unsafe struct SectorMode1 : ISector, ISectorHeader
{
    public const int SyncPosition = 0;

    public const int SyncSize = 12;

    public fixed byte Sync[SyncSize];

    public const int HeaderPosition = SyncPosition + SyncSize;

    public const int HeaderSize = 4;

    public fixed byte Header[HeaderSize];

    public const int UserDataPosition = HeaderPosition + HeaderSize;

    public const int UserDataSize = 2048;

    public fixed byte UserData[UserDataSize];

    public const int EdcPosition = UserDataPosition + UserDataSize;

    public const int EdcSize = 4;

    public fixed byte Edc[EdcSize];

    public const int IntermediatePosition = EdcPosition + EdcSize;

    public const int IntermediateSize = 8;

    public fixed byte Intermediate[IntermediateSize];

    public const int PParityPosition = IntermediatePosition + IntermediateSize;

    public const int PParitySize = 172;

    public fixed byte PParity[PParitySize];

    public const int QParityPosition = IntermediatePosition + IntermediateSize;

    public const int QParitySize = 104;

    public fixed byte QParity[QParitySize];

    public uint GetEdc()
    {
        return ISector.ReadUInt32LE(ref this, EdcPosition);
    }

    public uint GetEdcSum()
    {
        return ISector.GetEdcSum(ref this, SyncPosition, SyncSize + HeaderSize + UserDataSize);
    }

    public Span<byte> GetUserData()
    {
        return ISector.GetSlice(ref this, UserDataPosition, UserDataSize);
    }

    SectorHeader ISectorHeader.Header => ISector.GetHeader(ref this, HeaderPosition, HeaderSize);
}