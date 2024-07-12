﻿using System.ComponentModel;
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

        await using var x = data.ConfigureAwait(false);
        await using var y = sptd.ConfigureAwait(false);

#pragma warning disable CA2000 // Dispose objects before losing scope // gets disposed in callback
        var state = new ReadSectorAsyncWindowsState();
#pragma warning restore CA2000 // Dispose objects before losing scope

        try
        {
            if (state.Execute(Handle, sptd))
            {
                var sector = ISector.Read(Sector, data.Span);

                state.Dispose();

                return sector;
            }

            var error = Marshal.GetLastPInvokeError();

            if (error is not NativeConstants.ERROR_IO_PENDING)
            {
                throw new Win32Exception(error);
            }

            var handle = state.GetHandle(ReadSectorAsyncWindowsCallBack, TimeSpan.FromSeconds(timeout));

            await state.Source.Task;

            handle.Unregister(null);

            return ISector.Read(Sector, data.Span);
        }
        catch (Exception)
        {
            state.Dispose();
            throw;
        }
    }

    [SupportedOSPlatform("windows")]
    private static void ReadSectorAsyncWindowsCallBack(object? state, bool timedOut)
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
                s.Source.SetResult();
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
        public ReadSectorAsyncWindowsState()
        {
            Event = new ManualResetEvent(false);
        }

        private ManualResetEvent Event { get; }

        public TaskCompletionSource Source { get; } = new();

        protected override void DisposeNative()
        {
            Event.Dispose();
        }

        public bool Execute(SafeFileHandle handle, NativeMarshaller<NativeTypes.SCSI_PASS_THROUGH_DIRECT> sptd)
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

            return ioctl;
        }

        public RegisteredWaitHandle GetHandle(WaitOrTimerCallback callback, TimeSpan timeout)
        {
            return ThreadPool.RegisterWaitForSingleObject(Event, callback, this, timeout, true);
        }
    }
}