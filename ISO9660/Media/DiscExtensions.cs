using ISO9660.FileSystem;

namespace ISO9660.Media;

public static class DiscExtensions
{
    public static void ReadFileRaw(this Disc disc, IsoFileSystemEntryFile file, Stream stream)
    {
        ReadFile(disc, file, stream, ReadFileRaw);
    }

    public static void ReadFileUser(this Disc disc, IsoFileSystemEntryFile file, Stream stream)
    {
        ReadFile(disc, file, stream, ReadFileUser);
    }

    private static void ReadFile(Disc disc, IsoFileSystemEntryFile file, Stream stream, ReadFileHandler handler)
    {
        var position = (int)file.Position;

        var track = disc.Tracks.FirstOrDefault(s => position >= s.Position)
                    ?? throw new InvalidOperationException("Failed to determine track for file.");

        var sectors = (int)Math.Ceiling((double)file.Length / track.Sector.GetUserDataLength());

        for (var i = position; i < position + sectors; i++)
        {
            var sector = track.ReadSector(i);

            var span = handler(file, stream, sector);

            stream.Write(span);
        }
    }

    private static Span<byte> ReadFileRaw(IsoFileSystemEntryFile file, Stream stream, ISector sector)
    {
        var data = sector.GetData();

        var size = data.Length;

        var span = data[..size];

        return span;
    }

    private static Span<byte> ReadFileUser(IsoFileSystemEntryFile file, Stream stream, ISector sector)
    {
        var data = sector.GetUserData();

        var size = (int)Math.Min(Math.Max(file.Length - stream.Length, 0), data.Length);

        var span = data[..size];

        return span;
    }

    private delegate Span<byte> ReadFileHandler(IsoFileSystemEntryFile file, Stream stream, ISector sector);
}