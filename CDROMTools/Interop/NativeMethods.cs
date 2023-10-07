using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace CDROMTools.Interop
{
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public partial class NativeMethods
    {
        // https://msdn.microsoft.com/en-us/library/ms182161.aspx

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetOverlappedResult(
            [In] SafeFileHandle hFile, [In] ref Overlapped lpOverlapped,
            [Out] out uint lpNumberOfBytesTransferred, [MarshalAs(UnmanagedType.Bool)] bool bWait);


        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateEvent(
            [In] IntPtr lpEventAttributes, [MarshalAs(UnmanagedType.Bool)] bool bManualReset,
            [MarshalAs(UnmanagedType.Bool)] bool bInitialState, [In] [MarshalAs(UnmanagedType.LPWStr)] string lpName);


        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle([In] IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeviceIoControl(
            [In] SafeFileHandle hDevice, uint dwIoControlCode,
            [In] IntPtr lpInBuffer, uint nInBufferSize, IntPtr lpOutBuffer, uint nOutBufferSize,
            out uint lpBytesReturned, [In, Out] ref Overlapped lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern void RtlZeroMemory(IntPtr dst, uint length);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern SafeFileHandle CreateFileW(
            [In, MarshalAs(UnmanagedType.LPWStr)] string fileName,
            [In, MarshalAs(UnmanagedType.U4)] FileAccess desiredAccess,
            [In, MarshalAs(UnmanagedType.U4)] FileShare shareMode,
            [In, Optional] IntPtr securityAttributes,
            [In, MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [In, MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            [In, Optional] IntPtr templateFile);
    }
}