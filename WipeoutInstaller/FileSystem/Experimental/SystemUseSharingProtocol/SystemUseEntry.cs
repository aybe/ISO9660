using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

public abstract class SystemUseEntry
{
    protected SystemUseEntry(BinaryReader reader)
    {
        SignatureWord = reader.ReadStringAscii(2);

        Length = new Iso711(reader);

        SystemUseEntryVersion = new Iso711(reader);
    }

    public string SignatureWord { get; }

    public Iso711 Length { get; }

    public Iso711 SystemUseEntryVersion { get; }
}