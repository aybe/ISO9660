namespace ISO9660.Tests.WorkInProgress;

public readonly struct SectorSubHeader
{
    public readonly byte FileNumber;
    public readonly byte ChannelNumber;
    public readonly byte SubMode;
    public readonly byte CodingInformation;

    public override string ToString()
    {
        return
            $"{nameof(FileNumber)}: {FileNumber}, " +
            $"{nameof(ChannelNumber)}: {ChannelNumber}, " +
            $"{nameof(SubMode)}: {SubMode}, " +
            $"{nameof(CodingInformation)}: {CodingInformation}";
    }
}