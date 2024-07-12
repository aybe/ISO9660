using System.Runtime.InteropServices;

namespace ISO9660.Extensions;

public sealed class NativeMarshaller<T> : IDisposable, IAsyncDisposable where T : struct
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

    public nint Pointer { get; private set; }

    public T Structure
    {
        get
        {
            if (Pointer == nint.Zero)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            return Marshal.PtrToStructure<T>(Pointer);
        }
        set
        {
            if (Pointer == nint.Zero)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            Marshal.StructureToPtr(value, Pointer, true);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(false);
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private ValueTask DisposeAsyncCore()
    {
        DisposePointer();

        return ValueTask.CompletedTask;
    }

    ~NativeMarshaller()
    {
        Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        DisposePointer();

        if (disposing)
        {
            // NOP
        }
    }

    private void DisposePointer()
    {
        if (Pointer == nint.Zero)
        {
            return;
        }

        Marshal.FreeHGlobal(Pointer);

        Pointer = nint.Zero;
    }
}