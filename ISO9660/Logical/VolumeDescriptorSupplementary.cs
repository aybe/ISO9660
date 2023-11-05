using Whatever.Extensions;

namespace ISO9660.Logical;

public sealed class VolumeDescriptorSupplementary : VolumeDescriptor
{
    public VolumeDescriptorSupplementary(VolumeDescriptor descriptor, BinaryReader reader)
        : base(descriptor)
    {
        VolumeFlags = reader.ReadByte();

        SystemIdentifier = reader.ReadIsoString(32, IsoStringFlags.ACharacters); // TODO A1

        VolumeIdentifier = reader.ReadIsoString(32, IsoStringFlags.DCharacters); // TODO D1

        reader.Seek(8);

        VolumeSpaceSize = reader.ReadIso733();

        EscapeSequences = reader.ReadBytes(32); // TODO escape sequences

        VolumeSetSize = reader.ReadIso723();

        VolumeSequenceNumber = reader.ReadIso723();

        LogicalBlockSize = reader.ReadIso723();

        PathTableSize = reader.ReadIso733();

        LocationOfOccurrenceOfTypeLPathTable = reader.ReadIso731();

        LocationOfOptionalOccurrenceOfTypeLPathTable = reader.ReadIso731();

        LocationOfOccurrenceOfTypeMPathTable = reader.ReadIso732();

        LocationOfOptionalOccurrenceOfTypeMPathTable = reader.ReadIso732();

        DirectoryRecordForRootDirectory = new DirectoryRecord(reader);

        VolumeSetIdentifier = reader.ReadIsoString(128, IsoStringFlags.DCharacters); // TODO D1

        PublisherIdentifier = reader.ReadIsoString(128, IsoStringFlags.ACharacters); // TODO A1

        DataPreparerIdentifier = reader.ReadIsoString(128, IsoStringFlags.ACharacters); // TODO A1

        ApplicationIdentifier = reader.ReadIsoString(128, IsoStringFlags.ACharacters); // TODO A1

        CopyrightFileIdentifier = reader.ReadIsoString(37, IsoStringFlags.DCharacters | IsoStringFlags.Separator1 | IsoStringFlags.Separator2); // TODO D1

        AbstractFileIdentifier = reader.ReadIsoString(37, IsoStringFlags.DCharacters | IsoStringFlags.Separator1 | IsoStringFlags.Separator2); // TODO D1

        BibliographicFileIdentifier = reader.ReadIsoString(37, IsoStringFlags.DCharacters | IsoStringFlags.Separator1 | IsoStringFlags.Separator2); // TODO D1

        VolumeCreationDateAndTime = new VolumeDescriptorDateTime(reader);

        VolumeModificationDateAndTime = new VolumeDescriptorDateTime(reader);

        VolumeExpirationDateAndTime = new VolumeDescriptorDateTime(reader);

        VolumeEffectiveDateAndTime = new VolumeDescriptorDateTime(reader);

        FileStructureVersion = reader.ReadIso711();

        Reserved1 = reader.ReadByte();

        ApplicationUse = reader.ReadBytes(512);

        Reserved2 = reader.ReadBytes(653);
    }

    public byte VolumeFlags { get; }

    public string SystemIdentifier { get; }

    public string VolumeIdentifier { get; }

    public uint VolumeSpaceSize { get; }

    public byte[] EscapeSequences { get; }

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