namespace WipeoutInstaller.WorkInProgress;

public static class SectorConstants
{
    public const int Size = 2352;

    public const int SyncPosition = 0;
    public const int SyncSize = 12;

    public const int HeaderPosition = SyncPosition + SyncSize;
    public const int HeaderSize = 4;

    public const int SubHeaderPosition = HeaderPosition + HeaderSize;
    public const int SubHeaderSize = 8;

    public const int UserDataPosition = SubHeaderPosition + SubHeaderSize;
    public const int UserDataSize = 2328;
}