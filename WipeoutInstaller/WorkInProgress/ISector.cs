using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WipeoutInstaller.WorkInProgress;

public interface ISector
{
    public const int Size = 2352;

    uint GetEdc();

    uint GetEdcSum();

    Span<byte> GetUserData();

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