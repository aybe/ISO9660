namespace ISO9660.Tests.ISO9660;

public sealed class IsoFileSystemEntryDirectory : IsoFileSystemEntry
{
    public IsoFileSystemEntryDirectory(IsoFileSystemEntryDirectory? parent, DirectoryRecord record)
        : base(parent, record)
    {
    }

    public List<IsoFileSystemEntryDirectory> Directories { get; } = new();

    public List<IsoFileSystemEntryFile> Files { get; } = new();
}