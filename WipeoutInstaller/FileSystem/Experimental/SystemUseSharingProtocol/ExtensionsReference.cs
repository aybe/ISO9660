using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

public sealed class ExtensionsReference : SystemUseEntry
{
    public ExtensionsReference(BinaryReader reader)
        : base(reader)
    {
        IdentifierLength = reader.ReadIso711();

        DescriptorLength = reader.ReadIso711();

        SourceLength = reader.ReadIso711();

        ExtensionVersion = reader.ReadIso711();

        ExtensionIdentifier = new IsoString(reader, IdentifierLength, IsoStringFlags.DCharacters); // TODO or D1

        ExtensionDescriptor = new IsoString(reader, DescriptorLength, IsoStringFlags.ACharacters); // TODO or A1

        ExtensionSource = new IsoString(reader, SourceLength, IsoStringFlags.ACharacters); // TODO or A1
    }

    public byte IdentifierLength { get; }

    public byte DescriptorLength { get; }

    public byte SourceLength { get; }

    public byte ExtensionVersion { get; }

    public IsoString ExtensionIdentifier { get; }

    public IsoString ExtensionDescriptor { get; }

    public IsoString ExtensionSource { get; }
}