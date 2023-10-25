using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem.VolumeDescriptors;

public class VolumeDescriptor
{
    internal VolumeDescriptor(BinaryReader reader)
    {
        VolumeDescriptorType    = reader.Read<VolumeDescriptorType>(); // 711
        StandardIdentifier      = reader.ReadStringAscii(5);
        VolumeDescriptorVersion = reader.ReadIso711();
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