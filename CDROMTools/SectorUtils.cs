using System;
using System.Runtime.InteropServices;
using CDROMTools.Interop;
using CDROMTools.Utils;

namespace CDROMTools
{
    /// <summary>
    ///     Utilities for converting a sector as an array of bytes to a concrete sector type.
    /// </summary>
    public static class SectorUtils
    {
        /// <summary>
        ///     Creates an instance out of an array of bytes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T FromBytes<T>(byte[] bytes) where T : ISector, new()
        {
            if (bytes == null)throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length < NativeConstants.CD_RAW_SECTOR_SIZE)
                throw new ArgumentOutOfRangeException(nameof(bytes));
            using (var scope = new PinnedScope(bytes))
            {
                var fromPointer = FromPointer<T>(scope);
                return fromPointer;
            }
        }

        /// <summary>
        ///     Creates an instance out of a pointer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static T FromPointer<T>(IntPtr ptr) where T : ISector, new()
        {
            if (ptr == IntPtr.Zero) throw new ArgumentOutOfRangeException(nameof(ptr));
            var t = Marshal.PtrToStructure<T>(ptr);
            return t;
        }
    }
}