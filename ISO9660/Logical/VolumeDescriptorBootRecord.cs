using Whatever.Extensions;

namespace ISO9660.Logical;

public sealed class VolumeDescriptorBootRecord(VolumeDescriptor descriptor, Stream stream)
    : VolumeDescriptor(descriptor)
{
    public string BootSystemIdentifier { get; } = stream.ReadIsoString(32, IsoStringFlags.ACharacters);

    public string BootIdentifier { get; } = stream.ReadIsoString(32, IsoStringFlags.ACharacters);

    public byte[] BootSystemUse { get; } = stream.ReadExactly(1977);
}