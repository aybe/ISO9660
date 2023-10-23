using System.Collections.ObjectModel;

namespace ISO9660.Tests.FileSystem.VolumeDescriptors;

public sealed class VolumeDescriptorSet : Collection<VolumeDescriptor>
{
    public PrimaryVolumeDescriptor PrimaryVolumeDescriptor => this.OfType<PrimaryVolumeDescriptor>().Single();
}