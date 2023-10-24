using ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

namespace ISO9660.Tests.FileSystem.Experimental.RockRidge;

public sealed class PosixFileAttributes : SystemUseEntry
{
    public PosixFileAttributes(BinaryReader reader)
        : base(reader)
    {
        PosixFileMode = (PosixFileMode)new Iso733(reader).Value1;

        PosixFileLinks = new Iso733(reader);

        PosixFileUserId = new Iso733(reader);

        PosixFileGroupId = new Iso733(reader);

        PosixFileSerialNumber = new Iso733(reader); // BUG only for Rock Ridge 1.12
    }

    public PosixFileMode PosixFileMode { get; }

    public Iso733 PosixFileLinks { get; }

    public Iso733 PosixFileUserId { get; }

    public Iso733 PosixFileGroupId { get; }

    public Iso733 PosixFileSerialNumber { get; }
}