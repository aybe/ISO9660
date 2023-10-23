using System.Diagnostics.CodeAnalysis;

namespace ISO9660.Tests.FileSystem;

public class VolumeDescriptor
{
    internal VolumeDescriptor()
    {
    }

    [SetsRequiredMembers]
    protected VolumeDescriptor(VolumeDescriptor descriptor)
    {
        VolumeDescriptorType    = descriptor.VolumeDescriptorType;
        StandardIdentifier      = descriptor.StandardIdentifier;
        VolumeDescriptorVersion = descriptor.VolumeDescriptorVersion;
    }

    public required VolumeDescriptorType VolumeDescriptorType { get; init; }

    public required string StandardIdentifier { get; init; }

    public required Iso711 VolumeDescriptorVersion { get; init; }
}