using ISO9660.Tests.Extensions;
using ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

namespace ISO9660.Tests.FileSystem.Experimental.RockRidge;

public sealed class ParentLink : SystemUseEntry
{
    public ParentLink(BinaryReader reader)
        : base(reader)
    {
        LocationOfParentDirectory = reader.ReadIso733();
    }

    public uint LocationOfParentDirectory { get; }
}