using ISO9660.Tests.Extensions;

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

        BytesSkipped = reader.ReadIso711();
    }

    public byte[] CheckBytes { get; }

    public byte BytesSkipped { get; }
}