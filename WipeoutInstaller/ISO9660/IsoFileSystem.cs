using WipeoutInstaller.Extensions;
using WipeoutInstaller.JSON;
using WipeoutInstaller.WorkInProgress;

namespace WipeoutInstaller.ISO9660;

public sealed class IsoFileSystem : Disposable
{
    public IsoFileSystem(Disc disc)
    {
        Disc = disc;

        Descriptors = GetVolumeDescriptors();

        PrimaryVolumeDescriptor = Descriptors.OfType<PrimaryVolumeDescriptor>().Single();
    }

    private Disc Disc { get; }

    public Action<object?>? Logger { get; set; }

    private List<VolumeDescriptor> Descriptors { get; }

    private PrimaryVolumeDescriptor PrimaryVolumeDescriptor { get; }

    private List<VolumeDescriptor> GetVolumeDescriptors()
    {
        var sectorIndex = 16;

        var descriptors = new List<VolumeDescriptor>();

        while (true)
        {
            var sector = Disc.ReadSector(sectorIndex);

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

    public IsoFileSystemEntryDirectory GetRootDirectory()
    {
        var pathTableRecords = GetPathTableRecords();
        var directoryRecords = GetDirectoryRecords(pathTableRecords);

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

    private IList<PathTableRecord> GetPathTableRecords()
    {
        var records = new List<PathTableRecord>();

        var sector = Disc.ReadSector(PrimaryVolumeDescriptor.LocationOfOccurrenceOfTypeLPathTable);

        using var reader = sector.GetUserData().ToBinaryReader();

        var pathTableRead = 0L;

        var pathTableSize = PrimaryVolumeDescriptor.PathTableSize;

        if (pathTableSize > 2048) // TODO is temporary guard, check specifications and adjust
        {
            throw new NotImplementedException("The path table spans over multiple sectors.");
        }

        while (pathTableRead < pathTableSize)
        {
            var recordPosition = reader.BaseStream.Position;

            var record = new PathTableRecord(reader);

            var recordLength = reader.BaseStream.Position - recordPosition;

            pathTableRead += recordLength;

            records.Add(record);
        }

        return records;
    }

    private void GetDirectoryRecords(ICollection<DirectoryRecord> records, int extent)
    {
        using var reader = Disc.ReadSector(extent).GetUserData().ToBinaryReader();

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

    private IDictionary<PathTableRecord, IList<DirectoryRecord>> GetDirectoryRecords(IEnumerable<PathTableRecord> pathTableRecords)
    {
        var dictionary = new Dictionary<PathTableRecord, IList<DirectoryRecord>>();

        foreach (var pathTableRecord in pathTableRecords)
        {
            var records = dictionary.GetOrAdd(pathTableRecord, () => new List<DirectoryRecord>());

            var extent = pathTableRecord.LocationOfExtent.ToInt32();

            GetDirectoryRecords(records, extent++);

            var length = records[0].DataLength.ToInt32();

            var blocks = length / 2048 - 1; // TODO constant

            for (var i = 0; i < blocks; i++)
            {
                GetDirectoryRecords(records, extent++);
            }
        }

        return dictionary;
    }

    private void Log(object? value = null)
    {
        Logger?.Invoke(value);
    }

    private void LogJson(object? value = null)
    {
        var json = JsonUtility.ToJson(value);

        Log(json);
    }
}