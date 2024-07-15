using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using ISO9660.Extensions;
using Microsoft.Win32.SafeHandles;
using Whatever.Extensions;

namespace ISO9660.Physical;

internal sealed class TrackRaw(int index, int position, int length, bool audio, ISector sector, SafeFileHandle handle)
    : Track(audio, index, length, position, sector)
{
    private NativeMemory<byte> Buffer { get; } = Disc.GetDeviceAlignedBuffer(2352, handle);

    private SafeFileHandle Handle { get; } = handle;

    public override ISector ReadSector(int index)
    {
        if (OperatingSystem.IsWindows())
        {
            return ReadSectorWindows(index);
        }

        throw new PlatformNotSupportedException();
    }

    private void DisposeCore()
    {
        Buffer.Dispose();
    }

    protected override ValueTask DisposeAsyncCore()
    {
        DisposeCore();
        return ValueTask.CompletedTask;
    }

    protected override void DisposeManaged()
    {
        DisposeCore();
    }

    public override Task<ISector> ReadSectorAsync(int index)
    {
        if (OperatingSystem.IsWindows())
        {
            return ReadSectorWindowsAsync(index);
        }

        throw new PlatformNotSupportedException();
    }

    [SupportedOSPlatform("windows")]
    private ISector ReadSectorWindows(int index)
    {
        var buffer = Buffer.Manager.Memory.Span;

        Disc.ReadSector(Handle.DangerousGetHandle(), (uint)index, buffer);

        var sector = ISector.Read(Sector, buffer);

        return sector;
    }

    [SupportedOSPlatform("windows")]
    private async Task<ISector> ReadSectorWindowsAsync(int index, uint timeout = 3u)
    {
        var query = Disc.ReadSectorWindowsQuery((uint)index, 1u, timeout, Buffer.Pointer, Buffer.Length);

        var state = new ReadSectorWindowsAsyncState();

        await using var y = query.ConfigureAwait(false);
        await using var z = state.ConfigureAwait(false);

        var source = new TaskCompletionSource();

        var handle = ReadSectorWindowsAsync(query, state, source, TimeSpan.FromSeconds(timeout));

        if (handle != null)
        {
            var error = Marshal.GetLastPInvokeError();

            if (error is not (NativeConstants.ERROR_SUCCESS or NativeConstants.ERROR_IO_PENDING))
            {
                throw new Win32Exception(error);
            }

            await source.Task.ConfigureAwait(false);

            handle.Unregister(null);
        }

        var sector = ISector.Read(Sector, Buffer.Manager.Memory.Span);

        return sector;
    }

    [SupportedOSPlatform("windows")]
    private unsafe RegisteredWaitHandle? ReadSectorWindowsAsync(
        NativeMarshaller<NativeTypes.SCSI_PASS_THROUGH_DIRECT> query,
        ReadSectorWindowsAsyncState state,
        TaskCompletionSource source,
        TimeSpan timeout)
    {
        var overlapped =
            new Overlapped(0, 0, state.Event.SafeWaitHandle.DangerousGetHandle(), null)
                .Pack(null, null);

        var ioctl = NativeMethods.DeviceIoControl(
            Handle,
            NativeConstants.IOCTL_SCSI_PASS_THROUGH_DIRECT,
            query.Pointer, (uint)query.Length, query.Pointer, (uint)query.Length,
            out _,
            overlapped
        );

        if (ioctl is false)
        {
            return ThreadPool.RegisterWaitForSingleObject(state.Event, ReadSectorWindowsAsync, source, timeout, true);
        }

        Overlapped.Free(overlapped); // implicit when async...

        return null;
    }

    [SupportedOSPlatform("windows")]
    private static void ReadSectorWindowsAsync(object? state, bool timedOut)
    {
        var source = state as TaskCompletionSource
                     ?? throw new ArgumentOutOfRangeException(nameof(state));

        try
        {
            if (timedOut)
            {
                source.SetCanceled();
            }
            else
            {
                source.SetResult();
            }
        }
        catch (Exception e)
        {
            source.SetException(e);
        }
    }

    [SupportedOSPlatform("windows")]
    private sealed class ReadSectorWindowsAsyncState : DisposableAsync
    // for the magic of 'await using'
    {
        public readonly ManualResetEvent Event = new(false);

        private void DisposeCore()
        {
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
    }
}