using ISO9660.Tests.Extensions;
using ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

namespace ISO9660.Tests.FileSystem.Experimental.RockRidge;

public sealed class ChildLink : SystemUseEntry
{
    public ChildLink(BinaryReader reader)
        : base(reader)
    {
        LocationOfChildDirectory = reader.ReadIso733();
    }

    public uint LocationOfChildDirectory { get; }
}