using System.Collections.ObjectModel;

namespace WipeoutInstaller.ISO9660;

public sealed class VolumeDescriptorSet : Collection<VolumeDescriptor>
{
    public PrimaryVolumeDescriptor PrimaryVolumeDescriptor => this.OfType<PrimaryVolumeDescriptor>().Single();
}