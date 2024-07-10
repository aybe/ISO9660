using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
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
        if (OperatingSystem.IsWindows())
        {
            return ReadSectorWindows(index);
        }

        throw new PlatformNotSupportedException();
    }

    [SupportedOSPlatform("windows")]
    private ISector ReadSectorWindows(int index)
    {
        using var memory = Disc.GetDeviceAlignedBuffer(2352, Handle);

        var buffer = memory.Span;

        Disc.ReadSector(Handle.DangerousGetHandle(), (uint)index, buffer);

        var sector = ISector.Read(Sector, buffer);

        return sector;
    }

    public override Task<ISector> ReadSectorAsync(int index)
    {
        if (OperatingSystem.IsWindows())
        {
            return ReadSectorAsyncWindows(index);
        }

        throw new PlatformNotSupportedException();
    }

    [SupportedOSPlatform("windows")]
    private unsafe Task<ISector> ReadSectorAsyncWindows(int index, uint timeout = 3u)
    {
#pragma warning disable CA2000 // Dispose objects before losing scope
        var memory = Disc.GetDeviceAlignedBuffer(2352, Handle);

        var sector = Disc.ReadSectorWindowsQuery((uint)index, 1u, timeout, memory.Pointer, memory.Length);
#pragma warning restore CA2000 // Dispose objects before losing scope

        var @event = new ManualResetEvent(false);

        var overlapped = new Overlapped(0, 0, @event.SafeWaitHandle.DangerousGetHandle(), null).Pack(null, null);

        var ioctl = NativeMethods.DeviceIoControl(
            Handle,
            NativeConstants.IOCTL_SCSI_PASS_THROUGH_DIRECT,
            sector.Pointer, (uint)sector.Length, sector.Pointer, (uint)sector.Length,
            out _,
            overlapped
        );

        if (ioctl is false && Marshal.GetLastPInvokeError() is not NativeConstants.ERROR_IO_PENDING)
        {
            throw new Win32Exception();
        }

        var source = new TaskCompletionSource<ISector>();

        var handle = ThreadPool.RegisterWaitForSingleObject(
            @event,
            ReadSectorAsyncWindowsCallBack,
            new ReadSectorAsyncWindowsData(source, sector, memory, overlapped),
            TimeSpan.FromSeconds(timeout),
            true
        );

        source.Task.ContinueWith(_ =>
        {
            handle.Unregister(null);
            memory.Dispose();
            @event.Dispose();
        }, TaskScheduler.Current);

        return source.Task;
    }

    [SupportedOSPlatform("windows")]
    private unsafe void ReadSectorAsyncWindowsCallBack(object? state, bool timedOut)
    {
        var (source, query, memory, overlapped) = (ReadSectorAsyncWindowsData)state!;

        try
        {
            if (timedOut)
            {
                source.SetCanceled();
            }
            else
            {
                var sector = ISector.Read(Sector, memory.Span);

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
            query.Dispose();
        }
    }

    private readonly unsafe struct ReadSectorAsyncWindowsData(
        TaskCompletionSource<ISector> source,
        NativeMarshaller<NativeTypes.SCSI_PASS_THROUGH_DIRECT> query,
        NativeMemory<byte> memory,
        NativeOverlapped* overlapped
    )
    {
        public TaskCompletionSource<ISector> Source { get; } = source;

        public NativeMarshaller<NativeTypes.SCSI_PASS_THROUGH_DIRECT> Query { get; } = query;

        public NativeMemory<byte> Memory { get; } = memory;

        public NativeOverlapped* Overlapped { get; } = overlapped;

        public void Deconstruct(
            out TaskCompletionSource<ISector> source,
            out NativeMarshaller<NativeTypes.SCSI_PASS_THROUGH_DIRECT> query,
            out NativeMemory<byte> memory,
            out NativeOverlapped* overlapped)
        {
            source     = Source;
            query      = Query;
            memory     = Memory;
            overlapped = Overlapped;
        }
    }
}