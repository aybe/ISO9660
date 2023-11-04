using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;

namespace Whatever.Extensions
{
    public static class BinaryReaderExtensions
    {
        [PublicAPI]
        public static Endianness Endianness { get; } = BitConverter.IsLittleEndian ? Endianness.LE : Endianness.BE;

        public static T Read<T>(this BinaryReader reader, Endianness? endianness = null) where T : unmanaged
        {
            var type = typeof(T);

            var size = type.IsEnum ? Marshal.SizeOf(type.GetEnumUnderlyingType()) : Marshal.SizeOf<T>();

            var data = reader.ReadBytes(size);

            if ((endianness ?? Endianness) != Endianness)
            {
                data.AsSpan().Reverse();
            }

            var read = MemoryMarshal.Read<T>(data);

            return read;
        }

        public static string ReadStringAscii(this BinaryReader reader, int length)
        {
            if (reader.BaseStream.Length - reader.BaseStream.Position < length)
            {
                throw new EndOfStreamException();
            }

            var bytes = reader.ReadBytes(length);

            var ascii = Encoding.ASCII.GetString(bytes);

            return ascii;
        }

        public static long Seek(this BinaryReader reader, long offset, SeekOrigin origin = SeekOrigin.Current)
        {
            return reader.BaseStream.Seek(offset, origin);
        }

        public static bool TryPeek<T>(this BinaryReader reader, Func<BinaryReader, T> func, out T result)
        {
            result = default!;

            var position = reader.BaseStream.Position;

            try
            {
                var value = func(reader);

                result = value;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                reader.BaseStream.Position = position;
            }
        }
    }
}