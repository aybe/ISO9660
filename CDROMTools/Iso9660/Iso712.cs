using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CDROMTools.Iso9660
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Iso712
    {
        public readonly sbyte Value;

        public Iso712(BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            Value = reader.ReadSByte();
        }

        public override string ToString()
        {
            return $"{Value}";
        }

        public static implicit operator sbyte(Iso712 iso712)
        {
            return iso712.Value;
        }
    }
}