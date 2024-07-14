using System.Buffers.Binary;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using ISO9660.Extensions;
using ISO9660.GoldenHawk;
using Microsoft.Win32.SafeHandles;
using Whatever.Extensions;

namespace ISO9660.Physical;

public sealed class Disc : DisposableAsync
{
    private Disc(IReadOnlyList<Track> tracks)
    {
        Tracks = tracks;
    }

    public IReadOnlyList<Track> Tracks { get; }

    internal static void ReadSector(nint handle, uint position, Span<byte> buffer, uint timeout = 3)
    {
        if (OperatingSystem.IsWindows())
        {
            ReadSectorWindows(handle, position, buffer, timeout);
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }

    [SupportedOSPlatform("windows")]
    private static unsafe void ReadSectorWindows(nint handle, uint position, Span<byte> buffer, uint timeout = 3)
    {
        if (buffer.Length < 2352)
        {
            throw new ArgumentOutOfRangeException(nameof(buffer), "Buffer length must be >= 2352 bytes.");
        }

        fixed (byte* data = &MemoryMarshal.GetReference(buffer))
        {
            // ReSharper disable once ConvertToConstant.Local
            var transfer = 1u; // sectors

            using var query = ReadSectorWindowsQuery(position, transfer, timeout, (nint)data, (uint)buffer.Length);

            var ioctl = NativeMethods.DeviceIoControl(
                handle,
                NativeConstants.IOCTL_SCSI_PASS_THROUGH_DIRECT,
                query.Pointer, (uint)query.Length, query.Pointer, (uint)query.Length,
                out _
            );

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

    internal static NativeMemory<byte> GetDeviceAlignedBuffer(uint byteCount, SafeFileHandle? handle)
    {
        var alignment = handle is null ? 1 : GetDeviceAlignmentMask(handle.DangerousGetHandle()) + 1;

        var memory = new NativeMemory<byte>(byteCount, alignment);

        return memory;
    }

    private static uint GetDeviceAlignmentMask(nint handle)
    {
        if (OperatingSystem.IsWindows())
        {
            return GetDeviceAlignmentMaskWindows(handle);
        }

        throw new PlatformNotSupportedException();
    }

    [SupportedOSPlatform("windows")]
    private static uint GetDeviceAlignmentMaskWindows(nint handle)
    {
        var inBufferSize = (uint)Marshal.SizeOf<NativeTypes.STORAGE_PROPERTY_QUERY>();
        var inBuffer = Marshal.AllocHGlobal((int)inBufferSize);

        var outBufferSize = (uint)Marshal.SizeOf<NativeTypes.STORAGE_ADAPTER_DESCRIPTOR>();
        var outBuffer = Marshal.AllocHGlobal((int)outBufferSize);

        var query = new NativeTypes.STORAGE_PROPERTY_QUERY
        {
            QueryType  = NativeTypes.STORAGE_QUERY_TYPE.PropertyStandardQuery,
            PropertyId = NativeTypes.STORAGE_PROPERTY_ID.StorageAdapterProperty,
        };

        Marshal.StructureToPtr(query, inBuffer, false);

        var ioctl = NativeMethods.DeviceIoControl(
            handle,
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

    public static Disc Open(string path)
    {
        var extension = Path.GetExtension(path);

        switch (extension.ToLowerInvariant())
        {
            case ".cue":
                return OpenCue(path);
            case ".iso":
                return OpenIso(path);
        }

        var info = new DriveInfo(path);

        if (info.DriveType is not DriveType.CDRom)
        {
            throw new ArgumentOutOfRangeException(nameof(path), path, "CD-ROM drive expected.");
        }

        if (OperatingSystem.IsWindows())
        {
            return OpenRawWindows(info);
        }

        throw new PlatformNotSupportedException();
    }

    private static Disc OpenCue(string path)
    {
        var sheet = CueSheetParser.Parse(path);

        var tracks = sheet.Files.SelectMany(s => s.Tracks).Select(s => new TrackCue(s)).ToList().AsReadOnly();

        var disc = new Disc(tracks);

        return disc;
    }

    private static Disc OpenIso(string path)
    {
        var stream = File.OpenRead(path);

        var track = new TrackIso(stream, 1, 0);

        var disc = new Disc(new ReadOnlyObservableCollection<Track>([track]));

        return disc;
    }

    [SupportedOSPlatform("windows")]
    private static Disc OpenRawWindows(DriveInfo info)
    {
        var handle = File.OpenHandle($@"\\.\{info.Name[..2]}", FileMode.Open, FileAccess.ReadWrite, FileShare.Read, FileOptions.Asynchronous);

        var inBufferSize = (uint)Marshal.SizeOf<NativeTypes.CDROM_READ_TOC_EX>();
        var inBuffer = Marshal.AllocHGlobal((int)inBufferSize);

        var outBufferSize = (uint)Marshal.SizeOf<NativeTypes.CDROM_TOC>();
        var outBuffer = Marshal.AllocHGlobal((int)outBufferSize);

        var ex = new NativeTypes.CDROM_READ_TOC_EX
        {
            Format       = NativeConstants.CDROM_READ_TOC_EX_FORMAT_TOC,
            SessionTrack = 1,
        };

        Marshal.StructureToPtr(ex, inBuffer, false);

        var ioctl = NativeMethods.DeviceIoControl(
            handle.DangerousGetHandle(),
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

        var tracks = new Track[toc.LastTrack - toc.FirstTrack + 1];

        using var memory = GetDeviceAlignedBuffer(2352, handle);

        for (var i = 0; i < tracks.Length; i++)
        {
            var track1 = datas[i + index + 0];
            var track2 = datas[i + index + 1];

            var address1 = BinaryPrimitives.ReadInt32BigEndian(track1.Address);
            var address2 = BinaryPrimitives.ReadInt32BigEndian(track2.Address);

            var length = address2 - address1;

            var audio = (track1.Control & 4) == 0;

            ISector sector;

            if (audio)
            {
                sector = new SectorRawAudio();
            }
            else
            {
                var buffer = memory.Manager.Memory.Span;

                ReadSector(handle.DangerousGetHandle(), (uint)address1, buffer);

                sector = ISector.GetSectorTypeRaw(buffer);
            }

            tracks[i] = new TrackRaw(track1.TrackNumber, address1, length, audio, sector, handle);
        }

        return new Disc(tracks);
    }

    [SupportedOSPlatform("windows")]
    internal static NativeMarshaller<NativeTypes.SCSI_PASS_THROUGH_DIRECT> ReadSectorWindowsQuery(
        uint position, uint transfer, uint timeOut, nint buffer, uint bufferLength)
    {
        return new NativeMarshaller<NativeTypes.SCSI_PASS_THROUGH_DIRECT>
        {
            Structure = new NativeTypes.SCSI_PASS_THROUGH_DIRECT(12)
            {
                Length             = (ushort)Marshal.SizeOf<NativeTypes.SCSI_PASS_THROUGH_DIRECT>(),
                DataIn             = NativeConstants.SCSI_IOCTL_DATA_IN,
                DataTransferLength = bufferLength,
                DataBuffer         = buffer,
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
}