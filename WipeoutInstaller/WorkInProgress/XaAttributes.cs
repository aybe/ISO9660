namespace WipeoutInstaller.WorkInProgress;

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