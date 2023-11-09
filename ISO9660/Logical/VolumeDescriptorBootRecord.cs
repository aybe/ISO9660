using Whatever.Extensions;

namespace ISO9660.Logical;

public sealed class VolumeDescriptorBootRecord : VolumeDescriptor
{
    public VolumeDescriptorBootRecord(VolumeDescriptor descriptor, Stream stream)
        : base(descriptor)
    {
        BootSystemIdentifier = stream.ReadIsoString(32, IsoStringFlags.ACharacters);
        BootIdentifier       = stream.ReadIsoString(32, IsoStringFlags.ACharacters);
        BootSystemUse        = stream.ReadExactly(1977);
    }

    public string BootSystemIdentifier { get; }

    public string BootIdentifier { get; }

    public byte[] BootSystemUse { get; }
}