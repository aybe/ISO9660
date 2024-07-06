using System.Runtime.InteropServices;
using Whatever.Extensions;

namespace ISO9660.Extensions;

public sealed class NativeMarshaller<T> : Disposable where T : struct
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
            ThrowIfDisposed();

            return Marshal.PtrToStructure<T>(Pointer);
        }
        set
        {
            ThrowIfDisposed();

            Marshal.StructureToPtr(value, Pointer, true);
        }
    }

    private void ThrowIfDisposed() // TODO add to Whatever.Extensions.Disposable
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    protected override void DisposeNative()
    {
        Marshal.FreeHGlobal(Pointer);
    }
}