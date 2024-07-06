using System.ComponentModel;
using System.Runtime.InteropServices;
using ISO9660.Extensions;
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
        using var memory = Disc.GetDeviceAlignedBuffer(2352, Handle);

        var buffer = memory.Span;

        Disc.ReadSector(Handle.DangerousGetHandle(), (uint)index, buffer);

        ISector sector = Sector switch
        {
            SectorCooked2048       => throw new NotSupportedException(Sector.GetType().Name),
            SectorCooked2324       => throw new NotSupportedException(Sector.GetType().Name),
            SectorCooked2336       => throw new NotSupportedException(Sector.GetType().Name),
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

    public override unsafe Task<ISector> ReadSectorAsync(in int index)
    {
        const uint duration = 3u;
        const uint transfer = 1u; // sectors

        var memory = Disc.GetDeviceAlignedBuffer(2352, Handle);

        var sector = ReadSectorQuery((uint)index, transfer, duration, memory);

        var @event = new ManualResetEvent(false);

        var overlapped = new Overlapped(0, 0, @event.SafeWaitHandle.DangerousGetHandle(), null);

        var nativeOverlapped = overlapped.Pack(null, null);

        var ioctl = NativeMethods.DeviceIoControl(
            Handle,
            NativeConstants.IOCTL_SCSI_PASS_THROUGH_DIRECT,
            sector.Pointer, (uint)sector.Length, sector.Pointer, (uint)sector.Length,
            out _,
            nativeOverlapped
        );

        if (ioctl is false && Marshal.GetLastPInvokeError() is not NativeConstants.ERROR_IO_PENDING)
        {
            throw new Win32Exception();
        }

        var source = new TaskCompletionSource<ISector>();

        var handle = ThreadPool.RegisterWaitForSingleObject(
            @event,
            ReadSectorAsyncCallBack,
            new ReadSectorAsyncData(source, memory, nativeOverlapped),
            TimeSpan.FromSeconds(duration),
            true
        );

        source.Task.ContinueWith(_ =>
        {
            sector.Dispose();
            handle.Unregister(null);
            memory.Dispose();
            @event.Dispose();
        }, TaskScheduler.Current);

        return source.Task;
    }

    private unsafe void ReadSectorAsyncCallBack(object? state, bool timedOut)
    {
        var (source, memory, overlapped) = (ReadSectorAsyncData)state!;

        try
        {
            if (timedOut)
            {
                source.SetCanceled();
            }
            else
            {
                var sector = ReadSector(memory.Span);

                source.SetResult(sector);
            }
        }
        catch (Exception e)
        {
            source.SetException(e);
        }
        finally
        {
            Overlapped.Free(overlapped);
        }
    }

    private ISector ReadSector(Span<byte> span) // TODO DRY
    {
        ISector sector = Sector switch
        {
            SectorCooked2048       => throw new NotSupportedException(Sector.GetType().Name),
            SectorCooked2324       => throw new NotSupportedException(Sector.GetType().Name),
            SectorCooked2336       => throw new NotSupportedException(Sector.GetType().Name),
            SectorRawAudio         => MemoryMarshal.Read<SectorRawAudio>(span),
            SectorRawMode0         => MemoryMarshal.Read<SectorRawMode0>(span),
            SectorRawMode1         => MemoryMarshal.Read<SectorRawMode1>(span),
            SectorRawMode2Form1    => MemoryMarshal.Read<SectorRawMode2Form1>(span),
            SectorRawMode2Form2    => MemoryMarshal.Read<SectorRawMode2Form2>(span),
            SectorRawMode2FormLess => MemoryMarshal.Read<SectorRawMode2FormLess>(span),
            _                      => throw new NotSupportedException(Sector.GetType().Name),
        };

        return sector;
    }

    private static NativeMarshaller<NativeTypes.SCSI_PASS_THROUGH_DIRECT> ReadSectorQuery(
        uint position, uint transfer, uint timeOut, NativeMemory<byte> memory)
    {
        return new NativeMarshaller<NativeTypes.SCSI_PASS_THROUGH_DIRECT>
        {
            Structure = new NativeTypes.SCSI_PASS_THROUGH_DIRECT(12)
            {
                DataIn             = NativeConstants.SCSI_IOCTL_DATA_IN,
                DataTransferLength = memory.Length,
                DataBuffer         = memory.Pointer,
                TimeOutValue       = timeOut,
                Cdb =
                {
                    [00] = 0xBE,                          // operation code: READ CD
                    [01] = 0,                             // expected sector type: any
                    [02] = (byte)(position >> 24 & 0xFF), // starting LBA
                    [03] = (byte)(position >> 16 & 0xFF), // starting LBA
                    [04] = (byte)(position >> 08 & 0xFF), // starting LBA
                    [05] = (byte)(position >> 00 & 0xFF), // starting LBA
                    [06] = (byte)(transfer >> 16 & 0xFF), // transfer length
                    [07] = (byte)(transfer >> 08 & 0xFF), // transfer length
                    [08] = (byte)(transfer >> 00 & 0xFF), // transfer length
                    [09] = 0xF8,                          // sync, header, sub-header, user data, EDC, ECC
                    [10] = 0,                             // sub-channel data: none
                    [11] = 0,                             // control
                },
            },
        };
    }

    private readonly unsafe struct ReadSectorAsyncData(
        TaskCompletionSource<ISector> source,
        NativeMemory<byte> memory,
        NativeOverlapped* overlapped
    )
    {
        public TaskCompletionSource<ISector> Source { get; } = source;

        public NativeMemory<byte> Memory { get; } = memory;

        public NativeOverlapped* Overlapped { get; } = overlapped;

        public void Deconstruct(
            out TaskCompletionSource<ISector> source,
            out NativeMemory<byte> memory,
            out NativeOverlapped* overlapped)
        {
            source     = Source;
            memory     = Memory;
            overlapped = Overlapped;
        }
    }
}