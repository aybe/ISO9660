using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ISO9660.Tests.WorkInProgress;

public interface ISector
    // CD-ROM
    // https://en.wikipedia.org/wiki/CD-ROM
    // ECMA 130
    // https://www.ecma-international.org/wp-content/uploads/ECMA-130_2nd_edition_june_1996.pdf 
    // SYSTEM DESCRIPTION CD-ROM XA, PHILIPS/SONY, May 1991
    // https://archive.org/details/xa-10-may-1991
    // CD Cracking Uncovered Protection Against Unsanctioned CD Copying
    // https://archive.org/details/CDCrackingUncoveredProtectionAgainstUnsanctionedCDCopyingKrisKaspersky
{

    public const int RawSize = 2352;
    
    public const int SyncPosition = 0;
    public const int SyncSize = 12;
    
    public const int HeaderPosition = 12;
    public const int HeaderSize = 4;

    public const int SubHeaderPositionMode2Form1 = 16;
    public const int SubHeaderPositionMode2Form2 = 16;

    int Size => RawSize;

    Span<byte> AsByteSpan();

    Span<byte> GetUserData();

    int GetUserDataLength();

    public static SectorHeader GetHeader<T>(ref T sector, in int start, in int length)
        where T : struct, ISector
    {
        var slice = GetSlice(ref sector, start, length);

        var header = MemoryMarshal.Read<SectorHeader>(slice);

        return header;
    }

    public static Span<byte> AsByteSpan<T>(scoped ref T sector)
        where T : struct, ISector
    {
        return GetSlice(ref sector, 0, sector.Size);
    }

    public static Span<byte> GetSlice<T>(scoped ref T sector, int start, int length)
        where T : struct, ISector
    {
        var span = MemoryMarshal.CreateSpan(ref sector, 1);

        var bytes = MemoryMarshal.AsBytes(span);

        var slice = bytes.Slice(start, length);

        return slice;
    }

    public static ISector Read<T>(Stream stream) where T : struct, ISector
    {
        var size = Unsafe.SizeOf<T>();

        Span<byte> span = stackalloc byte[size];

        stream.ReadExactly(span);

        var read = MemoryMarshal.Read<T>(span);

        return read;
    }
}