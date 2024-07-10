using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using ISO9660.Extensions;
using Microsoft.Win32.SafeHandles;
using Whatever.Extensions;

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
    private Task<ISector> ReadSectorAsyncWindows(int index, uint timeout = 3u)
    {
#pragma warning disable CA2000 // Dispose objects before losing scope // gets disposed in callback
        var state = new ReadSectorAsyncWindowsState(Handle, (uint)index, timeout);
#pragma warning restore CA2000 // Dispose objects before losing scope

        try
        {
            if (state.Execute(Handle))
            {
                var sector = state.GetResult(Sector);

                state.Dispose();

                var result = Task.FromResult(sector);

                return result;
            }

            var error = Marshal.GetLastPInvokeError();

            if (error is not NativeConstants.ERROR_IO_PENDING)
            {
                throw new Win32Exception(error);
            }

            var handle = ThreadPool.RegisterWaitForSingleObject(
                state.Event, ReadSectorAsyncWindowsCallBack, state, TimeSpan.FromSeconds(timeout), true);

            var task = state.Source.Task;

            task.ContinueWith(_ => { handle.Unregister(null); }, TaskScheduler.Current);

            return task;
        }
        catch (Exception)
        {
            state.Dispose();
            throw;
        }
    }

    [SupportedOSPlatform("windows")]
    private void ReadSectorAsyncWindowsCallBack(object? state, bool timedOut)
    {
        var s = (ReadSectorAsyncWindowsState)state!;

        try
        {
            if (timedOut)
            {
                s.Source.SetCanceled();
            }
            else
            {
                var sector = s.GetResult(Sector);

                s.Source.SetResult(sector);
            }
        }
        catch (Exception e)
        {
            s.Source.SetException(e);
        }
        finally
        {
            s.Dispose();
        }
    }

    private sealed unsafe class ReadSectorAsyncWindowsState : Disposable
    {
        public ReadSectorAsyncWindowsState(SafeFileHandle handle, uint position, uint timeout = 3u)
        {
            Memory = Disc.GetDeviceAlignedBuffer(2352, handle);

            Query = Disc.ReadSectorWindowsQuery(position, 1u, timeout, Memory.Pointer, Memory.Length);

            Event = new ManualResetEvent(false);

            var overlapped = new Overlapped(0, 0, Event.SafeWaitHandle.DangerousGetHandle(), null);

            Overlapped = overlapped.Pack(null, null);
        }

        public ManualResetEvent Event { get; }

        public TaskCompletionSource<ISector> Source { get; } = new();

        public NativeMarshaller<NativeTypes.SCSI_PASS_THROUGH_DIRECT> Query { get; }

        public NativeMemory<byte> Memory { get; }

        public NativeOverlapped* Overlapped { get; }

        protected override void DisposeNative()
        {
            Event.Dispose();
            System.Threading.Overlapped.Free(Overlapped);
            Query.Dispose();
            Memory.Dispose();
        }

        public bool Execute(SafeFileHandle handle)
        {
            var ioctl = NativeMethods.DeviceIoControl(
                handle,
                NativeConstants.IOCTL_SCSI_PASS_THROUGH_DIRECT,
                Query.Pointer, (uint)Query.Length, Query.Pointer, (uint)Query.Length,
                out _,
                Overlapped
            );

            return ioctl;
        }

        public ISector GetResult(ISector sector)
        {
            var result = ISector.Read(sector, Memory.Span);

            return result;
        }
    }
}