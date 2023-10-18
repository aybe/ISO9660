using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WipeoutInstaller.WorkInProgress;

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

    public const int Size = 2352;
    
    public const int SyncPosition = 0;
    public const int SyncSize = 12;
    
    public const int HeaderPosition = 12;
    public const int HeaderSize = 4;

    public const int EdcPositionMode1 = 2064;
    public const int EdcPositionMode2Form1 = 2072;
    public const int EdcPositionMode2Form2 = 2348;
    public const int EdcSize = 4;

    public const int IntermediatePositionMode1 = 2068;
    public const int IntermediateSizeMode1 = 8;

    public const int PParityPositionMode1 = 2076;
    public const int PParityPositionMode2Form1 = 2076;
    public const int PParitySizeMode1 = 172;
    public const int PParitySizeMode2Form1 = 172;
    
    public const int QParityPositionMode1 = 2076;
    public const int QParityPositionMode2Form1 = 2248;
    public const int QParitySizeMode1 = 104;
    public const int QParitySizeMode2Form1 = 104;
    
    public const int SubHeaderPositionMode2Form1 = 16;
    public const int SubHeaderPositionMode2Form2 = 16;
    public const int SubHeaderSizeMode2Form1 = 8;
    public const int SubHeaderSizeMode2Form2 = 8;

    public const int UserDataPositionAudio = 0;
    public const int UserDataPositionMode0 = 16;
    public const int UserDataPositionMode1 = 16;
    public const int UserDataPositionMode2Form1 = 24;
    public const int UserDataPositionMode2Form2 = 24;
    public const int UserDataPositionMode2FormLess = 16;
    
    public const int UserDataSizeAudio = 2352;
    public const int UserDataSizeMode0 = 2336;
    public const int UserDataSizeMode1 = 2048;
    public const int UserDataSizeMode2Form1 = 2048;
    public const int UserDataSizeMode2Form2 = 2324;
    public const int UserDataSizeMode2FormLess = 2336;

    uint GetEdc();

    uint GetEdcSum();

    int GetSize()
    {
        return Size;
    }

    Span<byte> GetUserData();

    int GetUserDataLength();

    int GetUserDataPosition();

    public static unsafe uint GetEdcSum<T>(ref T sector, in int start, in int length)
        where T : struct, ISector
    // TODO can't we use method below?
    {
        var pointer = Unsafe.AsPointer(ref sector);

        var span = new Span<byte>(pointer, Size);

        var slice = span.Slice(start, length);

        var edc = EdcUtility.Compute(slice);

        return edc;
    }

    public static SectorHeader GetHeader<T>(ref T sector, in int start, in int length)
        where T : struct, ISector
    {
        var slice = GetSlice(ref sector, start, length);

        var header = MemoryMarshal.Read<SectorHeader>(slice);

        return header;
    }

    public static Span<byte> GetSlice<T>(scoped ref T sector, int start, int length)
        where T : struct, ISector
    {
        var span = MemoryMarshal.CreateSpan(ref sector, 1);

        var bytes = MemoryMarshal.AsBytes(span);

        var slice = bytes.Slice(start, length);

        return slice;
    }

    public static uint ReadUInt32LE<T>(ref T sector, int start)
        where T : struct, ISector
    {
        var slice = GetSlice(ref sector, start, sizeof(uint));

        var value = BinaryPrimitives.ReadUInt32LittleEndian(slice);

        return value;
    }

    public static ISector Read<T>(Stream stream) where T : struct, ISector
    {
        Span<byte> span = stackalloc byte[Size];

        stream.ReadExactly(span);

        var read = MemoryMarshal.Read<T>(span);

        return read;
    }
}