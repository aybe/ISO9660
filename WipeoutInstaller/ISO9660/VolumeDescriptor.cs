namespace WipeoutInstaller.Iso9660;

public class VolumeDescriptor
{
    public VolumeDescriptor()
    {
    }

    protected VolumeDescriptor(VolumeDescriptor descriptor)
    {
        VolumeDescriptorType    = descriptor.VolumeDescriptorType;
        StandardIdentifier      = descriptor.StandardIdentifier;
        VolumeDescriptorVersion = descriptor.VolumeDescriptorVersion;
    }

    public VolumeDescriptorType VolumeDescriptorType { get; init; }

    public string StandardIdentifier { get; init; }

    public Iso711 VolumeDescriptorVersion { get; init; }
}