using System.Runtime.InteropServices;
using Whatever.Extensions;

namespace ISO9660.Extensions;

public sealed class NativeMemory<T> : DisposableAsync where T : unmanaged
{
    public unsafe NativeMemory(uint count, uint alignment = 1)
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
            ObjectDisposedException.ThrowIf(IsDisposed, this);

            return Manager.Memory;
        }
    }

    public Span<T> Span
    {
        get
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);

            return Memory.Span;
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

    private unsafe void DisposePointer()
    {
        NativeMemory.AlignedFree(Pointer.ToPointer());
    }
}