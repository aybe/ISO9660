using System.Diagnostics.CodeAnalysis;
using System.Text;
using ISO9660.Physical;
using Whatever.Extensions;

namespace ISO9660.Logical;

public sealed class IsoFileSystem : Disposable
{
    private IsoFileSystem(VolumeDescriptorSet descriptorSet, IsoFileSystemEntryDirectory rootDirectory)
    {
        DescriptorSet = descriptorSet;
        RootDirectory = rootDirectory;
    }

    public VolumeDescriptorSet DescriptorSet { get; }

    public IsoFileSystemEntryDirectory RootDirectory { get; }

    public static IsoFileSystem Read(Disc disc)
    {
        var descriptorSet = ReadVolumeDescriptors(disc);

        var rootDirectory = ReadRootDirectory(disc, descriptorSet.PrimaryVolumeDescriptor);

        var isoFileSystem = new IsoFileSystem(descriptorSet, rootDirectory);

        return isoFileSystem;
    }

    public bool TryFindFile(string filePath, [MaybeNullWhen(false)] out IsoFileSystemEntryFile result)
    {
        result = default;

        var split = filePath.Split(IsoFileSystemEntry.DirectorySeparator, StringSplitOptions.RemoveEmptyEntries);

        var queue = new Queue<string>(split);

        var stack = new Stack<IsoFileSystemEntryDirectory>();

        stack.Push(RootDirectory);

        while (stack.Count > 0 && queue.Count > 0)
        {
            var directory = stack.Pop();

            var entryName = queue.Peek();

            var entryFile = directory.Files
                .SingleOrDefault(s => string.Equals(s.FileName, entryName, StringComparison.Ordinal));

            if (entryFile != null)
            {
                result = entryFile;
                return true;
            }

            var entryDirectory = directory.Directories
                .SingleOrDefault(s => string.Equals(s.FileName, entryName, StringComparison.Ordinal));

            if (entryDirectory == null)
            {
                continue;
            }

            queue.Dequeue();

            stack.Push(entryDirectory);
        }

        return false;
    }

    private static VolumeDescriptorSet ReadVolumeDescriptors(Disc disc)
    {
        var sectorIndex = 16;

        var descriptors = new VolumeDescriptorSet();

        while (true)
        {
            // NOTE here we don't need a 'stream of sectors' as opposed to PTRs which can overlap sectors

            using var stream = disc.Tracks.First().GetStream(sectorIndex);

            var descriptor = new VolumeDescriptor(stream);

            if (Enum.IsDefined(descriptor.VolumeDescriptorType) == false)
            {
                throw new NotSupportedException($"Unknown volume descriptor type: '{descriptor.VolumeDescriptorType}'.");
            }

            if (descriptor.StandardIdentifier != "CD001")
            {
                throw new NotSupportedException($"Unknown volume standard identifier: '{descriptor.StandardIdentifier}'.");
            }

            descriptor = descriptor.VolumeDescriptorType switch
            {
                VolumeDescriptorType.BootRecord                    => new VolumeDescriptorBootRecord(descriptor, stream),
                VolumeDescriptorType.PrimaryVolumeDescriptor       => new VolumeDescriptorPrimary(descriptor, stream),
                VolumeDescriptorType.SupplementaryVolumeDescriptor => new VolumeDescriptorSupplementary(descriptor, stream),
                VolumeDescriptorType.VolumePartitionDescriptor     => new VolumeDescriptorPartition(descriptor, stream),
                VolumeDescriptorType.VolumeDescriptorSetTerminator => new VolumeDescriptorSetTerminator(descriptor, stream),
                _                                                  => throw new NotSupportedException(descriptor.VolumeDescriptorType.ToString())
            };

            descriptors.Add(descriptor);

            if (descriptor is VolumeDescriptorSetTerminator)
            {
                break;
            }

            sectorIndex++;
        }

        return descriptors;
    }

