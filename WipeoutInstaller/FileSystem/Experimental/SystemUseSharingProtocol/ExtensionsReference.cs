namespace ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

public sealed class ExtensionsReference : SystemUseEntry
{
    public ExtensionsReference(BinaryReader reader)
        : base(reader)
    {
        IdentifierLength = new Iso711(reader);

        DescriptorLength = new Iso711(reader);

        SourceLength = new Iso711(reader);

        ExtensionVersion = new Iso711(reader);

        ExtensionIdentifier = new IsoString(reader, IdentifierLength, IsoStringFlags.DCharacters); // TODO or D1

        ExtensionDescriptor = new IsoString(reader, DescriptorLength, IsoStringFlags.ACharacters); // TODO or A1

        ExtensionSource = new IsoString(reader, SourceLength, IsoStringFlags.ACharacters); // TODO or A1
    }

    public Iso711 IdentifierLength { get; }

    public Iso711 DescriptorLength { get; }

    public Iso711 SourceLength { get; }

    public Iso711 ExtensionVersion { get; }

    public IsoString ExtensionIdentifier { get; }

    public IsoString ExtensionDescriptor { get; }

    public IsoString ExtensionSource { get; }
}