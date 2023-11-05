namespace ISO9660.Logical;

[Flags]
public enum DirectoryRecordFlags : byte
{
    None = 0,
    Existence = 1 << 0,
    Directory = 1 << 1,
    AssociatedFile = 1 << 2,
    Record = 1 << 3,
    Protection = 1 << 4,
    Reserved1 = 1 << 5,
    Reserved2 = 1 << 6,
    MultiExtent = 1 << 7
}