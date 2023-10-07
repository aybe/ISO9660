namespace CDROMTools.Interop
{
    public enum AdrSubChannelQField : byte
    {
        NoModeInformation = 0x0,

        EncodesCurrentPosition = 0x1,

        EncodesMediaCatalog = 0x2,

        EncodesIsrc = 0x3
    }
}