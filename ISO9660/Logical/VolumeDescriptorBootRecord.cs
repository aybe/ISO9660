namespace ISO9660.Logical;

public sealed class VolumeDescriptorBootRecord : VolumeDescriptor
{
    public VolumeDescriptorBootRecord(VolumeDescriptor descriptor, BinaryReader reader)
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