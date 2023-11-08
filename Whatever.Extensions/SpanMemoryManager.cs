using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace Whatever.Extensions
{
    /// <summary>
    ///     Memory manager implementation backed by a <see cref="Span{T}" />.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of items in the <see cref="Memory{T}" />.
    /// </typeparam>
    /// <remarks>
    ///     For scenarios where it is desired to obtain a <see cref="Span{T}" /> from a <see cref="Span{T}" />.
    /// </remarks>
    public unsafe class SpanMemoryManager<T> : MemoryManager<T>
        where T : unmanaged
    {
        private int Length;

        private T* Pointer;

        /// <summary>
        ///     Sets a memory span that represents the underlying memory buffer.
        /// </summary>
        /// <param name="span">
        ///     A memory span that represents the underlying memory buffer.
        /// </param>
        public void SetSpan(Span<T> span)
        {
            fixed (T* pointer = &MemoryMarshal.GetReference(span))
            {
                Pointer = pointer;
                Length  = span.Length;
            }
        }

        protected override void Dispose(bool disposing)
        {
            Pointer = default;
            Length  = default;
        }

        public override Span<T> GetSpan()
        {
            return new Span<T>(Pointer, Length);
        }

        public override MemoryHandle Pin(int elementIndex = 0)
        {
            if (elementIndex <= 0 || elementIndex >= Length)
            {
                throw new ArgumentOutOfRangeException(nameof(elementIndex));
            }

            var handle = new MemoryHandle(Pointer + elementIndex);

            return handle;
        }

        public override void Unpin()
        {
        }
    }
}