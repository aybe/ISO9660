using Microsoft.Win32.SafeHandles;
using Whatever.Extensions;

namespace ISO9660.Physical;

internal sealed unsafe class AsyncDeviceIoControl : Disposable
{
    private readonly ManualResetEvent _manualResetEvent;
    private readonly NativeOverlapped* _nativeOverlapped;
    private readonly PreAllocatedOverlapped _preAllocatedOverlapped;
    private readonly ThreadPoolBoundHandle _threadPoolBoundHandle;

    public AsyncDeviceIoControl(SafeFileHandle handle, object? state, object? pinData)
    {
        _threadPoolBoundHandle = ThreadPoolBoundHandle.BindHandle(handle);

        _preAllocatedOverlapped = PreAllocatedOverlapped.UnsafeCreate(Callback, state, pinData);

        _nativeOverlapped = _threadPoolBoundHandle.AllocateNativeOverlapped(_preAllocatedOverlapped);

        _manualResetEvent = new ManualResetEvent(false);

        _nativeOverlapped->EventHandle = _manualResetEvent.SafeWaitHandle.DangerousGetHandle();
    }

    private void Callback(uint errorCode, uint numBytes, NativeOverlapped* pOVERLAP)
    {
        throw new NotImplementedException();
    }

    protected override void DisposeManaged()
    {
        _threadPoolBoundHandle.FreeNativeOverlapped(_nativeOverlapped);

        _threadPoolBoundHandle.Dispose();

        _preAllocatedOverlapped.Dispose();

        _manualResetEvent.Dispose();
    }
}