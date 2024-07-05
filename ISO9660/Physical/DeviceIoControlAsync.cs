using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace ISO9660.Physical;

[Obsolete]
internal unsafe class DeviceIoControlAsync : IDisposable
{
    private readonly EventWaitHandle EventWaitHandle;
    private readonly SafeFileHandle Handle;
    private readonly NativeOverlapped* NativeOverlapped;
    private readonly PreAllocatedOverlapped PreAllocatedOverlapped;
    private readonly ThreadPoolBoundHandle ThreadPoolBoundHandle;

    public DeviceIoControlAsync(SafeFileHandle handle, object? state, object? pinData)
    {
        if (handle.IsInvalid)
        {
            throw new InvalidOperationException();
        }


        if (!handle.IsAsync)
        {
            throw new InvalidOperationException();
        }

        Handle = handle;

        ThreadPoolBoundHandle = ThreadPoolBoundHandle.BindHandle(handle);

        PreAllocatedOverlapped = new PreAllocatedOverlapped(Callback, state, pinData);

        NativeOverlapped = ThreadPoolBoundHandle.AllocateNativeOverlapped(PreAllocatedOverlapped);

        EventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        NativeOverlapped->EventHandle = EventWaitHandle.SafeWaitHandle.DangerousGetHandle();
    }

    public void Dispose()
    {
        ThreadPoolBoundHandle.FreeNativeOverlapped(NativeOverlapped);

        EventWaitHandle.Dispose();

        PreAllocatedOverlapped.Dispose();

        ThreadPoolBoundHandle.Dispose();
    }

    public void TestMethod2(string fileName)
    {
        using (var sfh = CreateFile(
                   fileName,
                   FileAccess.ReadWrite,
                   FileShare.Read,
                   nint.Zero,
                   FileMode.Open,
                   (FileAttributes)FileOptions.Asynchronous,
                   nint.Zero
               ))
        {
            if (sfh.IsInvalid)
            {
                throw new InvalidOperationException();
            }

            if (!sfh.IsAsync)
            {
                throw new InvalidOperationException();
            }
            var buffer = new byte[16384];

            void Callback1(uint errorCode, uint numBytes, NativeOverlapped* pOVERLAP)
            {
                Console.WriteLine(errorCode);
                Console.WriteLine(numBytes);
                Console.WriteLine();

                for (var i = 0; i < 10; i++)
                {
                    Console.WriteLine(buffer[i]);
                }
            }

            using (var tpbh = ThreadPoolBoundHandle.BindHandle(sfh))
            using (var pao = new PreAllocatedOverlapped(Callback1, 1234, buffer))
            {
                var no = tpbh.AllocateNativeOverlapped(pao);

                using (var eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset))
                {
                    no->EventHandle = eventWaitHandle.SafeWaitHandle.DangerousGetHandle();

                    var lpBytesReturned = 0u;

                    fixed (byte* pBuffer = &buffer[0])
                    {
                        var control = NativeMethods.DeviceIoControl(
                            sfh.DangerousGetHandle(),
                            NativeConstants.IOCTL_CDROM_READ_TOC_EX,
                            0,
                            0,
                            (nint)pBuffer,
                            (uint)buffer.Length,
                            out lpBytesReturned,
                            (nint)no);

                        if (control is false)
                        {
                            var win32Exception = new Win32Exception();
                            Console.WriteLine(win32Exception.Message);
                            //throw new Win32Exception();
                        }

                        var waitOne = eventWaitHandle.WaitOne();

                        Console.WriteLine(waitOne);
                    }
                    tpbh.FreeNativeOverlapped(no);
                }
            }
        }
    }

    public static Task<bool> FromWaitHandle(WaitHandle handle, TimeSpan timeout)
    {
        if (handle.WaitOne(0)) // was synchronous
        {
            return Task.FromResult(true);
        }

        if (timeout == TimeSpan.Zero)
        {
            return Task.FromResult(false);
        }

        // Register all asynchronous cases.
        var tcs = new TaskCompletionSource<bool>();

        var registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(handle, CallBack, tcs, timeout, true);

        tcs.Task.ContinueWith(s => { registeredWaitHandle.Unregister(handle); }, TaskScheduler.Default);

        return tcs.Task;

        static void CallBack(object? state, bool @out)
        {
            var tcs = (TaskCompletionSource<bool>)state!;
            tcs.TrySetResult(!@out);
        }
    }

    private void Callback(uint errorCode, uint numBytes, NativeOverlapped* pOVERLAP)
    {
        throw new NotImplementedException();
    }

    /// Return Type: HANDLE->void*
    /// lpFileName: LPCWSTR->WCHAR*
    /// dwDesiredAccess: DWORD->unsigned int
    /// dwShareMode: DWORD->unsigned int
    /// lpSecurityAttributes: LPSECURITY_ATTRIBUTES->_SECURITY_ATTRIBUTES*
    /// dwCreationDisposition: DWORD->unsigned int
    /// dwFlagsAndAttributes: DWORD->unsigned int
    /// hTemplateFile: HANDLE->void*
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern SafeFileHandle CreateFile(
        [In] [MarshalAs(UnmanagedType.LPWStr)] string fileName,
        [In] [MarshalAs(UnmanagedType.U4)] FileAccess desiredAccess,
        [In] [MarshalAs(UnmanagedType.U4)] FileShare shareMode,
        [In] [Optional] nint securityAttributes,
        [In] [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
        [In] [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
        [In] [Optional] nint templateFile);
}

internal class DeviceControl
{
    private const uint INFINITE = 0xFFFFFFFF;
    private const int ERROR_IO_PENDING = 997;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool DeviceIoControl(nint hDevice, uint dwIoControlCode,
        nint lpInBuffer, uint nInBufferSize,
        nint lpOutBuffer, uint nOutBufferSize,
        out uint lpBytesReturned, ref OVERLAPPED lpOverlapped);

    public static async Task<bool> DeviceIoControlAsync(nint hDevice, uint dwIoControlCode,
        nint lpInBuffer, uint nInBufferSize,
        nint lpOutBuffer, uint nOutBufferSize)
    {
        var overlapped = new OVERLAPPED
        {
            hEvent = CreateEvent(nint.Zero, true, false, null),
        };

        uint bytesReturned;

        var result = DeviceIoControl(hDevice, dwIoControlCode,
            lpInBuffer,                       nInBufferSize,
            lpOutBuffer,                      nOutBufferSize,
            out bytesReturned,                ref overlapped);

        if (!result)
        {
            var error = Marshal.GetLastWin32Error();

            if (error == ERROR_IO_PENDING)
            {
                // Wait for the operation to complete
                await Task.Run(() => WaitForSingleObject(overlapped.hEvent, INFINITE));
                result = true;
            }
            else
            {
                // Handle error
                result = false;
            }
        }

        CloseHandle(overlapped.hEvent);
        return result;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint CreateEvent(nint lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(nint hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint WaitForSingleObject(nint hHandle, uint dwMilliseconds);

    [StructLayout(LayoutKind.Sequential)]
    private struct OVERLAPPED
    {
        public IntPtr Internal;
        public IntPtr InternalHigh;
        public uint Offset;
        public uint OffsetHigh;
        public IntPtr hEvent;
    }
}