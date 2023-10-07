using WipeoutInstaller.Extensions;

namespace WipeoutInstaller;

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

[Flags]
public enum XaAttributes : ushort
{
    None = 0,
    OwnerRead = 1 << 0,
    Reserved1 = 1 << 1,
    OwnerExecute = 1 << 2,
    Reserved3 = 1 << 3,
    GroupRead = 1 << 4,
    Reserved5 = 1 << 5,
    GroupExecute = 1 << 6,
    Reserved7 = 1 << 7,
    WorldRead = 1 << 8,
    Reserved9 = 1 << 9,
    WorldExecute = 1 << 10,
    ContainsForm1Sectors = 1 << 11,
    ContainsForm2Sectors = 1 << 12,
    ContainsInterleavedSectors = 1 << 13,
    CdAudio = 1 << 14,
    Directory = 1 << 15
}

public readonly struct XaOwnerId
{
    public readonly ushort GroupId;
    public readonly ushort UserId;

    public XaOwnerId(ushort groupId, ushort userId)
    {
        GroupId = groupId;
        UserId  = userId;
    }

    public XaOwnerId(BinaryReader reader)
    {
        GroupId = reader.Read<ushort>(Endianness.BE);
        UserId  = reader.Read<ushort>(Endianness.BE);
    }

    public override string ToString()
    {
        return $"{nameof(GroupId)}: {GroupId}, {nameof(UserId)}: {UserId}";
    }
}