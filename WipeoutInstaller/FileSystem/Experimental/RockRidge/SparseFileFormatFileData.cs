using ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

namespace ISO9660.Tests.FileSystem.Experimental.RockRidge;

public sealed class SparseFileFormatFileData : SystemUseEntry
{
    public SparseFileFormatFileData(BinaryReader reader)
        : base(reader)
    {
        VirtualFileSizeHigh = new Iso733(reader);

        VirtualFileSizeLow = new Iso733(reader);

        TableDepth = reader.ReadByte();
    }

    public Iso733 VirtualFileSizeHigh { get; }

    public Iso733 VirtualFileSizeLow { get; }

    public byte TableDepth { get; }
}