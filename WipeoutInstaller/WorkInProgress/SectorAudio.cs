using System.Diagnostics.CodeAnalysis;

namespace WipeoutInstaller.WorkInProgress;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public unsafe struct SectorAudio : ISector
{
    public const int DataPosition = 0;

    public const int DataSize = 2352;

    public fixed byte Data[DataSize];

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
        return ISector.GetSlice(ref this, DataPosition, DataSize);
    }
}