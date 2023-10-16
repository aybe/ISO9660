using WipeoutInstaller.Extensions;
using WipeoutInstaller.WorkInProgress;

namespace WipeoutInstaller.ISO9660;

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

    private static VolumeDescriptorSet ReadVolumeDescriptors(Disc disc)
    {
        var sectorIndex = 16;

        var descriptors = new VolumeDescriptorSet();

        while (true)
        {
            var sector = disc.ReadSector(sectorIndex);

            using var reader = sector.GetUserData().ToBinaryReader();

            var descriptor = new VolumeDescriptor
            {
                VolumeDescriptorType    = reader.Read<VolumeDescriptorType>(), // 711
                StandardIdentifier      = reader.ReadStringAscii(5),
                VolumeDescriptorVersion = new Iso711(reader)
            };

            descriptor = descriptor.VolumeDescriptorType switch
            {
                VolumeDescriptorType.PrimaryVolumeDescriptor       => new PrimaryVolumeDescriptor(descriptor, reader),
                VolumeDescriptorType.VolumeDescriptorSetTerminator => new VolumeDescriptorSetTerminator(descriptor, reader),
                _                                                  => throw new NotImplementedException(descriptor.VolumeDescriptorType.ToString())
            };

            descriptors.Add(descriptor);

            if (descriptor is VolumeDescriptorSetTerminator)
            {
                break; // TODO add a mechanism to read N max descriptors
            }

            sectorIndex++;
        }

        return descriptors;
    }

    private static IsoFileSystemEntryDirectory ReadRootDirectory(Disc disc, PrimaryVolumeDescriptor pvd)
    {
        var pathTableRecords = ReadPathTableRecords(disc, pvd);
        var directoryRecords = ReadDirectoryRecords(disc, pathTableRecords);

        var dictionary = pathTableRecords.ToDictionary(s => s.LocationOfExtent.ToInt32(), s => s);

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
                if (record.FileFlags.HasFlags(FileFlags.Directory))
                {
                    switch (record.FileIdentifier.Value) // ignore . and .. or infinite loop
                    {
                        case IsoString.Byte00:
                        case IsoString.Byte01:
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

    private static IList<PathTableRecord> ReadPathTableRecords(Disc disc, PrimaryVolumeDescriptor pvd)
    {
        var records = new List<PathTableRecord>();

        var pathTableRead = 0L;

        var pathTableSize = pvd.PathTableSize;

        var sectorIndex = pvd.LocationOfOccurrenceOfTypeLPathTable.ToInt32();

        while (pathTableRead < pathTableSize)
        {
            var sector = disc.ReadSector(sectorIndex);

            using var reader = sector.GetUserData().ToBinaryReader();

            while (pathTableRead < pathTableSize)
            {
                var recordPosition = reader.BaseStream.Position;

                var record = new PathTableRecord(reader);

                var recordLength = reader.BaseStream.Position - recordPosition;

                pathTableRead += recordLength;

                records.Add(record);

                var length = reader.BaseStream.Length - reader.BaseStream.Position;

                if (length < PathTableRecord.MinimumLength)
                {
                    break;
                }
            }

            sectorIndex++;
        }

        return records;
    }

    private static void ReadDirectoryRecords(Disc disc, ICollection<DirectoryRecord> records, int extent)
    {
        using var reader = disc.ReadSector(extent).GetUserData().ToBinaryReader();

        while (true)
        {
            var record = new DirectoryRecord(reader);

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

            var extent = pathTableRecord.LocationOfExtent.ToInt32();

            ReadDirectoryRecords(disc, records, extent++);

            var length = records[0].DataLength.ToInt32();

            var blocks = length / 2048 - 1; // TODO constant

            for (var i = 0; i < blocks; i++)
            {
                ReadDirectoryRecords(disc, records, extent++);
            }
        }

        return dictionary;
    }
}