    private static IsoFileSystemEntryDirectory ReadRootDirectory(Disc disc, VolumeDescriptorPrimary pvd)
    {
        var pathTableRecords = ReadPathTableRecords(disc, pvd);
        var directoryRecords = ReadDirectoryRecords(disc, pathTableRecords);

        var dictionary = pathTableRecords.ToDictionary(s => s.LocationOfExtent, s => s);

        var pathTable1 = pathTableRecords.First();
        var directory1 = directoryRecords[pathTable1].First();

        var firstDirectory = new IsoFileSystemEntryDirectory(null, directory1);

        var stack = new Stack<(IsoFileSystemEntryDirectory, PathTableRecord)>();

        stack.Push((firstDirectory, pathTable1));

        while (stack.Any())
        {
            var (parent, popRec) = stack.Pop();

            var list = directoryRecords[popRec];

            foreach (var record in list)
            {
                if (record.FileFlags.HasFlags(DirectoryRecordFlags.Directory))
                {
                    switch (record.FileIdentifier) // ignore . and .. or infinite loop
                    {
                        case "\u0000":
                            continue;
                        case "\u0001":
                            continue;
                    }

                    var child = new IsoFileSystemEntryDirectory(parent, record);

                    parent.Directories.Add(child);

                    stack.Push((child, dictionary[record.LocationOfExtent]));
                }
                else
                {
                    var child = new IsoFileSystemEntryFile(parent, record);

                    parent.Files.Add(child);
                }
            }
        }

        return firstDirectory;
    }

    private static IList<PathTableRecord> ReadPathTableRecords(Disc disc, VolumeDescriptorPrimary pvd)
    {
        var records = new List<PathTableRecord>();

        var pathTableRead = 0L;

        var pathTableSize = pvd.PathTableSize;

        var track = disc.Tracks.First();

        using var stream = track.GetStream(Convert.ToInt32(pvd.LocationOfOccurrenceOfTypeLPathTable));

        while (pathTableRead < pathTableSize)
        {
            var recordPosition = stream.Position;

            var record = new PathTableRecord(stream);

            var recordLength = stream.Position - recordPosition;

            pathTableRead += recordLength;

            records.Add(record);
        }

        return records;
    }

    private static void ReadDirectoryRecords(Disc disc, ICollection<DirectoryRecord> records, uint extent)
    {
        using var stream = disc.Tracks.First().GetStream(Convert.ToInt32(extent));

        while (true)
        {
            var record = new DirectoryRecord(stream);

            if (record.LengthOfDirectoryRecord == 0)
            {
                break;
            }

            records.Add(record);
        }
    }

    private static IDictionary<PathTableRecord, IList<DirectoryRecord>> ReadDirectoryRecords(Disc disc, IEnumerable<PathTableRecord> pathTableRecords)
    {
        var dictionary = new Dictionary<PathTableRecord, IList<DirectoryRecord>>();

        foreach (var pathTableRecord in pathTableRecords)
        {
            var records = dictionary.GetOrAdd(pathTableRecord, () => new List<DirectoryRecord>());

            var extent = pathTableRecord.LocationOfExtent;

            ReadDirectoryRecords(disc, records, extent++);

            var length = records[0].DataLength;

            var blocks = length / 2048 - 1;

            for (var i = 0; i < blocks; i++)
            {
                ReadDirectoryRecords(disc, records, extent++);
            }
        }

        return dictionary;
    }

    public void Print(StringBuilder builder)
    {
        var stack = new Stack<(IsoFileSystemEntry Entry, int Depth)>();

        stack.Push((RootDirectory, 0));

        while (stack.Count > 0)
        {
            var (entry, i) = stack.Pop();

            builder.AppendLine($"{new string('\t', i)}{entry.FileName}");

            if (entry is not IsoFileSystemEntryDirectory directory)
            {
                continue;
            }

            foreach (var item in directory.Directories.AsEnumerable().Reverse())
            {
                stack.Push((item, i + 1));
            }

            foreach (var item in directory.Files.AsEnumerable().Reverse())
            {
                stack.Push((item, i + 1));
            }
        }
    }
}