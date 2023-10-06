namespace WipeoutInstaller.Iso9660;

public sealed class VolumeDescriptorSetTerminator : VolumeDescriptor
{
    public VolumeDescriptorSetTerminator(VolumeDescriptor descriptor, BinaryReader reader)
        : base(descriptor)
    {
        Reserved = reader.ReadBytes(2041);
    }

    public byte[] Reserved { get; }
}