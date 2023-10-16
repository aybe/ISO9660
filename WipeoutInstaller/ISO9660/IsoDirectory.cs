namespace WipeoutInstaller.ISO9660;

public sealed class IsoDirectory : IsoFileSystemEntry
{
    public IsoDirectory(IsoDirectory? parent, DirectoryRecord record)
        : base(parent, record)
    {
    }

    public List<IsoDirectory> Directories { get; } = new();

    public List<IsoFile> Files { get; } = new();
}