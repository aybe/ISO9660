namespace CDROMTools
{
    /// <summary>
    /// Defines an interface representing a CD-ROM sector.
    /// </summary>
    public interface ISector
    {
        /// <summary>
        /// Gets the 'user data' part for this instance.
        /// </summary>
        /// <returns>User data part as an array of bytes.</returns>
        byte[] GetUserData();
    }
}