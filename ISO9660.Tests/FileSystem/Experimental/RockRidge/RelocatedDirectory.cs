using ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

namespace ISO9660.Tests.FileSystem.Experimental.RockRidge;

public sealed class RelocatedDirectory : SystemUseEntry
{
    public RelocatedDirectory(BinaryReader reader)
        : base(reader)
    {
        // nothing
    }
}