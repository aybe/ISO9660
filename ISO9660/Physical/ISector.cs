using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ISO9660.Physical;

/// <summary>
///     Base interface for a CD-ROM sector.
/// </summary>
public interface ISector
// ECMA 130 https://www.ecma-international.org/wp-content/uploads/ECMA-130_2nd_edition_june_1996.pdf
// SCSI MMC https://www.13thmonkey.org/documentation/SCSI/mmc3r10g.pdf
// |---------------|------|--------|------------|-----------|----------|--------------|----------|----------|
// | Name          | Sync | Header | Sub-header | User data | EDC      | Intermediate | P Parity | Q Parity |
// |---------------|------|--------|------------|-----------|----------|--------------|----------|----------|
// | Audio         |      |        |            |      2352 |          |              |          |          |
// | Mode 0        |   12 |      4 |            |      2336 |          |              |          |          |
// | Mode 1        |   12 |      4 |            |      2048 |        4 |            8 |      172 |      104 |
// | Mode 2        |   12 |      4 |            |      2336 | optional |              |          |          |
// | Mode 2 Form 1 |   12 |      4 |          8 |      2048 |        4 |              |      172 |      104 |
// | Mode 2 Form 2 |   12 |      4 |          8 |      2324 | optional |              |          |          |
// |---------------|------|--------|------------|-----------|----------|--------------|----------|----------|
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

    internal static Span<byte> GetSpan<T>(scoped ref T sector, int start, int length)
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
    internal static ISector Read<T>(Stream stream) where T : struct, ISector
    {
        var size = Unsafe.SizeOf<T>();

        Span<byte> span = stackalloc byte[size];

        stream.ReadExactly(span);

        var read = MemoryMarshal.Read<T>(span);

        return read;
    }

    /// <summary>
    ///     Reads a sector of specified type from a stream.
    /// </summary>
    internal static async Task<ISector> ReadAsync<T>(Stream stream)
        where T : struct, ISector
    {
        var buffer = new byte[Unsafe.SizeOf<T>()];

        await stream.ReadExactlyAsync(buffer).ConfigureAwait(false);

        var sector = MemoryMarshal.Read<T>(buffer);

        return sector;
    }
}