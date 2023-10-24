namespace ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

public sealed class PaddingField : SystemUseEntry
{
    public PaddingField(BinaryReader reader)
        : base(reader)
    {
        PaddingArea = reader.ReadBytes(Length - 5 + 1);
    }

    public byte[] PaddingArea { get; }
}