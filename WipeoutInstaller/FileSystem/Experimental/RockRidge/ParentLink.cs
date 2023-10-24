using ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

namespace ISO9660.Tests.FileSystem.Experimental.RockRidge;

public sealed class ParentLink : SystemUseEntry
{
    public ParentLink(BinaryReader reader)
        : base(reader)
    {
        LocationOfParentDirectory = new Iso733(reader);
    }

    public Iso733 LocationOfParentDirectory { get; }
}