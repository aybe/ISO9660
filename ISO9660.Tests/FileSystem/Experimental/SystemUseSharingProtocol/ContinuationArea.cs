using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

public sealed class ContinuationArea : SystemUseEntry
{
    public ContinuationArea(BinaryReader reader)
        : base(reader)
    {
        BlockLocationOfContinuationArea = reader.ReadIso733();

        OffsetToStartOfContinuationArea = reader.ReadIso733();

        LengthOfTheContinuationArea = reader.ReadIso733();
    }

    public uint BlockLocationOfContinuationArea { get; }

    public uint OffsetToStartOfContinuationArea { get; }

    public uint LengthOfTheContinuationArea { get; }
}