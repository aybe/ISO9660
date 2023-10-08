using JetBrains.Annotations;

namespace WipeoutInstaller.Extensions;

[PublicAPI]
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

    protected virtual void DisposeManaged()
    {
    }

    protected virtual void DisposeNative()
    {
    }

    ~Disposable()
    {
        Dispose(false);
    }
}