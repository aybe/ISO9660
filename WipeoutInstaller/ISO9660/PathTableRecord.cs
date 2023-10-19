namespace WipeoutInstaller.ISO9660;

public sealed class PathTableRecord
{
    public const int MinimumLength = 8;

    public PathTableRecord(BinaryReader reader)
    {
        LengthOfDirectoryIdentifier = new Iso711(reader);

        ExtendedAttributeRecordLength = new Iso711(reader);

        LocationOfExtent = new Iso731(reader);

        ParentDirectoryNumber = new Iso721(reader);

        DirectoryIdentifier = new IsoString(reader, LengthOfDirectoryIdentifier,
            IsoStringFlags.DCharacters | IsoStringFlags.Byte00 | IsoStringFlags.Byte01);

        PaddingField = LengthOfDirectoryIdentifier % 2 is not 0 // TODO add method but invert
            ? reader.ReadByte()
            : null;
    }

    public Iso711 LengthOfDirectoryIdentifier { get; }

    public Iso711 ExtendedAttributeRecordLength { get; }

    public Iso731 LocationOfExtent { get; }

    public Iso721 ParentDirectoryNumber { get; }

    public IsoString DirectoryIdentifier { get; }

    public byte? PaddingField { get; }

    public override string ToString()
    {
        return $"{nameof(LocationOfExtent)}: {LocationOfExtent}, " +
               $"{nameof(ParentDirectoryNumber)}: {ParentDirectoryNumber}, " +
               $"{nameof(DirectoryIdentifier)}: {DirectoryIdentifier}";
    }
}