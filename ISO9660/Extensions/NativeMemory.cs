using System.Runtime.InteropServices;

namespace ISO9660.Extensions;

public sealed class NativeMemory<T> : IDisposable, IAsyncDisposable where T : unmanaged
{
    public unsafe NativeMemory(uint count, uint alignment = 1)
    {
        ArgumentOutOfRangeException.ThrowIfZero(alignment);

        Length = count * (uint)sizeof(T);

        Pointer = (nint)NativeMemory.AlignedAlloc(Length, alignment);

        Manager = new NativeMemoryManager<T>((T*)Pointer, (int)Length);

        Span.Clear();
    }

    private NativeMemoryManager<T>? Manager { get; set; }

    public uint Length { get; }

    public nint Pointer { get; private set; }

    public Memory<T> Memory
    {
        get
        {
            if (Manager is null)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            return Manager.Memory;
        }
    }

    public Span<T> Span
    {
        get
        {
            if (Manager is null)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            return Memory.Span;
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
        DisposeManager();

        DisposePointer();

        return ValueTask.CompletedTask;
    }

    ~NativeMemory()
    {
        Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        DisposePointer();

        if (disposing)
        {
            DisposeManager();
        }
    }

    private void DisposeManager()
    {
        if (Manager is not IDisposable disposable)
        {
            return;
        }

        disposable.Dispose();

        Manager = null;
    }

    private unsafe void DisposePointer()
    {
        if (Pointer == nint.Zero)
        {
            return;
        }

        NativeMemory.AlignedFree(Pointer.ToPointer());

        Pointer = nint.Zero;
    }
}