using System;
using System.IO;
using CDROMTools.Utils;

namespace CDROMTools.Iso9660
{
     public sealed class VolumePartitionDescriptor : VolumeDescriptor
    {
        public readonly byte Unused;
        public readonly string SystemIdentifier;
        public readonly string VolumePartitionIdentifer;
        public readonly Iso733 VolumePartitionLocation;
        public readonly Iso733 VolumePartitionSize;
        public readonly byte[] SystemUse;

        internal VolumePartitionDescriptor(VolumeDescriptor descriptor, BinaryReader reader) : base(descriptor)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            Unused = reader.ReadByte();
            SystemIdentifier = reader.ReadStringAscii(32);
            VolumePartitionIdentifer = reader.ReadStringAscii(32);
            VolumePartitionLocation = new Iso733(reader);
            VolumePartitionSize = new Iso733(reader);
            SystemUse = reader.ReadBytes(1960);
        }
    }
}