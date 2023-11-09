using Whatever.Extensions;

namespace ISO9660.Logical;

public class VolumeDescriptor
{
    internal VolumeDescriptor(Stream stream)
    {
        VolumeDescriptorType    = stream.Read<VolumeDescriptorType>(); // 711
        StandardIdentifier      = stream.ReadStringAscii(5);
        VolumeDescriptorVersion = stream.ReadIso711();
    }

    protected VolumeDescriptor(VolumeDescriptor descriptor)
    {
        VolumeDescriptorType    = descriptor.VolumeDescriptorType;
        StandardIdentifier      = descriptor.StandardIdentifier;
        VolumeDescriptorVersion = descriptor.VolumeDescriptorVersion;
    }

    public VolumeDescriptorType VolumeDescriptorType { get; }

    public string StandardIdentifier { get; }

    public byte VolumeDescriptorVersion { get; }
}