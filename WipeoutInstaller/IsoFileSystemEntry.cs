using WipeoutInstaller.Iso9660;

namespace WipeoutInstaller;

public abstract class IsoFileSystemEntry
{
    public string Name { get; set; } = null!;

    public IsoDirectory? Parent { get; set; }

    public static IsoDirectory Build(Dictionary<PathTableRecord, List<DirectoryRecord>> dictionary, List<PathTableRecord> pathTableRecords)
    {
        var pathTableRecord1 = pathTableRecords.First();

        Console.WriteLine(JsonUtility.ToJson(pathTableRecord1));

        var first = dictionary.First();
        var directory = new IsoDirectory();

        var stack = new Stack<PathTableRecord>();

        stack.Push(dictionary.First().Key);
        foreach (var record in first.Value)
        {
            if (record.FileFlags is not FileFlags.Directory)
            {
                directory.Files.Add(new IsoFile { Name = record.ToString() });
            }
        }

        return directory;
    }
}

public sealed class IsoDirectory : IsoFileSystemEntry
{
    public List<IsoDirectory> Directories { get; } = new();

    public List<IsoFile> Files { get; } = new();
}

public sealed class IsoFile : IsoFileSystemEntry
{
}