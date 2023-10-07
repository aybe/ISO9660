using System;

namespace CDROMTools.Iso9660
{
    [Flags]
    public enum FileFlags : byte
    {
        File = 0x00,
        Existence = 0x01,
        Directory = 0x02,
        AssociatedFile = 0x04,
        Record = 0x08,
        Protection = 0x10,
        Reserved1 = 0x20,
        Reserved2 = 0x40,
        MultiExtent = 0x80
    }
}