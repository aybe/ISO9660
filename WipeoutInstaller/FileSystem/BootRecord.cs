using System.Diagnostics.CodeAnalysis;

namespace ISO9660.Tests.FileSystem;

public sealed class BootRecord : VolumeDescriptor
{
    [SetsRequiredMembers]
    public BootRecord(VolumeDescriptor descriptor, BinaryReader reader)
        : base(descriptor)
    {
        BootSystemIdentifier = new IsoString(reader, 32, IsoStringFlags.ACharacters);
        BootIdentifier       = new IsoString(reader, 32, IsoStringFlags.ACharacters);
        BootSystemUse        = reader.ReadBytes(1977);
    }

    public IsoString BootSystemIdentifier { get; }

    public IsoString BootIdentifier { get; }

    public byte[] BootSystemUse { get; }
}