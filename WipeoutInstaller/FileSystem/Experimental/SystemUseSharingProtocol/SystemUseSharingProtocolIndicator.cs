namespace ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

public sealed class SystemUseSharingProtocolIndicator : SystemUseEntry
{
    public SystemUseSharingProtocolIndicator(BinaryReader reader)
        : base(reader)
    {
        CheckBytes = reader.ReadBytes(2);

        if (!CheckBytes.AsSpan().SequenceEqual(stackalloc byte[] { 0xBE, 0xEF }))
        {
            throw new InvalidDataException();
        }

        BytesSkipped = new Iso711(reader);
    }

    public byte[] CheckBytes { get; }

    public Iso711 BytesSkipped { get; }
}