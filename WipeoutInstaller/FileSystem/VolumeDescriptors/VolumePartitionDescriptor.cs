﻿using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem.VolumeDescriptors;

public sealed class VolumePartitionDescriptor : VolumeDescriptor
{
    public VolumePartitionDescriptor(VolumeDescriptor descriptor, BinaryReader reader)
        : base(descriptor)
    {
        reader.Seek(1);

        SystemIdentifier = reader.ReadIsoString(32, IsoStringFlags.ACharacters);

        VolumePartitionIdentifier = reader.ReadIsoString(32, IsoStringFlags.DCharacters);

        VolumePartitionLocation = reader.ReadIso733();

        VolumePartitionSize = reader.ReadIso733();

        SystemUse = reader.ReadBytes(1960);
    }

    public string SystemIdentifier { get; }

    public string VolumePartitionIdentifier { get; }

    public uint VolumePartitionLocation { get; }

    public uint VolumePartitionSize { get; }

    public byte[] SystemUse { get; }
}