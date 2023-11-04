namespace ISO9660.FileSystem;

public sealed class PathTableRecord
{
    public PathTableRecord(BinaryReader reader)
    {
        LengthOfDirectoryIdentifier = reader.ReadIso711();

        ExtendedAttributeRecordLength = reader.ReadIso711();

        LocationOfExtent = reader.ReadIso731();

        ParentDirectoryNumber = reader.ReadIso721();

        DirectoryIdentifier = reader.ReadIsoString(LengthOfDirectoryIdentifier,
            IsoStringFlags.DCharacters | IsoStringFlags.Byte00 | IsoStringFlags.Byte01);

        PaddingField = LengthOfDirectoryIdentifier % 2 is not 0
            ? reader.ReadByte()
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