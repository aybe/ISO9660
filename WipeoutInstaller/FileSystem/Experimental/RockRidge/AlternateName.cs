using ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

namespace ISO9660.Tests.FileSystem.Experimental.RockRidge;

public sealed class AlternateName : SystemUseEntry
{
    public AlternateName(BinaryReader reader)
        : base(reader)
    {
        Flags = reader.ReadByte();

        NameContent = reader.ReadBytes(Length - 6 + 1);
    }

    public byte Flags { get; }

    public byte[] NameContent { get; }
}