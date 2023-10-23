using System.Text.RegularExpressions;

namespace ISO9660.Tests.ISO9660;

public sealed partial class IsoFileSystemEntryFile : IsoFileSystemEntry
{
    public IsoFileSystemEntryFile(IsoFileSystemEntryDirectory? parent, DirectoryRecord record)
        : base(parent, record)
    {
        var regex = VersionRegex();

        var match = regex.Match(record.FileIdentifier);

        int version;

        if (match.Success && int.TryParse(match.Value, out var i))
        {
            version = i;
        }
        else // NetBSD shit, might be present but as padding field
        {
            version = 1;
        }

        Version = version;
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