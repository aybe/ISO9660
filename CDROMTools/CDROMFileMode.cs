namespace CDROMTools
{
    /// <summary>
    /// Defines the mode a <see cref="CDROMFile"/> is in.
    /// </summary>
    public enum CDROMFileMode
    {
        /// <summary>
        /// Mode 1.
        /// </summary>
        Mode1,
        /// <summary>
        /// Mode 2 Form 1.
        /// </summary>
        Mode2Form1,
        /// <summary>
        /// Mode 2 Form 2.
        /// </summary>
        Mode2Form2,
        /// <summary>
        /// Mode 2 mixed (when there are intertwined <see cref="Mode2Form1"/> and <see cref="Mode2Form2"/> sectors).
        /// </summary>
        Mode2Mixed
    }
}