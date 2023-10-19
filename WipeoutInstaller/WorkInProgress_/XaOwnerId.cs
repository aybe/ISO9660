using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.WorkInProgress;

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