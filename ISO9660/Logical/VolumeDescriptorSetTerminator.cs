using Whatever.Extensions;

namespace ISO9660.Logical;

public sealed class VolumeDescriptorSetTerminator : VolumeDescriptor
{
    public VolumeDescriptorSetTerminator(VolumeDescriptor descriptor, Stream stream)
        : base(descriptor)
    {
        Reserved = stream.ReadExactly(2041);
    }

    public byte[] Reserved { get; }
}