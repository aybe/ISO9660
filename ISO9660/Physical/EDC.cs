using System.Buffers.Binary;

namespace ISO9660.Physical;

public static class EDC
// https://github.dev/claunia/edccchk
{
    private static readonly uint[] Table = new uint[256];

    static EDC()
    {
        for (var i = 0u; i < Table.Length; i++)
        {
            var edc = i;

            for (var j = 0; j < 8; j++)
            {
                edc = edc >> 1 ^ ((edc & 1) != 0 ? 0xD8018001 : 0);
            }

            Table[i] = edc;
        }
    }

    private static uint Compute(Span<byte> data)
    {
        var edc = 0u;

        foreach (var b in data)
        {
            edc = edc >> 8 ^ Table[(edc ^ b) & 0xFF];
        }

        return edc;
    }

    public static void Validate(Span<byte> data, Span<byte> code)
    {
        var x = Compute(data);
        var y = BinaryPrimitives.ReadUInt32LittleEndian(code);

        if (x != y)
        {
            throw new InvalidOperationException($"EDC mismatch: expected 0x{x:X8}, actual 0x{y:X8}.");
        }
    }
}