using System.Diagnostics.CodeAnalysis;

namespace WipeoutInstaller.WorkInProgress;

public unsafe struct SectorMode0
[SuppressMessage("ReSharper", "UnusedMember.Global")]
{
    public const int SyncPosition = 0;

    public const int SyncSize = 12;

    public fixed byte Sync[SyncSize];

    public const int HeaderPosition = SyncPosition + SyncSize;

    public const int HeaderSize = 4;

    public fixed byte Header[HeaderSize];

    public const int UserDataPosition = HeaderPosition + HeaderSize;

    public const int UserDataSize = 2336;

    public fixed byte UserData[UserDataPosition];
}