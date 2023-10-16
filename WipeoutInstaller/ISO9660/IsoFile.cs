using System.Text.RegularExpressions;

namespace WipeoutInstaller.ISO9660;

public sealed partial class IsoFile : IsoFileSystemEntry
{
    public IsoFile(IsoDirectory? parent, DirectoryRecord record)
        : base(parent, record)
    {
        Version = Convert.ToInt32(VersionRegex().Match(record.FileIdentifier).Value);
    }

    public int Length => Record.DataLength;

    public int Position => Record.LocationOfExtent;

    public int Version { get; }

    [GeneratedRegex("""(?<=;)\d+""")]
    private static partial Regex VersionRegex();

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(Length)}: {Length}, {nameof(Position)}: {Position}, {nameof(Version)}: {Version}";
    }
}