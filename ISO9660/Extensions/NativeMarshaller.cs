using System.Runtime.InteropServices;
using Whatever.Extensions;

namespace ISO9660.Extensions;

public sealed class NativeMarshaller<T> : DisposableAsync where T : struct
{
    public NativeMarshaller(T structure = default)
    {
        Length = Marshal.SizeOf<T>();

        Pointer = Marshal.AllocHGlobal(Length);

        try
        {
            Marshal.StructureToPtr(structure, Pointer, false);
        }
        catch (Exception)
        {
            Dispose();
            throw;
        }
    }

    public int Length { get; }

    public nint Pointer { get; }

    public T Structure
    {
        get
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);

            return Marshal.PtrToStructure<T>(Pointer);
        }
        set
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);

            Marshal.StructureToPtr(value, Pointer, true);
        }
    }

    protected override ValueTask DisposeAsyncCore()
    {
        DisposePointer();

        return ValueTask.CompletedTask;
    }

    protected override void DisposeNative()
    {
        DisposePointer();
    }

    private void DisposePointer()
    {
        Marshal.FreeHGlobal(Pointer);
    }
}