using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ISO9660.Extensions;
using ISO9660.GoldenHawk;

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

    private static int GetLength(ISector sector)
    {
        var length = sector switch
        {
            SectorCooked2048       => Unsafe.SizeOf<SectorCooked2048>(),
            SectorCooked2324       => Unsafe.SizeOf<SectorCooked2324>(),
            SectorCooked2336       => Unsafe.SizeOf<SectorCooked2336>(),
            SectorRawAudio         => Unsafe.SizeOf<SectorRawAudio>(),
            SectorRawMode0         => Unsafe.SizeOf<SectorRawMode0>(),
            SectorRawMode1         => Unsafe.SizeOf<SectorRawMode1>(),
            SectorRawMode2Form1    => Unsafe.SizeOf<SectorRawMode2Form1>(),
            SectorRawMode2Form2    => Unsafe.SizeOf<SectorRawMode2Form2>(),
            SectorRawMode2FormLess => Unsafe.SizeOf<SectorRawMode2FormLess>(),
            _                      => throw new NotSupportedException(sector.GetType().Name),
        };

        return length;
    }

    internal static ISector GetSectorTypeCue(CueSheetTrackType type)
    {
        ISector sector = type switch
        {
            CueSheetTrackType.Audio             => new SectorRawAudio(),
            CueSheetTrackType.Karaoke           => throw new NotSupportedException(type.ToString()),
            CueSheetTrackType.Mode1Cooked       => new SectorCooked2048(),
            CueSheetTrackType.Mode1Raw          => new SectorRawMode1(),
            CueSheetTrackType.Mode2Form1Cooked  => new SectorCooked2324(),
            CueSheetTrackType.Mode2Form2Cooked  => new SectorCooked2336(),
            CueSheetTrackType.Mode2Mixed        => throw new NotSupportedException(type.ToString()),
            CueSheetTrackType.Mode2Raw          => new SectorRawMode2Form1(),
            CueSheetTrackType.InteractiveCooked => throw new NotSupportedException(type.ToString()),
            CueSheetTrackType.InteractiveRaw    => throw new NotSupportedException(type.ToString()),
            _                                   => throw new NotSupportedException(type.ToString()),
        };

        return sector;
    }

    internal static Span<byte> GetSpan<T>(scoped ref T sector, int start, int length)
        where T : struct, ISector
    {
        var span = MemoryMarshal.CreateSpan(ref sector, 1);

        var bytes = MemoryMarshal.AsBytes(span);

        var slice = bytes.Slice(start, length);

        return slice;
    }

    internal static ISector Read(ISector sector, Span<byte> buffer)
    {
        ISector result = sector switch
        {
            SectorCooked2048       => MemoryMarshal.Read<SectorCooked2048>(buffer),
            SectorCooked2324       => MemoryMarshal.Read<SectorCooked2324>(buffer),
            SectorCooked2336       => MemoryMarshal.Read<SectorCooked2336>(buffer),
            SectorRawAudio         => MemoryMarshal.Read<SectorRawAudio>(buffer),
            SectorRawMode0         => MemoryMarshal.Read<SectorRawMode0>(buffer),
            SectorRawMode1         => MemoryMarshal.Read<SectorRawMode1>(buffer),
            SectorRawMode2Form1    => MemoryMarshal.Read<SectorRawMode2Form1>(buffer),
            SectorRawMode2Form2    => MemoryMarshal.Read<SectorRawMode2Form2>(buffer),
            SectorRawMode2FormLess => MemoryMarshal.Read<SectorRawMode2FormLess>(buffer),
            _                      => throw new NotSupportedException(sector.GetType().Name),
        };

        return result;
    }

    internal static ISector Read(ISector sector, Stream stream)
    {
        using var scope = new ArrayPoolScope<byte>(GetLength(sector));

        stream.ReadExactly(scope.Span);

        var result = Read(sector, scope.Span);

        return result;
    }

    internal static async Task<ISector> ReadAsync(ISector sector, Stream stream)
    {
        using var scope = new ArrayPoolScope<byte>(GetLength(sector));

        await stream.ReadExactlyAsync(scope.Memory);

        var result = Read(sector, scope.Span);

        return result;
    }
}