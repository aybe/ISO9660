using ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

namespace ISO9660.Tests.FileSystem.Experimental.RockRidge;

public sealed class ChildLink : SystemUseEntry
{
    public ChildLink(BinaryReader reader)
        : base(reader)
    {
        LocationOfChildDirectory = new Iso733(reader);
    }

    public Iso733 LocationOfChildDirectory { get; }
}