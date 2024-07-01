using System.Buffers.Binary;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using ISO9660.Extensions;
using ISO9660.GoldenHawk;
using Microsoft.Win32.SafeHandles;
using Whatever.Extensions;

namespace ISO9660.Physical;

public sealed class Disc : DisposableAsync
{
    private Disc(IReadOnlyList<Track> tracks, SafeFileHandle? handle = null)
    {
        Tracks = tracks;
        Handle = handle;
    }

    private SafeFileHandle? Handle { get; }

    public IReadOnlyList<Track> Tracks { get; }

    public NativeMemory<byte> GetDeviceAlignedBuffer(uint byteCount)
    {
        var memory = GetDeviceAlignedBuffer(byteCount, Handle);

        return memory;
    }

    public void ReadSector(uint position, Span<byte> buffer, uint timeout = 3)
    {
        if (position < Tracks[0].Position || position >= Tracks[^1].Position + Tracks[^1].Length)
        {
            throw new ArgumentOutOfRangeException(nameof(position), position, "Invalid sector position.");
        }

        if (Handle is null)
        {
            throw new NotImplementedException();
        }

        ReadSector(Handle.DangerousGetHandle(), position, buffer, timeout);
    }

    public static void ReadSector(nint handle, uint position, Span<byte> buffer, uint timeout = 3)
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
                handle,
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

    [SuppressMessage("ReSharper", "RedundantIfElseBlock")]
    private static ISector GetSectorType(Span<byte> buffer, bool verify = true)
    {
        var header = MemoryMarshal.Read<SectorHeader>(buffer[12..16]);

        var mode = header.Mode;

        switch (mode)
        {
            case SectorMode.Mode0:
                return new SectorRawMode0();
            case SectorMode.Mode1:
                if (verify)
                {
                    EDC.Validate(buffer[..2064], buffer[2064..2068]);
                }
                return new SectorRawMode1();
            case SectorMode.Mode2:
                var header1 = MemoryMarshal.Read<SectorMode2SubHeader>(buffer[16..20]);
                var header2 = MemoryMarshal.Read<SectorMode2SubHeader>(buffer[20..24]);

                if (header1 != header2)
                {
                    return new SectorRawMode2FormLess(); // assumption
                }

                if (header1.SubMode.HasFlags(SectorMode2SubMode.Form))
                {
                    if (verify)
                    {
                        EDC.Validate(buffer[16..2332], buffer[2348..2352]);
                    }

                    return new SectorRawMode2Form2();
                }
                else
                {
                    if (verify)
                    {
                        EDC.Validate(buffer[16..2072], buffer[2072..2076]);
                    }

                    return new SectorRawMode2Form1();
                }
            case SectorMode.Reserved:
            default:
                throw new NotSupportedException(mode.ToString());
        }
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

    private static Disc OpenRawWindows(DriveInfo info)
    {
        var handle = File.OpenHandle($@"\\.\{info.Name[..2]}", FileMode.Open, FileAccess.ReadWrite); // TODO async

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

        using var memory = GetDeviceAlignedBuffer(2352 /*TODO*/, handle);

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
                var buffer = memory.Span;

                ReadSector(handle.DangerousGetHandle(), (uint)address1, buffer);

                sector = GetSectorType(buffer);
            }

            tracks[i] = new TrackRaw(track1.TrackNumber, address1, length, audio, sector, handle);
        }

        return new Disc(tracks, handle);
    }
}