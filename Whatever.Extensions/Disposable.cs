using System;
using System.Diagnostics.CodeAnalysis;

namespace Whatever.Extensions
{
    public abstract class Disposable : IDisposable
    {
        private bool IsDisposed { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            DisposeNative();

            if (disposing)
            {
                DisposeManaged();
            }

            IsDisposed = true;
        }

        [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
        protected virtual void DisposeManaged()
        {
        }

        [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
        protected virtual void DisposeNative()
        {
        }

        ~Disposable()
        {
            Dispose(false);
        }
    }
}