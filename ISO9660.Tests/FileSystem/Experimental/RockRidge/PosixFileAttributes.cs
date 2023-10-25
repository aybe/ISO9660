using ISO9660.Tests.Extensions;
using ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

namespace ISO9660.Tests.FileSystem.Experimental.RockRidge;

public sealed class PosixFileAttributes : SystemUseEntry
{
    public PosixFileAttributes(BinaryReader reader)
        : base(reader)
    {
        PosixFileMode = (PosixFileMode)reader.ReadIso733();

        PosixFileLinks = reader.ReadIso733();

        PosixFileUserId = reader.ReadIso733();

        PosixFileGroupId = reader.ReadIso733();

        PosixFileSerialNumber = reader.ReadIso733(); // BUG only for Rock Ridge 1.12
    }

    public PosixFileMode PosixFileMode { get; }

    public uint PosixFileLinks { get; }

    public uint PosixFileUserId { get; }

    public uint PosixFileGroupId { get; }

    public uint PosixFileSerialNumber { get; }
}