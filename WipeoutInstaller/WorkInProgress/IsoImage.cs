using System.Text;
using WipeoutInstaller.Extensions;
using WipeoutInstaller.Iso9660;
using WipeoutInstaller.XA;

namespace WipeoutInstaller.WorkInProgress;

public sealed class IsoImage : Disposable
// TODO allow other sector types
{
    private readonly BinaryReader Reader;

    public IsoImage(Stream stream)
    {
        var length = stream.Length;

        var type = true switch
        {
            true when length % 2048 == 0 => IsoImageSectorType.Cooked,
            true when length % 2352 == 0 => IsoImageSectorType.Raw,
            _                            => IsoImageSectorType.Unknown
        };

        if (type is not IsoImageSectorType.Raw)
        {
            throw new NotImplementedException(type.ToString());
        }

        Reader = new BinaryReader(stream, Encoding.ASCII, true);

        Descriptors = GetVolumeDescriptors();

        PrimaryVolumeDescriptor = Descriptors.OfType<PrimaryVolumeDescriptor>().Single();
    }

    public Action<object?>? Logger { get; set; }

    private List<VolumeDescriptor> Descriptors { get; }

    private PrimaryVolumeDescriptor PrimaryVolumeDescriptor { get; }

    protected override void DisposeManaged()
    {
        Reader.Dispose();
    }

    private IList<PathTableRecord> GetPathTableRecords()
    {
        SetSectorUserData(PrimaryVolumeDescriptor.LocationOfOccurrenceOfTypeLPathTable);

        var position = Reader.BaseStream.Position;

        var records = new List<PathTableRecord>();

        while (Reader.BaseStream.Position < position + PrimaryVolumeDescriptor.PathTableSize)
        {
            records.Add(new PathTableRecord(Reader));
        }

        return records;
    }

    private IDictionary<PathTableRecord, IList<DirectoryRecord>> GetDirectoryRecords(IEnumerable<PathTableRecord> pathTableRecords)
    {
        var dictionary = new Dictionary<PathTableRecord, IList<DirectoryRecord>>();

        foreach (var pathTableRecord in pathTableRecords)
        {
            var ptrExtent = pathTableRecord.LocationOfExtent;

            SetSectorUserData(ptrExtent);


            while (true)
            {
                var directoryRecord = new DirectoryRecord(Reader);

                if (directoryRecord.LengthOfDirectoryRecord == 0)
                {
                    break;
                }

                var directoryRecords = dictionary.GetOrAdd(pathTableRecord, () => new List<DirectoryRecord>());

                directoryRecords.Add(directoryRecord);
            }
        }

        return dictionary;
    }

    private List<VolumeDescriptor> GetVolumeDescriptors()
    {
        var sector = 16;

        var descriptors = new List<VolumeDescriptor>();

        while (true)
        {
            SetSectorUserData(sector);

            var descriptor = new VolumeDescriptor
            {
                VolumeDescriptorType    = Reader.Read<VolumeDescriptorType>(), // 711
                StandardIdentifier      = Reader.ReadStringAscii(5),
                VolumeDescriptorVersion = new Iso711(Reader)
            };

            descriptor = descriptor.VolumeDescriptorType switch
            {
                VolumeDescriptorType.PrimaryVolumeDescriptor       => new PrimaryVolumeDescriptor(descriptor, Reader),
                VolumeDescriptorType.VolumeDescriptorSetTerminator => new VolumeDescriptorSetTerminator(descriptor, Reader),
                _                                                  => throw new NotImplementedException()
            };

            descriptors.Add(descriptor);

            if (descriptor is VolumeDescriptorSetTerminator)
            {
                break; // TODO add a mechanism to read N max descriptors
            }

            sector++;
        }

        return descriptors;
    }

    private void SetSectorUserData(int sector)
    {
        Reader.BaseStream.Position = sector * SectorConstants.Size + SectorConstants.UserDataPosition; // TODO support other sector types
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

    public IsoDirectory GetRootDirectory()
    {
        var pathTableRecords = GetPathTableRecords();
        var directoryRecords = GetDirectoryRecords(pathTableRecords);

        var dictionary = pathTableRecords.ToDictionary(s => s.LocationOfExtent.ToInt32(), s => s);

        var pathTable1 = pathTableRecords.First();
        var directory1 = directoryRecords[pathTable1].First();

        var firstDirectory = new IsoDirectory(null, directory1);

        var stack = new Stack<(IsoDirectory, PathTableRecord)>();

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

                    var child = new IsoDirectory(parent, record);

                    parent.Directories.Add(child);

                    stack.Push((child, dictionary[record.LocationOfExtent]));
                }
                else
                {
                    var child = new IsoFile(parent, record);

                    parent.Files.Add(child);
                }
            }
        }

        return firstDirectory;
    }
}