using System.Buffers;

namespace ISO9660.Extensions;

public readonly struct ArrayPoolScope<T> : IDisposable // TODO move to Whatever.Extensions
{
    private readonly T[] Array;

    private readonly int Length;

    private readonly ArrayPool<T> Pool;

    public ArrayPoolScope(int length, ArrayPool<T>? pool = null)
    {
        Array = (Pool = pool ?? ArrayPool<T>.Shared).Rent(Length = length);
    }

    public Memory<T> Memory => Array.AsMemory(0, Length);

    public Span<T> Span => Array.AsSpan(0, Length);

    public void Dispose()
    {
        Pool.Return(Array);
    }
}