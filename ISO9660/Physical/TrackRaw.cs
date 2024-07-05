using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace ISO9660.Physical;

internal sealed class TrackRaw : Track
{
    public TrackRaw(int index, int position, int length, bool audio, ISector sector, SafeFileHandle handle)
    {
        Index = index;

        Position = position;

        Length = length;

        Audio = audio;

        Sector = sector;

        Handle = handle;
    }

    private SafeFileHandle Handle { get; }

    public override ISector ReadSector(in int index)
    {
        using var memory = Disc.GetDeviceAlignedBuffer(2352 /*TODO*/, Handle);

        var buffer = memory.Span;

        Disc.ReadSector(Handle.DangerousGetHandle(), (uint)index, buffer);

        ISector sector = Sector switch // TODO DRY with ISector.Read
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
            _                      => throw new NotSupportedException(Sector.GetType().Name),
        };

        return sector;
    }

    public override Task<ISector> ReadSectorAsync(in int index)
    {
        // TODO async DeviceIoControl -> PreAllocatedOverlapped, ThreadPoolBoundHandle

        var sector = ReadSector(index);

        return Task.FromResult(sector);
    }
}