using System;
using System.IO;
using CDROMTools.Iso9660;

namespace CDROMTools
{
    /// <summary>
    ///     Represents a file on a CD-ROM with an ISO9660 file system.
    /// </summary>
    public sealed class CDROMFile
    {
        internal CDROMFile(string fullName, Iso733 lba, Iso733 size)
        {
            if (fullName == null) throw new ArgumentNullException(nameof(fullName));
            FullName = fullName;
            Lba = lba;
            Size = size;
        }

        /// <summary>
        /// Gets the file name extension for this instance.
        /// </summary>
        public string Extension => Path.GetExtension(FullName);

        /// <summary>
        /// Gets the file name (including extension) for this instance.
        /// </summary>
        public string FileName => Path.GetFileName(FullName);

        /// <summary>
        /// Gets the directory name for this instance.
        /// </summary>
        public string DirectoryName => Path.GetDirectoryName(FullName);


        /// <summary>
        /// Gets the full name for this instance.
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// Gets the starting address for this instance.
        /// </summary>
        public Iso733 Lba { get; }

        /// <summary>
        /// Gets the size in bytes for this instance.
        /// </summary>
        public Iso733 Size { get; }


        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return $"FileName: {FileName}, Size: {Size}";
        }

        private bool Equals(CDROMFile other)
        {
            return string.Equals(FullName, other.FullName);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is CDROMFile && Equals((CDROMFile) obj);
        }

        /// <summary>
        /// Serves as the default hash function. 
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }
    }
}