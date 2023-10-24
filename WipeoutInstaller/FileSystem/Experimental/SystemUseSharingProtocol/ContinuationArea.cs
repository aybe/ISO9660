namespace ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

public sealed class ContinuationArea : SystemUseEntry
{
    public ContinuationArea(BinaryReader reader)
        : base(reader)
    {
        BlockLocationOfContinuationArea = new Iso733(reader);

        OffsetToStartOfContinuationArea = new Iso733(reader);

        LengthOfTheContinuationArea = new Iso733(reader);
    }

    public Iso733 BlockLocationOfContinuationArea { get; }

    public Iso733 OffsetToStartOfContinuationArea { get; }

    public Iso733 LengthOfTheContinuationArea { get; }
}