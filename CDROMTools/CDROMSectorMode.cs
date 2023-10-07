namespace CDROMTools
{
    /// <summary>
    ///     Defines the mode of a sector. Not to be confounded with <see cref="SectorMode" />, actually this is a refined
    ///     version of it that is returned by <see cref="CDROM.GetSectorMode" /> after further inspection of its content.
    /// </summary>
    public enum CDROMSectorMode
    {
        Mode0,
        Mode1,
        Mode2Formless,
        Mode2Form1,
        Mode2Form2,
        Reserved,
        Audio
    }
}