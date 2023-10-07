using System;
using System.IO;
using System.Text;

namespace CDROMTools.Utils
{
    public static class BinaryReaderExtensions
    {
        public static string ReadStringAscii(this BinaryReader reader, int count)
        {
            var chars = reader.ReadCharsAscii(count);
            var s = new string(chars);
            return s;
        }

        public static char[] ReadCharsAscii(this BinaryReader reader, int count)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            var encoding = Encoding.ASCII;
            var bytes = reader.ReadBytes(count);
            var chars = encoding.GetChars(bytes);
            return chars;
        }
    }
}