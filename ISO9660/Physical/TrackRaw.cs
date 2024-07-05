using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace ISO9660.Physical;

internal sealed class TrackRaw : Track
{
    private readonly IOCompletionCallback _ioCompletionCallback;
    private ManualResetEvent _manualResetEvent;
    private unsafe NativeOverlapped* _nativeOverlapped;
    private PreAllocatedOverlapped _preAllocatedOverlapped;
    private ThreadPoolBoundHandle _threadPoolBoundHandle;

    public unsafe TrackRaw(int index, int position, int length, bool audio, ISector sector, SafeFileHandle handle)
    {
        Index = index;

        Position = position;

        Length = length;

        Audio = audio;

        Sector = sector;

        Handle = handle;

        return; // TODO

        if (audio)
        {
            return; // TODO
        }

        _ioCompletionCallback = Callback;

        _threadPoolBoundHandle = ThreadPoolBoundHandle.BindHandle(handle); // BUG works only once

        object? state = null;   // TODO
        object? pinData = null; // TODO
        _preAllocatedOverlapped = PreAllocatedOverlapped.UnsafeCreate(_ioCompletionCallback, state, pinData);

        _nativeOverlapped = _threadPoolBoundHandle.AllocateNativeOverlapped(_preAllocatedOverlapped);

        _manualResetEvent = new ManualResetEvent(false); // TODO slim?

        _nativeOverlapped->EventHandle = _manualResetEvent.SafeWaitHandle.DangerousGetHandle();
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

    protected override unsafe void DisposeManaged()
    {
        return; // TODO

        _threadPoolBoundHandle.FreeNativeOverlapped(_nativeOverlapped);

        _threadPoolBoundHandle.Dispose();

        _preAllocatedOverlapped.Dispose();

        _manualResetEvent.Dispose();
    }

    public override async Task<ISector> ReadSectorAsync(int index)
        // TODO async DeviceIoControl -> PreAllocatedOverlapped, ThreadPoolBoundHandle
    {
        try
        {
            ReadSectorAsyncInternalInit();

            var buffer = new byte[2352]; // TODO align
            var timeout = 3u;            // TODO param
            var position = (uint)index;  // TODO

            var ioctl = ReadSectorAsyncInternal(position, timeout, buffer);

            if (ioctl is false) // TODO delete?
            {
                var error = Marshal.GetLastPInvokeError();

                if (error is not ERROR_IO_PENDING)
                {
                    throw new Win32Exception(error);
                }
            }

            await AsTask(_manualResetEvent).ConfigureAwait(false);

            ISector sector = Sector switch // TODO DRY
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

            return await Task.FromResult(sector);
            return sector; // TODO?
        }
        finally
        {
            ReadSectorAsyncInternalFree();
        }
    }

    private unsafe void ReadSectorAsyncInternalInit()
    {
        if (Audio)
        {
            return; // TODO
        }

        _threadPoolBoundHandle = ThreadPoolBoundHandle.BindHandle(Handle);

        _preAllocatedOverlapped = PreAllocatedOverlapped.UnsafeCreate(IoCompletionCallback, null, null);

        _manualResetEvent = new ManualResetEvent(false);

        _nativeOverlapped = _threadPoolBoundHandle.AllocateNativeOverlapped(_preAllocatedOverlapped);

        _nativeOverlapped->EventHandle = _manualResetEvent.SafeWaitHandle.DangerousGetHandle();

        return;

        static void IoCompletionCallback(uint code, uint bytes, NativeOverlapped* overlap) // TODO
        {
            Debug.WriteLine($"{nameof(code)}: {code}");
            Debug.WriteLine($"{nameof(bytes)}: {bytes}");
        }
    }

    private unsafe void ReadSectorAsyncInternalFree()
    {
        if (Audio)
        {
            return; // TODO
        }

        _threadPoolBoundHandle.FreeNativeOverlapped(_nativeOverlapped);

        _threadPoolBoundHandle.Dispose();

        _preAllocatedOverlapped.Dispose();

        _manualResetEvent.Dispose();

        _nativeOverlapped = null;
    }

    private unsafe bool ReadSectorAsyncInternal(uint position, uint timeout, byte[] buffer)
    {
        fixed (byte* data = buffer)
        {
            var inBufferSize = (uint)Marshal.SizeOf<NativeTypes.SCSI_PASS_THROUGH_DIRECT>();
            var inBuffer = Marshal.AllocHGlobal((int)inBufferSize);

            // ReSharper disable once ConvertToConstant.Local
            var transfer = 1u; // sectors

            var sptd = new NativeTypes.SCSI_PASS_THROUGH_DIRECT(12) // TODO DRY
            {
                Length             = (ushort)inBufferSize,
                DataIn             = NativeConstants.SCSI_IOCTL_DATA_IN,
                DataTransferLength = (uint)buffer.Length,
                DataBuffer         = (nint)data,
                TimeOutValue       = timeout,
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
            };

            Marshal.StructureToPtr(sptd, inBuffer, false);

            var ioctl = NativeMethods.DeviceIoControl(
                Handle.DangerousGetHandle(),
                NativeConstants.IOCTL_SCSI_PASS_THROUGH_DIRECT,
                inBuffer,
                inBufferSize,
                inBuffer,
                inBufferSize,
                out var bytesReturned, // TODO
                _nativeOverlapped
            );

            Marshal.FreeHGlobal(inBuffer);

            return ioctl;
        }
    }

    private static Task AsTask(ManualResetEvent @event, int timeOutMs = 3000)
    {
        // https://learn.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/interop-with-other-asynchronous-patterns-and-types#from-wait-handles-to-tap

        if (@event == null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        var tcs = new TaskCompletionSource<bool>();

        var rwh = ThreadPool.RegisterWaitForSingleObject(@event, (state, timedOut) =>
        {
            if (timedOut)
            {
                tcs.TrySetCanceled();
            }
            else
            {
                tcs.TrySetResult(true);
            }
        }, null, timeOutMs, true);

        var task = tcs.Task;

        task.ContinueWith(_ => rwh.Unregister(null));

        return task;
    }

    private unsafe void Callback(uint errorCode, uint numBytes, NativeOverlapped* pOVERLAP)
    {
        Debug.WriteLine($"{nameof(errorCode)}: {errorCode}");
        Debug.WriteLine($"{nameof(numBytes)}: {numBytes}");
        //throw new NotImplementedException();
    }

    #region winerror.h // TODO move

    public const int ERROR_SUCCESS = 0;

    public const int ERROR_IO_PENDING = 997;

    #endregion
}