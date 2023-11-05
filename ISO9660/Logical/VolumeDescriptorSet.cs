using System.Collections.ObjectModel;

namespace ISO9660.Logical;

public sealed class VolumeDescriptorSet : Collection<VolumeDescriptor>
{
    public VolumeDescriptorPrimary VolumeDescriptorPrimary => this.OfType<VolumeDescriptorPrimary>().Single();
}