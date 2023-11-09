using Whatever.Extensions;

namespace ISO9660.Logical;

public sealed class PathTableRecord
{
    public PathTableRecord(Stream stream)
    {
        LengthOfDirectoryIdentifier = stream.ReadIso711();

        ExtendedAttributeRecordLength = stream.ReadIso711();

        LocationOfExtent = stream.ReadIso731();

        ParentDirectoryNumber = stream.ReadIso721();

        DirectoryIdentifier = stream.ReadIsoString(LengthOfDirectoryIdentifier,
            IsoStringFlags.DCharacters | IsoStringFlags.Byte00 | IsoStringFlags.Byte01);

        PaddingField = LengthOfDirectoryIdentifier % 2 is not 0
            ? stream.ReadByte().ToByte()
            : null;
    }

    public byte LengthOfDirectoryIdentifier { get; }

    public byte ExtendedAttributeRecordLength { get; }

    public uint LocationOfExtent { get; }

    public ushort ParentDirectoryNumber { get; }

    public string DirectoryIdentifier { get; }

    public byte? PaddingField { get; }

    public override string ToString()
    {
        return $"{nameof(LocationOfExtent)}: {LocationOfExtent}, " +
               $"{nameof(ParentDirectoryNumber)}: {ParentDirectoryNumber}, " +
               $"{nameof(DirectoryIdentifier)}: {DirectoryIdentifier}";
    }
}