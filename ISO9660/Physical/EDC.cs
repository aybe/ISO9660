using System.Buffers.Binary;

namespace ISO9660.Physical;

public struct EDC
// https://github.dev/claunia/edccchk
{
    private static readonly uint[] Table = new uint[256];

    public uint Value { get; private set; }

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

    public void Hash(Span<byte> data)
    {
        foreach (var b in data)
        {
            Value = Value >> 8 ^ Table[(Value ^ b) & 0xFF];
        }
    }

    public void Reset()
    {
        Value = 0;
    }

    public override string ToString()
    {
        return $"0x{Value:X8}";
    }

    public static void Validate(Span<byte> data, Span<byte> code)
    {
        var edc = new EDC();

        edc.Hash(data);

        var x = edc.Value;
        var y = BinaryPrimitives.ReadUInt32LittleEndian(code);

        if (x != y)
        {
            throw new InvalidOperationException($"EDC mismatch: expected 0x{x:X8}, actual 0x{y:X8}.");
        }
    }
}