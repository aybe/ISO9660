namespace CDROMTools.Utils
{
    public static class BitUtils
    {
        public static byte GetValue(ref byte b, byte bitOffset, byte mask)
        {
            var m = mask << bitOffset;
            return (byte) ((b & m) >> bitOffset);
        }

        public static void SetValue(ref byte b, byte bitOffset, byte mask, byte value)
        {
            var m = mask << bitOffset;
            var v = value << bitOffset;
            b = (byte) (b & ~m | v & m);
        }

        public static byte[] Unpack(uint value)
        {
            var b0 = (byte) (value >> 00 & 0xFF);
            var b1 = (byte) (value >> 08 & 0xFF);
            var b2 = (byte) (value >> 16 & 0xFF);
            var b3 = (byte) (value >> 24 & 0xFF);
            return new[] {b0, b1, b2, b3};
        }

        public static byte[] Unpack(ushort value)
        {
            var b0 = (byte) (value >> 00 & 0xFF);
            var b1 = (byte) (value >> 08 & 0xFF);
            return new[] {b0, b1};
        }


        public static byte LOBYTE(ushort value)
        {
            return (byte) (value & 0xFF);
        }

        public static byte HIBYTE(ushort value)
        {
            return (byte) (value >> 8 & 0xFF);
        }

        public static ushort LOWORD(uint value)
        {
            return (ushort) (value & 0xFFFF);
        }

        public static ushort HIWORD(uint value)
        {
            return (ushort) (value >> 16 & 0xFFFF);
        }

        public static uint Reverse(uint value)
        {
            var b1 = value >> 24 & 0xFF;
            var b2 = value >> 16 & 0xFF;
            var b3 = value >> 08 & 0xFF;
            var b4 = value >> 00 & 0xFF;
            return b1 << 00 | b2 << 08 | b3 << 16 | b4 << 24;
        }

        public static int Reverse(int value)
        {
            var b1 = value >> 24 & 0xFF;
            var b2 = value >> 16 & 0xFF;
            var b3 = value >> 08 & 0xFF;
            var b4 = value >> 00 & 0xFF;
            return b1 << 00 | b2 << 08 | b3 << 16 | b4 << 24;
        }

        public static ushort Reverse(ushort value)
        {
            var b1 = value >> 08 & 0xFF;
            var b2 = value >> 00 & 0xFF;
            return (ushort) (b1 << 00 | b2 << 08);
        }
    }
}