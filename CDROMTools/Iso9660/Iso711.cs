using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CDROMTools.Iso9660
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Iso711
    {
        public readonly byte Value;

        public Iso711(BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            Value = reader.ReadByte();
        }

        public override string ToString()
        {
            return $"{Value}";
        }

        public static implicit operator byte(Iso711 iso711)
        {
            return iso711.Value;
        }
    }
}