using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using ISO9660.WorkInProgress;
using Microsoft.Win32.SafeHandles;
using Whatever.Extensions;

namespace ISO9660.Physical;

internal sealed class Disc : DisposableAsync, IDisc
{
    public Disc(IReadOnlyList<ITrack> tracks, SafeFileHandle? handle = null)
    {
        Tracks = tracks;
        Handle = handle;
    }

    private SafeFileHandle? Handle { get; }

    public IReadOnlyList<ITrack> Tracks { get; }

    public NativeMemory<byte> GetDeviceAlignedBuffer(uint byteCount)
    {
        var memory = IDisc.GetDeviceAlignedBuffer(byteCount, Handle);

        return memory;
    }

    public void ReadSector(uint position, Span<byte> buffer, uint timeout = 3)
    {
        if (Handle is null)
        {
            throw new NotImplementedException();
        }

        if (OperatingSystem.IsWindows())
        {
            ReadSectorWindows(position, buffer, timeout);
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }

    [SupportedOSPlatform("windows")]
    private unsafe void ReadSectorWindows(uint position, Span<byte> buffer, uint timeout = 3)
    {
        if (position < Tracks[0].Position || position >= Tracks[^1].Position + Tracks[^1].Length)
        {
            throw new ArgumentOutOfRangeException(nameof(position), position, "Invalid sector position.");
        }

        if (buffer.Length < 2352)
        {
            throw new ArgumentOutOfRangeException(nameof(buffer), "Buffer length must be >= 2352 bytes.");
        }

        fixed (byte* data = &MemoryMarshal.GetReference(buffer))
        {
            var inBufferSize = (uint)Marshal.SizeOf<NativeTypes.SCSI_PASS_THROUGH_DIRECT>();
            var inBuffer = Marshal.AllocHGlobal((int)inBufferSize);

            // ReSharper disable once ConvertToConstant.Local
            var transfer = 1u; // sectors

            var sptd = new NativeTypes.SCSI_PASS_THROUGH_DIRECT(12)
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
                out _
            );

            Marshal.FreeHGlobal(inBuffer);

            if (ioctl is false)
            {
                throw new Win32Exception();
            }
        }
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        foreach (var track in Tracks)
        {
            await track.DisposeAsync().ConfigureAwait(false);
        }
    }

    protected override void DisposeManaged()
    {
        foreach (var track in Tracks)
        {
            track.Dispose();
        }
    }
}