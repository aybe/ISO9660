namespace ISO9660.Logical;

public sealed class IsoFileSystemEntryDirectory : IsoFileSystemEntry
{
    public IsoFileSystemEntryDirectory(IsoFileSystemEntryDirectory? parent, DirectoryRecord record)
        : base(parent, record)
    {
    }

    public IList<IsoFileSystemEntryDirectory> Directories { get; } = new List<IsoFileSystemEntryDirectory>();

    public IList<IsoFileSystemEntryFile> Files { get; } = new List<IsoFileSystemEntryFile>();
}