namespace ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

public sealed class ExtensionSelector : SystemUseEntry
{
    public ExtensionSelector(BinaryReader reader)
        : base(reader)
    {
        ExtensionSequence = new Iso711(reader);
    }

    public Iso711 ExtensionSequence { get; }
}