using System.Collections.ObjectModel;

namespace ISO9660.Logical;

public sealed class VolumeDescriptorSet : Collection<VolumeDescriptor>
{
    public VolumeDescriptorPrimary PrimaryVolumeDescriptor => this.OfType<VolumeDescriptorPrimary>().Single();
}