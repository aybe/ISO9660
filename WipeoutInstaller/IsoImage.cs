using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using WipeoutInstaller.Extensions;
using WipeoutInstaller.Iso9660;
using WipeoutInstaller.XA;

namespace WipeoutInstaller;

public sealed class IsoImage : Disposable
// TODO allow other sector types
{
    private readonly BinaryReader Reader;

    public IsoImage(Stream stream)
    {
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
        Reader.BaseStream.Position = sector * SectorMode2.Size + SectorMode2.UserDataPosition; // TODO support other sector types
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
                if (record.FileFlags.HasFlagFast(FileFlags.Directory))
                {
                    if ((string)record.FileIdentifier is "." or "..")
                    {
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

public abstract partial class IsoFileSystemEntry
{
    [PublicAPI]
    public const char DirectorySeparator = '/';

    private readonly DirectoryRecord Record;

    protected IsoFileSystemEntry(IsoDirectory? parent, DirectoryRecord record)
    {
        Parent = parent;
        Record = record;
    }

    private string Identifier => Record.FileIdentifier;

    public IsoDirectory? Parent { get; }

    public string Extension
    {
        get
        {
            var value = NameRegex().Match(Identifier).Value;

            var extension = Path.GetExtension(value);

            return extension;
        }
    }

    public string FullName
    {
        get
        {
            var builder = new StringBuilder();

            var node = this;

            while (node != null)
            {
                builder.Insert(0, DirectorySeparator).Insert(1, node.Name);

                node = node.Parent;
            }

            if (this is IsoDirectory)
            {
                builder.Append(DirectorySeparator);
            }

            if (builder.Length > 1)
            {
                builder.Remove(0, 1);
            }

            var fullName = builder.ToString();

            return fullName;
        }
    }

    public string Name
    {
        get
        {
            var value = NameRegex().Match(Identifier).Value;

            var name = Path.GetFileName(value);

            return name;
        }
    }

    [GeneratedRegex("""^.+?(?=;|\r?$)""")]
    private static partial Regex NameRegex();

    public override string ToString()
    {
        return FullName;
    }
}

public sealed class IsoDirectory : IsoFileSystemEntry
{
    public IsoDirectory(IsoDirectory? parent, DirectoryRecord record)
        : base(parent, record)
    {
    }

    public List<IsoDirectory> Directories { get; } = new();

    public List<IsoFile> Files { get; } = new();
}

public sealed partial class IsoFile : IsoFileSystemEntry
{
    public IsoFile(IsoDirectory? parent, DirectoryRecord record)
        : base(parent, record)
    {
        Version = Convert.ToInt32(VersionRegex().Match(record.FileIdentifier).Value);
    }

    public int Version { get; }

    [GeneratedRegex("""(?<=;)\d+""")]
    private static partial Regex VersionRegex();
}