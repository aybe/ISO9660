using System.Diagnostics;
using WipeoutInstaller.Iso9660;

namespace WipeoutInstaller;

[DebuggerDisplay(
    "Parent = {ParentDirectoryNumber}, Extent = {LocationOfExtent}, Name = {DirectoryIdentifier}")]
public sealed class PathTableRecord
{
    public PathTableRecord(BinaryReader reader)
    {
        LengthOfDirectoryIdentifier = new Iso711(reader);

        ExtendedAttributeRecordLength = new Iso711(reader);

        LocationOfExtent = new Iso731(reader);

        ParentDirectoryNumber = new Iso721(reader);

        DirectoryIdentifier = new IsoStringD(reader, LengthOfDirectoryIdentifier);

        PaddingField = LengthOfDirectoryIdentifier % 2 is not 0 // TODO add method but invert
            ? reader.ReadByte()
            : null;
    }

    public Iso711 LengthOfDirectoryIdentifier { get; }

    public Iso711 ExtendedAttributeRecordLength { get; }

    public Iso731 LocationOfExtent { get; }

    public Iso721 ParentDirectoryNumber { get; }

    public IsoStringD DirectoryIdentifier { get; }

    public byte? PaddingField { get; }

    public override string ToString()
    {
        return DirectoryIdentifier.Value;
    }
}