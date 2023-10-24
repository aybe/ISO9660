using System.Diagnostics.CodeAnalysis;

namespace ISO9660.Tests.FileSystem.Experimental.RockRidge;

[Flags]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum PosixFileMode : uint
{
    /// <summary>
    ///     Read permission (owner). (0000400)
    /// </summary>
    S_IRUSR = 0x0100,

    /// <summary>
    ///     Write permission (owner). (0000200)
    /// </summary>
    S_IWUSR = 0x0080,

    /// <summary>
    ///     Execute permission (owner). (0000100)
    /// </summary>
    S_IXUSR = 0x0040,

    /// <summary>
    ///     Read permission (group). (0000040)
    /// </summary>
    S_IRGRP = 0x0020,

    /// <summary>
    ///     Write permission (group). (0000020)
    /// </summary>
    S_IWGRP = 0x0010,

    /// <summary>
    ///     Execute permission (group). (0000010)
    /// </summary>
    S_IXGRP = 0x0008,

    /// <summary>
    ///     Read permission (other). (0000004)
    /// </summary>
    S_IROTH = 0x0004,

    /// <summary>
    ///     Write permission (other). (0000002)
    /// </summary>
    S_IWOTH = 0x0002,

    /// <summary>
    ///     Execute permission (other). (0000001)
    /// </summary>
    S_IXOTH = 0x0001,

    /// <summary>
    ///     Set user ID on execution. (0004000)
    /// </summary>
    S_ISUID = 0x0800,

    /// <summary>
    ///     Set group ID on execution. (0002000)
    /// </summary>
    S_ISGID = 0x0400,

    /// <summary>
    ///     Enforced file locking (shared with set group ID). (0002000)
    /// </summary>
    S_ENFMT = 0x0400,

    /// <summary>
    ///     Save swapped text even after use. (0001000)
    /// </summary>
    S_ISVTX = 0x0200,

    /// <summary>
    ///     Socket. (0140000)
    /// </summary>
    S_IFSOCK = 0xC000,

    /// <summary>
    ///     Symbolic link. (0120000)
    /// </summary>
    S_IFLNK = 0xA000,

    /// <summary>
    ///     Regular file. (0100000)
    /// </summary>
    S_IFREG = 0x8000,

    /// <summary>
    ///     Block special. (0060000)
    /// </summary>
    S_IFBLK = 0x6000,

    /// <summary>
    ///     Character special. (0020000)
    /// </summary>
    S_IFCHR = 0x2000,

    /// <summary>
    ///     Directory. (0040000)
    /// </summary>
    S_IFDIR = 0x4000,

    /// <summary>
    ///     Pipe or FIFO. (0010000)
    /// </summary>
    S_IFIFO = 0x1000
}