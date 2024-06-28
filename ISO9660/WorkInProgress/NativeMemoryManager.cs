using System.Buffers;

namespace ISO9660.WorkInProgress;

public sealed unsafe class NativeMemoryManager<T>(T* pointer, int length)
    : MemoryManager<T> where T : unmanaged
{
    private T* Pointer { get; } = pointer;

    private int Length { get; } = length;

    protected override void Dispose(bool disposing)
    {
    }

    public override Span<T> GetSpan()
    {
        return new Span<T>(Pointer, Length);
    }

    public override MemoryHandle Pin(int elementIndex = 0)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(elementIndex);

        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(elementIndex, Length);

        return new MemoryHandle(Pointer + elementIndex);
    }

    public override void Unpin()
    {
    }
}