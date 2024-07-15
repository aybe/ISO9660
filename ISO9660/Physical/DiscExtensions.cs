using ISO9660.Logical;
using Microsoft.Extensions.ObjectPool;
using Whatever.Extensions;

namespace ISO9660.Physical;

public static class DiscExtensions
{
    private static readonly DefaultObjectPool<SpanMemoryManager<byte>> ReadFileManager = new(new ReadFileManagerPolicy());

    public static async Task ReadFileRawAsync(this Disc disc, IsoFileSystemEntryFile file, Stream stream, IProgress<double>? progress = null)
    {
        await ReadFileAsync(disc, file, stream, ReadFileRaw, progress).ConfigureAwait(false);
    }

    public static async Task ReadFileUserAsync(this Disc disc, IsoFileSystemEntryFile file, Stream stream, IProgress<double>? progress = null)
    {
        await ReadFileAsync(disc, file, stream, ReadFileUser, progress).ConfigureAwait(false);
    }

    private static async Task ReadFileAsync(Disc disc, IsoFileSystemEntryFile file, Stream stream, ReadFileHandler handler, IProgress<double>? progress)
    {
        var position = (int)file.Position;

        var track = disc.Tracks.FirstOrDefault(s => position >= s.Position)
                    ?? throw new InvalidOperationException("Failed to determine track for file.");

        var sectors = (int)Math.Ceiling((double)file.Length / track.Sector.GetUserDataLength());

        using var manager = ReadFileManager.Get();

        for (var i = 0; i < sectors; i++)
        {
            var sector = await track.ReadSectorAsync(i + position).ConfigureAwait(false);

            handler(file, stream, sector, manager);

            await stream.WriteAsync(manager.Memory).ConfigureAwait(false);

            progress?.Report(1.0d / sectors * (i + 1));
        }

        ReadFileManager.Return(manager);
    }

    private static void ReadFileRaw(IsoFileSystemEntryFile file, Stream stream, ISector sector, SpanMemoryManager<byte> manager)
    {
        var data = sector.GetData();

        var size = data.Length;

        var span = data[..size];

        manager.SetSpan(span);
    }

    private static void ReadFileUser(IsoFileSystemEntryFile file, Stream stream, ISector sector, SpanMemoryManager<byte> manager)
    {
        var data = sector.GetUserData();

        var size = (int)Math.Min(Math.Max(file.Length - stream.Length, 0), data.Length);

        var span = data[..size];

        manager.SetSpan(span);
    }

    private sealed class ReadFileManagerPolicy : IPooledObjectPolicy<SpanMemoryManager<byte>>
    {
        public SpanMemoryManager<byte> Create()
        {
            return new SpanMemoryManager<byte>();
        }

        public bool Return(SpanMemoryManager<byte> obj)
        {
            using var manager = obj;

            return true;
        }
    }

    private delegate void ReadFileHandler(IsoFileSystemEntryFile file, Stream stream, ISector sector, SpanMemoryManager<byte> manager);
}