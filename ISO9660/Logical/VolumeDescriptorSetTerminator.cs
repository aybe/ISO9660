using Whatever.Extensions;

namespace ISO9660.Logical;

public sealed class VolumeDescriptorSetTerminator(VolumeDescriptor descriptor, Stream stream)
    : VolumeDescriptor(descriptor)
{
    public byte[] Reserved { get; } = stream.ReadExactly(2041);
}