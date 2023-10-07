using System;
using System.Runtime.InteropServices;

namespace CDROMTools.Interop
{
    /// <summary>
    /// Shim to make the overall thing a little more 'friendly'.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct OverlappedOffsetShim
    {
        [FieldOffset(0)] public OverlappedOffset Offset;
        [FieldOffset(0)] public IntPtr Pointer;
    }
}