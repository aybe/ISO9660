namespace ISO9660.Tests.FileSystem.VolumeDescriptors;

public enum VolumeDescriptorType : byte
{
    BootRecord = 0,
    PrimaryVolumeDescriptor = 1,
    SupplementaryVolumeDescriptor = 2,
    VolumePartitionDescriptor = 3,
    VolumeDescriptorSetTerminator = 255
}