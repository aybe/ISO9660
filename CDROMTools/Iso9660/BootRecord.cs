using System;
using System.IO;
using CDROMTools.Utils;

namespace CDROMTools.Iso9660
{
    public sealed class BootRecord : VolumeDescriptor
    {
        public readonly string BootIdentifier;
        public readonly string BootSystemIdentifier;
        public readonly byte[] BootSystemUse;

        internal BootRecord(VolumeDescriptor volumeDescriptor, BinaryReader reader) : base(volumeDescriptor)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            BootSystemIdentifier = reader.ReadStringAscii(32);
            BootIdentifier = reader.ReadStringAscii(32);
            BootSystemUse = reader.ReadBytes(1977);
        }
    }
}