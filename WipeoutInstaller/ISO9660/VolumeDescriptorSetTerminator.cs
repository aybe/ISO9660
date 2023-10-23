using System.Diagnostics.CodeAnalysis;

namespace ISO9660.Tests.ISO9660;

public sealed class VolumeDescriptorSetTerminator : VolumeDescriptor
{
    [SetsRequiredMembers]
    public VolumeDescriptorSetTerminator(VolumeDescriptor descriptor, BinaryReader reader)
        : base(descriptor)
    {
        Reserved = reader.ReadBytes(2041);
    }

    public byte[] Reserved { get; }
}