using System;
using System.IO;
using System.Runtime.InteropServices;
using CDROMTools.Utils;

namespace CDROMTools.Iso9660
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Iso732
    {
        public readonly uint PackedValue;

        public Iso732(BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            PackedValue = reader.ReadUInt32();
        }

        public uint Value => BitConverter.IsLittleEndian
            ? BitUtils.Reverse(PackedValue)
            : PackedValue;

        public override string ToString()
        {
            return $"{Value}";
        }

        public static implicit operator uint(Iso732 iso732)
        {
            return iso732.Value;
        }
    }
}