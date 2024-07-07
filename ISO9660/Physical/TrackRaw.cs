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

    public override ISector ReadSector(int index)
    {
        using var memory = Disc.GetDeviceAlignedBuffer(2352, Handle);

        var buffer = memory.Span;

        Disc.ReadSector(Handle.DangerousGetHandle(), (uint)index, buffer);

        var sector = ISector.Read(Sector, buffer);

        return sector;
    }

    public override unsafe Task<ISector> ReadSectorAsync(int index)
    {
        const uint duration = 3u;
        const uint transfer = 1u; // sectors

        var memory = Disc.GetDeviceAlignedBuffer(2352, Handle);

        var sector = Disc.ReadSectorWindowsQuery((uint)index, transfer, duration, memory.Pointer, memory.Length);

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

    private ISector ReadSector(Span<byte> span)
    {
        var sector = ISector.Read(Sector, span);

        return sector;
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