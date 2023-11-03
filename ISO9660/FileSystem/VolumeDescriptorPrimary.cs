using ISO9660.Extensions;

namespace ISO9660.FileSystem;

public sealed class VolumeDescriptorPrimary : VolumeDescriptor
{
    public VolumeDescriptorPrimary(VolumeDescriptor descriptor, BinaryReader reader)
        : base(descriptor)
    {
        reader.Seek(1);

        SystemIdentifier = reader.ReadIsoString(32, IsoStringFlags.ACharacters);

        VolumeIdentifier = reader.ReadIsoString(32, IsoStringFlags.DCharacters);

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

        VolumeSetIdentifier = reader.ReadIsoString(128, IsoStringFlags.DCharacters);

        PublisherIdentifier = reader.ReadIsoString(128, IsoStringFlags.ACharacters);

        DataPreparerIdentifier = reader.ReadIsoString(128, IsoStringFlags.ACharacters);

        ApplicationIdentifier = reader.ReadIsoString(128, IsoStringFlags.ACharacters);

        CopyrightFileIdentifier = reader.ReadIsoString(37, IsoStringFlags.DCharacters | IsoStringFlags.Separator1 | IsoStringFlags.Separator2);

        AbstractFileIdentifier = reader.ReadIsoString(37, IsoStringFlags.DCharacters | IsoStringFlags.Separator1 | IsoStringFlags.Separator2);

        BibliographicFileIdentifier = reader.ReadIsoString(37, IsoStringFlags.DCharacters | IsoStringFlags.Separator1 | IsoStringFlags.Separator2);

        VolumeCreationDateAndTime = new VolumeDescriptorDateTime(reader);

        VolumeModificationDateAndTime = new VolumeDescriptorDateTime(reader);

        VolumeExpirationDateAndTime = new VolumeDescriptorDateTime(reader);

        VolumeEffectiveDateAndTime = new VolumeDescriptorDateTime(reader);

        FileStructureVersion = reader.ReadIso711();

        Reserved1 = reader.ReadByte();

        ApplicationUse = reader.ReadBytes(512);

        Reserved2 = reader.ReadBytes(653);
    }

    public string SystemIdentifier { get; }

    public string VolumeIdentifier { get; }

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

    public string VolumeSetIdentifier { get; }

    public string PublisherIdentifier { get; }

    public string DataPreparerIdentifier { get; }

    public string ApplicationIdentifier { get; }

    public string CopyrightFileIdentifier { get; }

    public string AbstractFileIdentifier { get; }

    public string BibliographicFileIdentifier { get; }

    public VolumeDescriptorDateTime VolumeCreationDateAndTime { get; }

    public VolumeDescriptorDateTime VolumeModificationDateAndTime { get; }

    public VolumeDescriptorDateTime VolumeExpirationDateAndTime { get; }

    public VolumeDescriptorDateTime VolumeEffectiveDateAndTime { get; }

    public byte FileStructureVersion { get; }

    public byte Reserved1 { get; }

    public byte[] ApplicationUse { get; }

    public byte[] Reserved2 { get; }
}