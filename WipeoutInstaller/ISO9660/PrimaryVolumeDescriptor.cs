using System.Diagnostics.CodeAnalysis;
using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.ISO9660;

public class PrimaryVolumeDescriptor : VolumeDescriptor
{
    [SetsRequiredMembers]
    public PrimaryVolumeDescriptor(VolumeDescriptor descriptor, BinaryReader reader)
        : base(descriptor)
    {
        reader.Seek(1);

        SystemIdentifier = new IsoString(reader, 32, IsoStringFlags.ACharacters);

        VolumeIdentifier = new IsoString(reader, 32, IsoStringFlags.DCharacters);

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

        VolumeSetIdentifier = new IsoString(reader, 128, IsoStringFlags.DCharacters);

        PublisherIdentifier = new IsoString(reader, 128, IsoStringFlags.ACharacters);

        DataPreparerIdentifier = new IsoString(reader, 128, IsoStringFlags.ACharacters);

        ApplicationIdentifier = new IsoString(reader, 128, IsoStringFlags.ACharacters);

        CopyrightFileIdentifier = new IsoString(reader, 37, IsoStringFlags.DCharacters | IsoStringFlags.Separator1 | IsoStringFlags.Separator2);

        AbstractFileIdentifier = new IsoString(reader, 37, IsoStringFlags.DCharacters | IsoStringFlags.Separator1 | IsoStringFlags.Separator2);

        BibliographicFileIdentifier = new IsoString(reader, 37, IsoStringFlags.DCharacters | IsoStringFlags.Separator1 | IsoStringFlags.Separator2);

        VolumeCreationDateAndTime = new DateAndTimeFormat(reader);

        VolumeModificationDateAndTime = new DateAndTimeFormat(reader);

        VolumeExpirationDateAndTime = new DateAndTimeFormat(reader);

        VolumeEffectiveDateAndTime = new DateAndTimeFormat(reader);

        FileStructureVersion = new Iso711(reader);

        Reserved1 = reader.ReadByte();

        ApplicationUse = reader.ReadBytes(512);

        Reserved2 = reader.ReadByte();
    }

    public IsoString SystemIdentifier { get; }

    public IsoString VolumeIdentifier { get; }

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

    public IsoString VolumeSetIdentifier { get; }

    public IsoString PublisherIdentifier { get; }

    public IsoString DataPreparerIdentifier { get; }

    public IsoString ApplicationIdentifier { get; }

    public IsoString CopyrightFileIdentifier { get; }

    public IsoString AbstractFileIdentifier { get; }

    public IsoString BibliographicFileIdentifier { get; }

    public DateAndTimeFormat VolumeCreationDateAndTime { get; }

    public DateAndTimeFormat VolumeModificationDateAndTime { get; }

    public DateAndTimeFormat VolumeExpirationDateAndTime { get; }

    public DateAndTimeFormat VolumeEffectiveDateAndTime { get; }

    public Iso711 FileStructureVersion { get; }

    public byte Reserved1 { get; set; }

    public byte[] ApplicationUse { get; }

    public byte Reserved2 { get; set; }
}