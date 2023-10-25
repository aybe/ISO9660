using ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

namespace ISO9660.Tests.FileSystem.Experimental.RockRidge;

public sealed class SymbolicLink : SystemUseEntry
{
    public SymbolicLink(BinaryReader reader)
        : base(reader)
    {
        Flags = reader.ReadByte();

        ComponentArea = reader.ReadBytes(Length - 6 + 1);
    }

    public byte Flags { get; }

    public byte[] ComponentArea { get; }
}