using System.Diagnostics.CodeAnalysis;
using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.ISO9660;

public sealed class VolumePartitionDescriptor : VolumeDescriptor
{
    [SetsRequiredMembers]
    public VolumePartitionDescriptor(VolumeDescriptor descriptor, BinaryReader reader)
        : base(descriptor)
    {
        reader.Seek(1);

        SystemIdentifier = new IsoString(reader, 32, IsoStringFlags.ACharacters);

        VolumePartitionIdentifier = new IsoString(reader, 32, IsoStringFlags.DCharacters);

        VolumePartitionLocation = new Iso733(reader);

        VolumePartitionSize = new Iso733(reader);

        SystemUse = reader.ReadBytes(1960);
    }

    public IsoString SystemIdentifier { get; }

    public IsoString VolumePartitionIdentifier { get; }

    public Iso733 VolumePartitionLocation { get; }

    public Iso733 VolumePartitionSize { get; }

    public byte[] SystemUse { get; }
}