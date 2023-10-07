using System;
using System.IO;
using System.Runtime.InteropServices;
using CDROMTools.Utils;

namespace CDROMTools.Iso9660
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Iso722
    {
        public readonly ushort PackedValue;

        public Iso722(BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            PackedValue = reader.ReadUInt16();
        }

        public ushort Value => BitConverter.IsLittleEndian
            ? BitUtils.Reverse(PackedValue)
            : PackedValue;

        public override string ToString()
        {
            return $"{Value}";
        }

        public static implicit operator ushort(Iso722 iso722)
        {
            return iso722.Value;
        }
    }
}