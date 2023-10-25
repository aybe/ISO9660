using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem.VolumeDescriptors;

public sealed class VolumePartitionDescriptor : VolumeDescriptor
{
    public VolumePartitionDescriptor(VolumeDescriptor descriptor, BinaryReader reader)
        : base(descriptor)
    {
        reader.Seek(1);

        SystemIdentifier = new IsoString(reader, 32, IsoStringFlags.ACharacters);

        VolumePartitionIdentifier = new IsoString(reader, 32, IsoStringFlags.DCharacters);

        VolumePartitionLocation = reader.ReadIso733();

        VolumePartitionSize = reader.ReadIso733();

        SystemUse = reader.ReadBytes(1960);
    }

    public IsoString SystemIdentifier { get; }

    public IsoString VolumePartitionIdentifier { get; }

    public uint VolumePartitionLocation { get; }

    public uint VolumePartitionSize { get; }

    public byte[] SystemUse { get; }
}