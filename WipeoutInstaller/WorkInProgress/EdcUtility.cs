namespace WipeoutInstaller.WorkInProgress;

public static class EdcUtility
    // https://github.com/claunia/edccchk
{
    private static readonly uint[] EdcLut = new uint[256];

    static EdcUtility()
    {
        for (var i = 0u; i < 256; i++)
        {
            var edc = i;

            for (var j = 0; j < 8; j++)
            {
                edc = (edc >> 1) ^ ((edc & 1) != 0 ? 0xD8018001 : 0);
            }

            EdcLut[i] = edc;
        }
    }

    public static uint Compute(ReadOnlySpan<byte> span)
    {
        var edc = 0u;

        foreach (var b in span)
        {
            edc = (edc >> 8) ^ EdcLut[(edc ^ b) & 0xFF];
        }

        return edc;
    }
}