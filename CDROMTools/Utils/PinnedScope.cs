using System;
using System.Runtime.InteropServices;

namespace CDROMTools.Utils
{
    public sealed class PinnedScope : IDisposable
    {
        private IntPtr _address;
        private GCHandle _handle;

        public PinnedScope(object value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var handle = GCHandle.Alloc(value, GCHandleType.Pinned);
            if (!handle.IsAllocated) throw new InvalidOperationException();
            _handle = handle;
            _address = handle.AddrOfPinnedObject();
        }

        public IntPtr Address => _address;

        #region IDisposable Members

        public void Dispose()
        {
            _handle.Free();
            _address = IntPtr.Zero;
        }

        #endregion

        public static implicit operator IntPtr(PinnedScope scope)
        {
            return scope.Address;
        }
    }
}