using System;
using System.IO;

namespace CDROMTools.Iso9660
{
      public sealed class VolumeDescriptorSetTerminator : VolumeDescriptor
    {
        public readonly byte[] Reserved;

        internal VolumeDescriptorSetTerminator(VolumeDescriptor descriptor, BinaryReader reader) : base(descriptor)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            Reserved = reader.ReadBytes(2041);
            }
    }
}