using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using ISO9660.WorkInProgress;
using Microsoft.Win32.SafeHandles;
using Whatever.Extensions;

namespace ISO9660.Physical;

internal sealed class Disc : DisposableAsync, IDisc
{
    public Disc(IReadOnlyList<ITrack> tracks, SafeFileHandle? handle = null)
    {
        Tracks = tracks;
        Handle = handle;
    }

    private SafeFileHandle? Handle { get; }

    public IReadOnlyList<ITrack> Tracks { get; }

    public NativeMemory<byte> GetDeviceAlignedBuffer(uint byteCount)
    {
        var alignment = GetDeviceAlignmentMask() + 1;

        var buffer = new NativeMemory<byte>(byteCount, alignment);

        return buffer;
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        foreach (var track in Tracks)
        {
            await track.DisposeAsync().ConfigureAwait(false);
        }
    }

    protected override void DisposeManaged()
    {
        foreach (var track in Tracks)
        {
            track.Dispose();
        }
    }

    private uint GetDeviceAlignmentMask()
    {
        if (Handle is null)
        {
            return 0;
        }

        var handle = Handle.DangerousGetHandle();

        if (OperatingSystem.IsWindows())
        {
            return GetDeviceAlignmentMaskWindows(handle);
        }

        throw new PlatformNotSupportedException();
    }

    [SupportedOSPlatform("windows")]
    private uint GetDeviceAlignmentMaskWindows(nint handle)
    {
        var inBufferSize = (uint)Marshal.SizeOf<NativeTypes.STORAGE_PROPERTY_QUERY>();
        var inBuffer = Marshal.AllocHGlobal((int)inBufferSize);

        var outBufferSize = (uint)Marshal.SizeOf<NativeTypes.STORAGE_ADAPTER_DESCRIPTOR>();
        var outBuffer = Marshal.AllocHGlobal((int)outBufferSize);

        var query = new NativeTypes.STORAGE_PROPERTY_QUERY
        {
            QueryType  = NativeTypes.STORAGE_QUERY_TYPE.PropertyStandardQuery,
            PropertyId = NativeTypes.STORAGE_PROPERTY_ID.StorageAdapterProperty,
        };

        Marshal.StructureToPtr(query, inBuffer, false);

        var ioctl = NativeMethods.DeviceIoControl(
            handle,
            NativeConstants.IOCTL_STORAGE_QUERY_PROPERTY,
            inBuffer,
            inBufferSize,
            outBuffer,
            outBufferSize,
            out _
        );

        var alignmentMask = 0u;

        if (ioctl)
        {
            var descriptor = Marshal.PtrToStructure<NativeTypes.STORAGE_ADAPTER_DESCRIPTOR>(outBuffer);

            alignmentMask = descriptor.AlignmentMask;
        }

        Marshal.FreeHGlobal(inBuffer);
        Marshal.FreeHGlobal(outBuffer);

        if (ioctl is false)
        {
            throw new Win32Exception();
        }

        return alignmentMask;
    }
}