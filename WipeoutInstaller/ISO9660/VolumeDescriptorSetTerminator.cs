using System.Diagnostics.CodeAnalysis;

namespace WipeoutInstaller.ISO9660;

public sealed class VolumeDescriptorSetTerminator : VolumeDescriptor
{
    [SetsRequiredMembers]
    public VolumeDescriptorSetTerminator(VolumeDescriptor descriptor, BinaryReader reader)
        : base(descriptor)
    {
        Reserved = reader.ReadBytes(2041); // BUG? check size
    }

    public byte[] Reserved { get; }
}