using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

public abstract class SystemUseEntry
{
    protected SystemUseEntry(BinaryReader reader)
    {
        SignatureWord = reader.ReadStringAscii(2);

        Length = reader.ReadIso711();

        SystemUseEntryVersion = reader.ReadIso711();
    }

    public string SignatureWord { get; }

    public byte Length { get; }

    public byte SystemUseEntryVersion { get; }
}