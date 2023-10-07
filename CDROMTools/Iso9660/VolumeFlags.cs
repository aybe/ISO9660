namespace CDROMTools.Iso9660
{
    public enum VolumeFlags : byte
    {
        /* NOTE
        should be a flags enum but since we only implement these twos
        we cheat a bit by not making it as such and ease usage btw
        */
        StrictEscapeSequences = 0,
        RelaxedEscapeSequences = 1
    }
}