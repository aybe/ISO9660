using ISO9660.Tests.Extensions;
using ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

namespace ISO9660.Tests.FileSystem.Experimental.RockRidge;

public sealed class SparseFileFormatFileData : SystemUseEntry
{
    public SparseFileFormatFileData(BinaryReader reader)
        : base(reader)
    {
        VirtualFileSizeHigh = reader.ReadIso733();

        VirtualFileSizeLow = reader.ReadIso733();

        TableDepth = reader.ReadByte();
    }

    public uint VirtualFileSizeHigh { get; }

    public uint VirtualFileSizeLow { get; }

    public byte TableDepth { get; }
}