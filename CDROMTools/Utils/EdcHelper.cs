using System;
using JetBrains.Annotations;

namespace CDROMTools.Utils
{
    public static class EdcHelper
    {
        private static readonly byte[] ecc_f_lut = new byte[256];
        private static readonly byte[] ecc_b_lut = new byte[256];
        private static readonly uint[] edc_lut = new uint[256];

        static EdcHelper()
        {
            Init();
        }

        private static void Init()
        {
            uint i, j, edc;
            for (i = 0; i < 256; i++)
            {
                j = unchecked((uint)((i << 1) ^ ((i & 0x80) != 0 ? 0x11D : 0)));
                ecc_f_lut[i] = (byte)j;
                ecc_b_lut[i ^ j] = (byte)i;
                edc = i;
                for (j = 0; j < 8; j++)
                {
                    edc = (edc >> 1) ^ ((edc & 1) != 0 ? 0xD8018001 : 0);
                }
                edc_lut[i] = edc;
            }
        }

        /*
                public static uint ComputeBlock(uint edc, byte[] src)
                {
                    if (src == null) throw new ArgumentNullException(nameof(src));
                    var size = (ushort) src.Length;
                    var i = 0;
                    while (size-- > 0)
                    {
                        edc = (edc >> 8) ^ edc_lut[(edc ^ src[i++]) & 0xFF];
                    }
                    return edc;
                }
        */

        public static uint ComputeBlock(uint edc, [NotNull] byte[] src, int index, int count)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));
            if (src.Length == 0) throw new ArgumentException("Argument is empty collection", nameof(src));
            if (index < 0 || index >= src.Length) throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            var to = index + count;
            if (to >= src.Length) throw new ArgumentOutOfRangeException(nameof(count));
            for (var i = index; i < to; i++)
            {
                edc = (edc >> 8) ^ edc_lut[(edc ^ src[i]) & 0xFF];
            }
            return edc;
        }
    }
}