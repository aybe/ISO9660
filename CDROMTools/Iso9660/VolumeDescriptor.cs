using System;
using System.IO;
using CDROMTools.Utils;

namespace CDROMTools.Iso9660
{
    public class VolumeDescriptor
    {
        public readonly string StandardIdentifier;
        public readonly VolumeDescriptorType VolumeDescriptorType;
        public readonly Iso711 VolumeDescriptorVersion;

        private VolumeDescriptor(VolumeDescriptorType type, string identifier, Iso711 version)
        {
            VolumeDescriptorType = type;
            StandardIdentifier = identifier;
            VolumeDescriptorVersion = version;
        }

        protected VolumeDescriptor(VolumeDescriptor descriptor)
            : this(
                descriptor.VolumeDescriptorType,
                descriptor.StandardIdentifier,
                descriptor.VolumeDescriptorVersion
                )
        {
        }

        public static VolumeDescriptor TryCreate(BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            var typeI = new Iso711(reader);
            var typeB = (byte)typeI;
            if (!Enum.IsDefined(typeof (VolumeDescriptorType), typeB)) return null;
            var type = (VolumeDescriptorType)typeB;

            var identifier = reader.ReadStringAscii(5);
            if (identifier != "CD001") return null;

            var version = new Iso711(reader);
            var descriptor = new VolumeDescriptor(type, identifier, version);
            switch (type)
            {
                case VolumeDescriptorType.BootRecord:
                    return new BootRecord(descriptor, reader);
                case VolumeDescriptorType.PrimaryVolumeDescriptor:
                    return new PrimaryVolumeDescriptor(descriptor, reader);
                case VolumeDescriptorType.SupplementaryVolumeDescriptor:
                    return new SupplementaryVolumeDescriptor(descriptor, reader);
                case VolumeDescriptorType.VolumePartitionDescriptor:
                    return new VolumePartitionDescriptor(descriptor, reader);
                case VolumeDescriptorType.VolumeDescriptorSetTerminator:
                    return new VolumeDescriptorSetTerminator(descriptor, reader);
                default:
                    return null;
            }
        }
    }
}