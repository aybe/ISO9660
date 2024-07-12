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
    private async Task<ISector> ReadSectorAsyncWindows(int index, uint timeout = 3u)
    {
        var data = Disc.GetDeviceAlignedBuffer(2352, Handle);
        var sptd = Disc.ReadSectorWindowsQuery((uint)index, 1u, timeout, data.Pointer, data.Length);

        var state = new ReadSectorAsyncWindowsState();

        await using var x = data.ConfigureAwait(false);
        await using var y = sptd.ConfigureAwait(false);
        await using var z = state.ConfigureAwait(false);

        try
        {
            if (state.Execute(Handle, sptd, TimeSpan.FromSeconds(timeout)))
            {
                var sector = ISector.Read(Sector, data.Span);

                return sector;
            }

            var error = Marshal.GetLastPInvokeError();

            if (error is not (NativeConstants.ERROR_SUCCESS or NativeConstants.ERROR_IO_PENDING))
            {
                throw new Win32Exception(error);
            }

            await state.Source.Task;

            return ISector.Read(Sector, data.Span);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private sealed unsafe class ReadSectorAsyncWindowsState : DisposableAsync
    {
        private readonly ManualResetEvent Event = new(false);

        private RegisteredWaitHandle? Handle;

        public TaskCompletionSource Source { get; } = new();

        private void DisposeCore()
        {
            Handle?.Unregister(null);

            Event.Dispose();
        }

        protected override void DisposeManaged()
        {
            DisposeCore();
        }

        protected override ValueTask DisposeAsyncCore()
        {
            DisposeCore();

            return ValueTask.CompletedTask;
        }

        private void Callback(object? state, bool timedOut)
        {
            try
            {
                if (timedOut)
                {
                    Source.SetCanceled();
                }
                else
                {
                    Source.SetResult();
                }
            }
            catch (Exception e)
            {
                Source.SetException(e);
            }
            finally
            {
                Dispose();
            }
        }

        public bool Execute(SafeFileHandle handle, NativeMarshaller<NativeTypes.SCSI_PASS_THROUGH_DIRECT> sptd, TimeSpan timeout)
        {
            var overlapped =
                new Overlapped(0, 0, Event.SafeWaitHandle.DangerousGetHandle(), null)
                    .Pack(null, null);

            var ioctl = NativeMethods.DeviceIoControl(
                handle,
                NativeConstants.IOCTL_SCSI_PASS_THROUGH_DIRECT,
                sptd.Pointer, (uint)sptd.Length, sptd.Pointer, (uint)sptd.Length,
                out _,
                overlapped
            );

            if (ioctl)
            {
                Overlapped.Free(overlapped);
            }
            else
            {
                Handle = ThreadPool.RegisterWaitForSingleObject(Event, Callback, this, timeout, true);
            }

            return ioctl;
        }
    }
}