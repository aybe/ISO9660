using System.Text.RegularExpressions;

namespace WipeoutInstaller.ISO9660;

public sealed partial class IsoFileSystemEntryFile : IsoFileSystemEntry
{
    public IsoFileSystemEntryFile(IsoFileSystemEntryDirectory? parent, DirectoryRecord record)
        : base(parent, record)
    {
        var regex = VersionRegex();

        var match = regex.Match(record.FileIdentifier);

        if (match.Success) // BUG who is wrong, NetBSD or ECMA-130?
        {
            Version = Convert.ToInt32(match.Value);
        }
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