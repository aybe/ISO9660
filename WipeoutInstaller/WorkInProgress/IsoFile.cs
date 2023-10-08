using System.Text.RegularExpressions;
using WipeoutInstaller.Iso9660;

namespace WipeoutInstaller.WorkInProgress;

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