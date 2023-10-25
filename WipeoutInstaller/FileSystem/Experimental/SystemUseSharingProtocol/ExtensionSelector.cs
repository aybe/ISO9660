using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

public sealed class ExtensionSelector : SystemUseEntry
{
    public ExtensionSelector(BinaryReader reader)
        : base(reader)
    {
        ExtensionSequence = reader.ReadIso711();
    }

    public byte ExtensionSequence { get; }
}