using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CDROMTools.Iso9660
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Iso733
    {
        public readonly ulong PackedValue;

        public Iso733(BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            PackedValue = reader.ReadUInt64();
        }

        public uint Value => BitConverter.IsLittleEndian
            ? (uint) (PackedValue >> 00 & 0xFFFFFFFF)
            : (uint) (PackedValue >> 32 & 0xFFFFFFFF);

        public override string ToString()
        {
            return $"{Value}";
        }

        public static implicit operator uint(Iso733 iso733)
        {
            return iso733.Value;
        }
    }
}