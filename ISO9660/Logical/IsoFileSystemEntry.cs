using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace ISO9660.Logical;

public abstract partial class IsoFileSystemEntry(IsoFileSystemEntryDirectory? parent, DirectoryRecord record)
{
    [PublicAPI]
    public const char DirectorySeparator = '/';

    protected readonly DirectoryRecord Record = record;

    [PublicAPI]
    public IsoFileSystemEntryDirectory? Parent { get; } = parent;

    [PublicAPI]
    public DateTimeOffset Modified => Record.RecordingDateAndTime.ToDateTimeOffset();

    [PublicAPI]
    public string FullName
    {
        get
        {
            var builder = new StringBuilder();

            var node = this;

            while (node != null)
            {
                builder.Insert(0, DirectorySeparator).Insert(1, node.FileName);

                node = node.Parent;
            }

            if (this is IsoFileSystemEntryDirectory)
            {
                builder.Append(DirectorySeparator);
            }

            if (builder.Length > 1)
            {
                builder.Remove(0, 2);
            }

            var fullName = builder.ToString();

            return fullName;
        }
    }

    [PublicAPI]
    public string FileName
    {
        get
        {
            var value = FileNameRegex().Match(Record.FileIdentifier).Value;

            var name = Path.GetFileName(value);

            return name;
        }
    }

    [GeneratedRegex("""^.+?(?=;|\r?$)""")]
    private static partial Regex FileNameRegex();

    public override string ToString()
    {
        return FullName;
    }
}