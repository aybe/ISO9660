namespace ISO9660.CDRWIN;

[Flags]
public enum CueSheetTrackFlags
{
    None = 0,
    DigitalCopyPermitted = 1 << 0,
    FourChannelAudio = 1 << 1,
    PreEmphasis = 1 << 2,
    SerialCopyManagementSystem = 1 << 3
}