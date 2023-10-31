using System.Collections.ObjectModel;

namespace ISO9660.FileSystem.VolumeDescriptors;

public sealed class VolumeDescriptorSet : Collection<VolumeDescriptor>
{
    public PrimaryVolumeDescriptor PrimaryVolumeDescriptor => this.OfType<PrimaryVolumeDescriptor>().Single();
}