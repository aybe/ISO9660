using System.Buffers.Binary;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Whatever.Extensions;

namespace ISO9660.WorkInProgress;

public sealed class Disc : Disposable
{
    public Disc(DriveInfo info)
    {
        if (info.DriveType is not DriveType.CDRom)
        {
            throw new ArgumentOutOfRangeException(nameof(info), "CD-ROM drive expected.");
        }

        if (info.IsReady is false)
        {
            throw new InvalidOperationException("CD-ROM drive is not ready.");
        }

        var handle = File.OpenHandle($@"\\.\{info.Name[..2]}", FileMode.Open, FileAccess.ReadWrite); // TODO async

        if (handle.IsInvalid)
        {
            throw new InvalidOperationException("CD-ROM drive handle is invalid.");
        }

        Handle = handle;

        Tracks = GetTracks().AsReadOnly();
    }

    private SafeFileHandle Handle { get; }

    public IReadOnlyList<Track> Tracks { get; }

    protected override void DisposeNative()
    {
        Handle.Dispose();
    }

    public NativeMemory<byte> GetDeviceAlignedBuffer(uint byteCount)
    {
        var alignment = GetDeviceAlignmentMask() + 1;

        var buffer = new NativeMemory<byte>(byteCount, alignment);

        return buffer;
    }

    private uint GetDeviceAlignmentMask()
    {
        var inBufferSize = (uint)Marshal.SizeOf<NativeTypes.STORAGE_PROPERTY_QUERY>();
        var inBuffer = Marshal.AllocHGlobal((int)inBufferSize);

        var outBufferSize = (uint)Marshal.SizeOf<NativeTypes.STORAGE_ADAPTER_DESCRIPTOR>();
        var outBuffer = Marshal.AllocHGlobal((int)outBufferSize);

        var query = new NativeTypes.STORAGE_PROPERTY_QUERY
        {
            QueryType = NativeTypes.STORAGE_QUERY_TYPE.PropertyStandardQuery,
            PropertyId = NativeTypes.STORAGE_PROPERTY_ID.StorageAdapterProperty,
        };

        Marshal.StructureToPtr(query, inBuffer, false);

        var ioctl = NativeMethods.DeviceIoControl(
            Handle.DangerousGetHandle(),
            NativeConstants.IOCTL_STORAGE_QUERY_PROPERTY,
            inBuffer,
            inBufferSize,
            outBuffer,
            outBufferSize,
            out _
        );

        var alignmentMask = 0u;

        if (ioctl)
        {
            var descriptor = Marshal.PtrToStructure<NativeTypes.STORAGE_ADAPTER_DESCRIPTOR>(outBuffer);

            alignmentMask = descriptor.AlignmentMask;
        }

        Marshal.FreeHGlobal(inBuffer);
        Marshal.FreeHGlobal(outBuffer);

        if (ioctl is false)
        {
            throw new Win32Exception();
        }

        return alignmentMask;
    }

    private Track[] GetTracks()
    {
        var inBufferSize = (uint)Marshal.SizeOf<NativeTypes.CDROM_READ_TOC_EX>();
        var inBuffer = Marshal.AllocHGlobal((int)inBufferSize);

        var outBufferSize = (uint)Marshal.SizeOf<NativeTypes.CDROM_TOC>();
        var outBuffer = Marshal.AllocHGlobal((int)outBufferSize);

        var ex = new NativeTypes.CDROM_READ_TOC_EX
        {
            Format = NativeConstants.CDROM_READ_TOC_EX_FORMAT_TOC,
            SessionTrack = 1,
        };

        Marshal.StructureToPtr(ex, inBuffer, false);

        var ioctl = NativeMethods.DeviceIoControl(
            Handle.DangerousGetHandle(),
            NativeConstants.IOCTL_CDROM_READ_TOC_EX,
            inBuffer,
            inBufferSize,
            outBuffer,
            outBufferSize,
            out _
        );

        var toc = ioctl ? Marshal.PtrToStructure<NativeTypes.CDROM_TOC>(outBuffer) : default;

        Marshal.FreeHGlobal(inBuffer);
        Marshal.FreeHGlobal(outBuffer);

        if (ioctl is false)
        {
            throw new Win32Exception();
        }

        var datas = toc.TrackData!;

        if (Array.FindIndex(datas, s => s.TrackNumber == toc.FirstTrack) is var index && index is -1)
        {
            throw new InvalidOperationException("Failed to find first track from index.");
        }

        var array = new Track[toc.LastTrack - toc.FirstTrack + 1];

        for (var i = 0; i < array.Length; i++)
        {
            var track1 = datas[i + index + 0];
            var track2 = datas[i + index + 1];

            var address1 = BinaryPrimitives.ReadInt32BigEndian(track1.Address);
            var address2 = BinaryPrimitives.ReadInt32BigEndian(track2.Address);

            var length = address2 - address1;

            var type = (track1.Control & 4) == 0 ? TrackType.Audio : TrackType.Data;

            array[i] = new Track(track1.TrackNumber, address1, length, type);
        }

        return array;
    }

    /// <remarks>
    ///     <see cref="GetDeviceAlignedBuffer" />
    /// </remarks>
    public unsafe void ReadSector(uint position, Span<byte> buffer, uint timeout = 3)
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
                Length = (ushort)inBufferSize,
                DataIn = NativeConstants.SCSI_IOCTL_DATA_IN,
                DataTransferLength = (uint)buffer.Length,
                DataBuffer = (nint)data,
                TimeOutValue = timeout,
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
}