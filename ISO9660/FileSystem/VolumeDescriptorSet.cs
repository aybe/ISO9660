using System.Collections.ObjectModel;

namespace ISO9660.FileSystem;

public sealed class VolumeDescriptorSet : Collection<VolumeDescriptor>
{
    public VolumeDescriptorPrimary VolumeDescriptorPrimary => this.OfType<VolumeDescriptorPrimary>().Single();
}