using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.Iso9660;

public class PrimaryVolumeDescriptor : VolumeDescriptor
{
    public PrimaryVolumeDescriptor(VolumeDescriptor descriptor, BinaryReader reader)
        : base(descriptor)
    {
        reader.Seek(1);

        SystemIdentifier = new IsoStringA(reader, 32);

        VolumeIdentifier = new IsoStringD(reader, 32);

        reader.Seek(8);

        VolumeSpaceSize = new Iso733(reader);

        reader.Seek(32);

        VolumeSetSize = new Iso723(reader);

        VolumeSequenceNumber = new Iso723(reader);

        LogicalBlockSize = new Iso723(reader);

        PathTableSize = new Iso733(reader);

        LocationOfOccurrenceOfTypeLPathTable = new Iso731(reader);

        LocationOfOptionalOccurrenceOfTypeLPathTable = new Iso731(reader);

        LocationOfOccurrenceOfTypeMPathTable = new Iso732(reader);

        LocationOfOptionalOccurrenceOfTypeMPathTable = new Iso732(reader);

        DirectoryRecordForRootDirectory = new DirectoryRecord(reader);

        VolumeSetIdentifier = new IsoStringD(reader, 128);

        PublisherIdentifier = new IsoStringA(reader, 128);

        DataPreparerIdentifier = new IsoStringA(reader, 128);

        ApplicationIdentifier = new IsoStringA(reader, 128);

        CopyrightFileIdentifier = new IsoStringD(reader, 37); // TODO d-characters, SEPARATOR 1, SEPARATOR 2 7.5

        AbstractFileIdentifier = new IsoStringD(reader, 37); // TODO d-characters, SEPARATOR 1, SEPARATOR 2 7.5

        BibliographicFileIdentifier = new IsoStringD(reader, 37); // TODO d-characters, SEPARATOR 1, SEPARATOR 2 7.5

        VolumeCreationDateAndTime = new DateAndTimeFormat(reader);

        VolumeModificationDateAndTime = new DateAndTimeFormat(reader);

        VolumeExpirationDateAndTime = new DateAndTimeFormat(reader);

        VolumeEffectiveDateAndTime = new DateAndTimeFormat(reader);

        FileStructureVersion = new Iso711(reader);

        Reserved1 = reader.ReadByte();

        ApplicationUse = reader.ReadBytes(512);

        Reserved2 = reader.ReadByte();
    }

    public IsoStringA SystemIdentifier { get; }

    public IsoStringD VolumeIdentifier { get; }

    public Iso733 VolumeSpaceSize { get; set; }

    public Iso723 VolumeSetSize { get; }

    public Iso723 VolumeSequenceNumber { get; }

    public Iso723 LogicalBlockSize { get; }

    public Iso733 PathTableSize { get; }

    public Iso731 LocationOfOccurrenceOfTypeLPathTable { get; }

    public Iso731 LocationOfOptionalOccurrenceOfTypeLPathTable { get; }

    public Iso732 LocationOfOccurrenceOfTypeMPathTable { get; }

    public Iso732 LocationOfOptionalOccurrenceOfTypeMPathTable { get; }

    public DirectoryRecord DirectoryRecordForRootDirectory { get; set; }

    public IsoStringD VolumeSetIdentifier { get; }

    public IsoStringA PublisherIdentifier { get; }

    public IsoStringA DataPreparerIdentifier { get; }

    public IsoStringA ApplicationIdentifier { get; }

    public IsoStringD CopyrightFileIdentifier { get; }

    public IsoStringD AbstractFileIdentifier { get; }

    public IsoStringD BibliographicFileIdentifier { get; }

    public DateAndTimeFormat VolumeCreationDateAndTime { get; }

    public DateAndTimeFormat VolumeModificationDateAndTime { get; }

    public DateAndTimeFormat VolumeExpirationDateAndTime { get; }

    public DateAndTimeFormat VolumeEffectiveDateAndTime { get; }

    public Iso711 FileStructureVersion { get; }

    public byte Reserved1 { get; set; }

    public byte[] ApplicationUse { get; }

    public byte Reserved2 { get; set; }
}