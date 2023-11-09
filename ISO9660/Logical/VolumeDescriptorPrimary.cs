using Whatever.Extensions;

namespace ISO9660.Logical;

public sealed class VolumeDescriptorPrimary : VolumeDescriptor
{
    public VolumeDescriptorPrimary(VolumeDescriptor descriptor, Stream stream)
        : base(descriptor)
    {
        stream.Seek(1, SeekOrigin.Current);

        SystemIdentifier = stream.ReadIsoString(32, IsoStringFlags.ACharacters);

        VolumeIdentifier = stream.ReadIsoString(32, IsoStringFlags.DCharacters);

        stream.Seek(8, SeekOrigin.Current);

        VolumeSpaceSize = stream.ReadIso733();

        stream.Seek(32, SeekOrigin.Current);

        VolumeSetSize = stream.ReadIso723();

        VolumeSequenceNumber = stream.ReadIso723();

        LogicalBlockSize = stream.ReadIso723();

        PathTableSize = stream.ReadIso733();

        LocationOfOccurrenceOfTypeLPathTable = stream.ReadIso731();

        LocationOfOptionalOccurrenceOfTypeLPathTable = stream.ReadIso731();

        LocationOfOccurrenceOfTypeMPathTable = stream.ReadIso732();

        LocationOfOptionalOccurrenceOfTypeMPathTable = stream.ReadIso732();

        DirectoryRecordForRootDirectory = new DirectoryRecord(stream);

        VolumeSetIdentifier = stream.ReadIsoString(128, IsoStringFlags.DCharacters);

        PublisherIdentifier = stream.ReadIsoString(128, IsoStringFlags.ACharacters);

        DataPreparerIdentifier = stream.ReadIsoString(128, IsoStringFlags.ACharacters);

        ApplicationIdentifier = stream.ReadIsoString(128, IsoStringFlags.ACharacters);

        CopyrightFileIdentifier = stream.ReadIsoString(37, IsoStringFlags.DCharacters | IsoStringFlags.Separator1 | IsoStringFlags.Separator2);

        AbstractFileIdentifier = stream.ReadIsoString(37, IsoStringFlags.DCharacters | IsoStringFlags.Separator1 | IsoStringFlags.Separator2);

        BibliographicFileIdentifier = stream.ReadIsoString(37, IsoStringFlags.DCharacters | IsoStringFlags.Separator1 | IsoStringFlags.Separator2);

        VolumeCreationDateAndTime = new VolumeDescriptorDateTime(stream);

        VolumeModificationDateAndTime = new VolumeDescriptorDateTime(stream);

        VolumeExpirationDateAndTime = new VolumeDescriptorDateTime(stream);

        VolumeEffectiveDateAndTime = new VolumeDescriptorDateTime(stream);

        FileStructureVersion = stream.ReadIso711();

        Reserved1 = stream.ReadByte().ToByte();

        ApplicationUse = stream.ReadExactly(512);

        Reserved2 = stream.ReadExactly(653);
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