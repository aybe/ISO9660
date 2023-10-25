using ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

namespace ISO9660.Tests.FileSystem.Experimental.RockRidge;

public sealed class FileTimeStamp : SystemUseEntry
{
    public FileTimeStamp(BinaryReader reader)
        : base(reader)
    {
        var flags = reader.ReadByte();

        reader.ReadBytes(Length - 5); // TODO contains the different dates (4.1.6)
    }
}