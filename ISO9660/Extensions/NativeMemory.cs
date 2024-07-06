using System.Runtime.InteropServices;
using Whatever.Extensions;

namespace ISO9660.Extensions;

public sealed unsafe class NativeMemory<T> : Disposable where T : unmanaged
{
    public NativeMemory(uint count, uint alignment = 1)
    {
        ArgumentOutOfRangeException.ThrowIfZero(alignment);

        Length = count * (uint)sizeof(T);

        Pointer = (nint)NativeMemory.AlignedAlloc(Length, alignment);

        Manager = new NativeMemoryManager<T>((T*)Pointer, (int)Length);

        Span.Clear();
    }

    private NativeMemoryManager<T> Manager { get; }

    public uint Length { get; }

    public nint Pointer { get; }

    public Memory<T> Memory
    {
        get
        {
            ThrowIfDisposed();

            return Manager.Memory;
        }
    }

    public Span<T> Span
    {
        get
        {
            ThrowIfDisposed();

            return Memory.Span;
        }
    }

    protected override void DisposeNative()
    {
        NativeMemory.AlignedFree(Pointer.ToPointer());
    }

    private void ThrowIfDisposed()
    {
        // TODO add to Whatever.Extensions.Disposable

        if (IsDisposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }
}