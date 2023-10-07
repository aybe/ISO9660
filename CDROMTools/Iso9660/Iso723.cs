using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CDROMTools.Iso9660
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Iso723
    {
        public readonly uint PackedValue;

        public Iso723(BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            PackedValue = reader.ReadUInt32();
        }

        public ushort Value => BitConverter.IsLittleEndian
            ? (ushort) (PackedValue >> 00 & 0xFFFF)
            : (ushort) (PackedValue >> 16 & 0xFFFF);

        public override string ToString()
        {
            return $"{Value}";
        }

        public static implicit operator uint(Iso723 iso723)
        {
            return iso723.Value;
        }
    }
}