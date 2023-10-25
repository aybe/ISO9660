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

        ExtensionIdentifier = reader.ReadIsoString(IdentifierLength, IsoStringFlags.DCharacters); // TODO or D1

        ExtensionDescriptor = reader.ReadIsoString(DescriptorLength, IsoStringFlags.ACharacters); // TODO or A1

        ExtensionSource = reader.ReadIsoString(SourceLength, IsoStringFlags.ACharacters); // TODO or A1
    }

    public byte IdentifierLength { get; }

    public byte DescriptorLength { get; }

    public byte SourceLength { get; }

    public byte ExtensionVersion { get; }

    public string ExtensionIdentifier { get; }

    public string ExtensionDescriptor { get; }

    public string ExtensionSource { get; }
}