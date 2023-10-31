using ISO9660.Extensions;

namespace ISO9660.FileSystem.VolumeDescriptors;

public sealed class BootRecord : VolumeDescriptor
{
    public BootRecord(VolumeDescriptor descriptor, BinaryReader reader)
        : base(descriptor)
    {
        BootSystemIdentifier = reader.ReadIsoString(32, IsoStringFlags.ACharacters);
        BootIdentifier       = reader.ReadIsoString(32, IsoStringFlags.ACharacters);
        BootSystemUse        = reader.ReadBytes(1977);
    }

    public string BootSystemIdentifier { get; }

    public string BootIdentifier { get; }

    public byte[] BootSystemUse { get; }
}