using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace ISO9660.Physical;

internal static class NativeMethods
{
    #region ioapiset.h

    /// <summary>
    ///     https://learn.microsoft.com/en-us/windows/win32/api/ioapiset/nf-ioapiset-deviceiocontrol
    /// </summary>
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool DeviceIoControl(
        [In] nint hDevice,
        [In] uint dwIoControlCode,
        [In] [Optional] nint lpInBuffer,
        [In] uint nInBufferSize,
        [Out] [Optional] nint lpOutBuffer,
        [In] uint nOutBufferSize,
        [Out] out uint lpBytesReturned,
        [In] [Out] [Optional] nint lpOverlapped);

    /// <summary>
    ///     https://learn.microsoft.com/en-us/windows/win32/api/ioapiset/nf-ioapiset-deviceiocontrol
    /// </summary>
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern unsafe bool DeviceIoControl(
        [In] SafeFileHandle hDevice,
        [In] uint dwIoControlCode,
        [In] [Optional] nint lpInBuffer,
        [In] uint nInBufferSize,
        [Out] [Optional] nint lpOutBuffer,
        [In] uint nOutBufferSize,
        [Out] out uint lpBytesReturned,
        [In] [Out] [Optional] NativeOverlapped* lpOverlapped);

    #endregion
}