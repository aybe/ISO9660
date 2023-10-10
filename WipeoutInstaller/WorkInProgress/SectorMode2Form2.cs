namespace WipeoutInstaller.WorkInProgress;

public unsafe struct SectorMode2Form2
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

    public const int UserDataSize = 2324;

    public fixed byte UserData[UserDataSize];

    public const int ReservedOrEdcPosition = UserDataPosition + UserDataSize;

    public const int ReservedOrEdcSize = 4;

    public fixed byte ReservedOrEdc[ReservedOrEdcSize];
}