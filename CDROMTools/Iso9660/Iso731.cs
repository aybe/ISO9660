using System;
using System.IO;
using System.Runtime.InteropServices;
using CDROMTools.Utils;

namespace CDROMTools.Iso9660
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Iso731
    {
        public readonly uint PackedValue;

        public Iso731(BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            PackedValue = reader.ReadUInt32();
        }

        public uint Value => BitConverter.IsLittleEndian
            ? PackedValue
            : BitUtils.Reverse(PackedValue);

        public override string ToString()
        {
            return $"{Value}";
        }

        public static implicit operator uint(Iso731 iso731)
        {
            return iso731.Value;
        }
    }
}