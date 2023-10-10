using System.Diagnostics.CodeAnalysis;

namespace WipeoutInstaller.WorkInProgress;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public unsafe struct SectorMode2Formless : ISector
{
    public const int SyncPosition = 0;

    public const int SyncSize = 12;

    public fixed byte Sync[SyncSize];

    public const int HeaderPosition = SyncPosition + SyncSize;

    public const int HeaderSize = 4;

    public fixed byte Header[HeaderSize];

    public const int UserDataPosition = HeaderPosition + HeaderSize;

    public const int UserDataSize = 2336;

    public fixed byte UserData[UserDataSize];

    public uint GetEdc()
    {
        throw new NotSupportedException();
    }

    public uint GetEdcSum()
    {
        throw new NotSupportedException();
    }

    public Span<byte> GetUserData()
    {
        return ISector.GetSlice(ref this, UserDataPosition, UserDataSize);
    }
}