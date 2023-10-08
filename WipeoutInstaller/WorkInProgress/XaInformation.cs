using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.WorkInProgress;

public class XaInformation
{
    public readonly byte FileNumber;
    public readonly byte[] Reserved;
    public readonly XaAttributes Attributes;
    public readonly XaOwnerId OwnerId;
    public readonly byte SignatureByte1;
    public readonly byte SignatureByte2;

    public XaInformation(byte[] bytes)
    {
        if (bytes.Length is not 14)
        {
            throw new ArgumentOutOfRangeException(nameof(bytes));
        }

        using var reader = bytes.ToBinaryReader();

        OwnerId        = new XaOwnerId(reader);
        Attributes     = reader.Read<XaAttributes>();
        SignatureByte1 = reader.ReadByte();
        SignatureByte2 = reader.ReadByte();
        FileNumber     = reader.ReadByte();
        Reserved       = reader.ReadBytes(5);
    }
}