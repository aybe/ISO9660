using System.Diagnostics.CodeAnalysis;

namespace WipeoutInstaller.WorkInProgress;

[SuppressMessage("ReSharper", "UnassignedField.Global")]
public unsafe struct SectorMode2Form1
{
    public const int SyncPosition = 0;

    public const int SyncSize = 12;

    public fixed byte Sync[SyncSize];

    public const int HeaderPosition = SyncPosition + SyncSize;

    public const int HeaderSize = 4;

    public fixed byte Header[HeaderSize];

    public const int SubHeaderPosition = HeaderPosition + HeaderSize;

    public const int SubHeaderSize = 8;

    public fixed byte SubHeader[SubHeaderSize];

    public const int UserDataPosition = SubHeaderPosition + SubHeaderSize;

    public const int UserDataSize = 2048;

    public fixed byte UserData[UserDataSize];

    public const int EdcPosition = UserDataPosition + UserDataSize;

    public const int EdcSize = 4;

    public fixed byte Edc[EdcSize];

    public const int PParityPosition = EdcPosition + EdcSize;

    public const int PParitySize = 172;

    public fixed byte PParity[PParitySize];

    public const int QParityPosition = PParityPosition + PParitySize;

    public const int QParitySize = 104;

    public fixed byte QParity[QParitySize];



}