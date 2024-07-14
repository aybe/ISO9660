namespace ISO9660.Logical;

public sealed class IsoFileSystemEntryDirectory(IsoFileSystemEntryDirectory? parent, DirectoryRecord record)
    : IsoFileSystemEntry(parent, record)
{
    public IList<IsoFileSystemEntryDirectory> Directories { get; } = new List<IsoFileSystemEntryDirectory>();

    public IList<IsoFileSystemEntryFile> Files { get; } = new List<IsoFileSystemEntryFile>();
}