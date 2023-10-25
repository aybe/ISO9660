using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem.VolumeDescriptors;

public class PrimaryVolumeDescriptor : VolumeDescriptor
{
    public PrimaryVolumeDescriptor(VolumeDescriptor descriptor, BinaryReader reader)
        : base(descriptor)
    {
        reader.Seek(1);

        SystemIdentifier = new IsoString(reader, 32, IsoStringFlags.ACharacters);

        VolumeIdentifier = new IsoString(reader, 32, IsoStringFlags.DCharacters);

        reader.Seek(8);

        VolumeSpaceSize = reader.ReadIso733();

        reader.Seek(32);

        VolumeSetSize = reader.ReadIso723();

        VolumeSequenceNumber = reader.ReadIso723();

        LogicalBlockSize = reader.ReadIso723();

        PathTableSize = reader.ReadIso733();

        LocationOfOccurrenceOfTypeLPathTable = reader.ReadIso731();

        LocationOfOptionalOccurrenceOfTypeLPathTable = reader.ReadIso731();

        LocationOfOccurrenceOfTypeMPathTable = reader.ReadIso732();

        LocationOfOptionalOccurrenceOfTypeMPathTable = reader.ReadIso732();

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

        FileStructureVersion = reader.ReadIso711();

        Reserved1 = reader.ReadByte();

        ApplicationUse = reader.ReadBytes(512);

        Reserved2 = reader.ReadBytes(653);
    }

    public IsoString SystemIdentifier { get; }

    public IsoString VolumeIdentifier { get; }

    public uint VolumeSpaceSize { get; }

    public ushort VolumeSetSize { get; }

    public ushort VolumeSequenceNumber { get; }

    public ushort LogicalBlockSize { get; }

    public uint PathTableSize { get; }

    public uint LocationOfOccurrenceOfTypeLPathTable { get; }

    public uint LocationOfOptionalOccurrenceOfTypeLPathTable { get; }

    public uint LocationOfOccurrenceOfTypeMPathTable { get; }

    public uint LocationOfOptionalOccurrenceOfTypeMPathTable { get; }

    public DirectoryRecord DirectoryRecordForRootDirectory { get; }

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

    public byte FileStructureVersion { get; }

    public byte Reserved1 { get; }

    public byte[] ApplicationUse { get; }

    public byte[] Reserved2 { get; }
}