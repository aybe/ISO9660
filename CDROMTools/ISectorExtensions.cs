using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace CDROMTools
{
    /// <summary>
    ///     Extension methods for <see cref="ISector" />.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class ISectorExtensions
    {
        /// <summary>
        ///     Gets a binary reader against the 'user data' part of a sector.
        /// </summary>
        /// <param name="sector">Sector to get a user data binary reader for.</param>
        /// <returns>Binary reader starting at byte 0, length of underlying stream will be <see cref="CDROM.RawSectorSize" />.</returns>
        public static BinaryReader GetUserDataReader([NotNull] this ISector sector)
        {
            if (sector == null) throw new ArgumentNullException(nameof(sector));
            var stream = sector.GetUserDataStream();
            var reader = new BinaryReader(stream, Encoding.ASCII);
            return reader;
        }

        private static MemoryStream GetUserDataStream([NotNull] this ISector sector)
        {
            if (sector == null) throw new ArgumentNullException(nameof(sector));
            var userData = sector.GetUserData();
            var stream = new MemoryStream(userData);
            return stream;
        }
    }
}