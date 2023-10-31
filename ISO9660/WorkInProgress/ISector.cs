using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ISO9660.WorkInProgress;

/// <summary>
///     Base interface for a CD-ROM sector.
/// </summary>
public interface ISector
    // ECMA 130 https://www.ecma-international.org/wp-content/uploads/ECMA-130_2nd_edition_june_1996.pdf 
{
    /// <summary>
    ///     Gets the length in bytes of sector.
    /// </summary>
    int Length { get; }

    /// <summary>
    ///     Gets a byte span over the entire sector, i.e. raw.
    /// </summary>
    Span<byte> GetData();

    /// <summary>
    ///     Gets a byte span over the user data area, i.e. cooked.
    /// </summary>
    Span<byte> GetUserData();

    /// <summary>
    ///     Gets the length in bytes of the user data area.
    /// </summary>
    int GetUserDataLength();

    protected static Span<byte> GetSpan<T>(scoped ref T sector)
        where T : struct, ISector
    {
        return GetSpan(ref sector, 0, sector.Length);
    }

    protected static Span<byte> GetSpan<T>(scoped ref T sector, int start, int length)
        where T : struct, ISector
    {
        var span = MemoryMarshal.CreateSpan(ref sector, 1);

        var bytes = MemoryMarshal.AsBytes(span);

        var slice = bytes.Slice(start, length);

        return slice;
    }

    /// <summary>
    ///     Reads a sector of specified type from a stream.
    /// </summary>
    public static ISector Read<T>(Stream stream) where T : struct, ISector
    {
        var size = Unsafe.SizeOf<T>();

        Span<byte> span = stackalloc byte[size];

        stream.ReadExactly(span);

        var read = MemoryMarshal.Read<T>(span);

        return read;
    }
}