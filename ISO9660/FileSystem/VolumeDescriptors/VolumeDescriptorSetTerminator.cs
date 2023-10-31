namespace ISO9660.FileSystem.VolumeDescriptors;

public sealed class VolumeDescriptorSetTerminator : VolumeDescriptor
{
    public VolumeDescriptorSetTerminator(VolumeDescriptor descriptor, BinaryReader reader)
        : base(descriptor)
    {
        Reserved = reader.ReadBytes(2041);
    }

    public byte[] Reserved { get; }
}