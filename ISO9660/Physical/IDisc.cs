using System.Buffers.Binary;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using ISO9660.GoldenHawk;
using ISO9660.WorkInProgress;
using Microsoft.Win32.SafeHandles;

namespace ISO9660.Physical;

public interface IDisc : IDisposable, IAsyncDisposable
{
    IReadOnlyList<ITrack> Tracks { get; }

    internal static NativeMemory<byte> GetDeviceAlignedBuffer(uint byteCount, SafeFileHandle? handle)
    {
        var alignment = handle is null ? 1 : GetDeviceAlignmentMask(handle.DangerousGetHandle()) + 1;

        var memory = new NativeMemory<byte>(byteCount, alignment);

        return memory;
    }

    internal static uint GetDeviceAlignmentMask(nint handle)
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

    NativeMemory<byte> GetDeviceAlignedBuffer(uint byteCount);

    /// <remarks>
    ///     <see cref="GetDeviceAlignedBuffer(uint)" />
    /// </remarks>
    public void ReadSector(uint position, Span<byte> buffer, uint timeout = 3);

    public static IDisc Open(string path)
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

    private static IDisc OpenCue(string path)
    {
        var sheet = CueSheetParser.Parse(path);

        var tracks = sheet.Files.SelectMany(s => s.Tracks).Select(s => new TrackCue(s)).ToList().AsReadOnly();

        var disc = new Disc(tracks);

        return disc;
    }

    private static IDisc OpenIso(string path)
    {
        var stream = File.OpenRead(path);

        var track = new TrackIso(stream, 1, 0);

        var disc = new Disc(new ReadOnlyObservableCollection<ITrack>([track]));

        return disc;
    }

    private static IDisc OpenRawWindows(DriveInfo info)
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

        var tracks = new ITrack[toc.LastTrack - toc.FirstTrack + 1];

        for (var i = 0; i < tracks.Length; i++)
        {
            var track1 = datas[i + index + 0];
            var track2 = datas[i + index + 1];

            var address1 = BinaryPrimitives.ReadInt32BigEndian(track1.Address);
            var address2 = BinaryPrimitives.ReadInt32BigEndian(track2.Address);

            var length = address2 - address1;

            // TODO sector type

            tracks[i] = new TrackRaw(track1.TrackNumber, address1, length, (track1.Control & 4) == 0);
        }

        return new Disc(tracks, handle);
    }
}