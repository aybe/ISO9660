using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace ISO9660.Tests.FileSystem;

public abstract partial class IsoFileSystemEntry
{
    [PublicAPI]
    public const char DirectorySeparator = '/';

    protected readonly DirectoryRecord Record;

    protected IsoFileSystemEntry(IsoFileSystemEntryDirectory? parent, DirectoryRecord record)
    {
        Parent = parent;
        Record = record;
    }

    private string Identifier => Record.FileIdentifier;

    public IsoFileSystemEntryDirectory? Parent { get; }

    public DateTimeOffset Modified => Record.RecordingDateAndTime.ToDateTimeOffset();

    public string Extension
    {
        get
        {
            var value = NameRegex().Match(Identifier).Value;

            var extension = Path.GetExtension(value);

            return extension;
        }
    }

    public string FullName
    {
        get
        {
            var builder = new StringBuilder();

            var node = this;

            while (node != null)
            {
                builder.Insert(0, DirectorySeparator).Insert(1, node.Name);

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

    public string Name
    {
        get
        {
            var value = NameRegex().Match(Identifier).Value;

            var name = Path.GetFileName(value);

            return name;
        }
    }

    [GeneratedRegex("""^.+?(?=;|\r?$)""")]
    private static partial Regex NameRegex();

    public override string ToString()
    {
        return FullName;
    }
}