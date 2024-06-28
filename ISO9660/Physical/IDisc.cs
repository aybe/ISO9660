using System.Buffers.Binary;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using ISO9660.GoldenHawk;
using ISO9660.WorkInProgress;

namespace ISO9660.Physical;

public interface IDisc : IDisposable, IAsyncDisposable
{
    IReadOnlyList<ITrack> Tracks { get; }

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

            tracks[i] = new WorkInProgress.Track(track1.TrackNumber, address1, length, (track1.Control & 4) == 0);
        }

        return new Disc(tracks, handle);
    }
}