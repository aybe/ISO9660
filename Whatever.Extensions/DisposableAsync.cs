using System;
using System.Threading.Tasks;

namespace Whatever.Extensions
{
    public abstract class DisposableAsync : Disposable
    {
        protected virtual async ValueTask DisposeAsyncCore()
        {
            await new ValueTask().ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            GC.SuppressFinalize(this);
        }
    }
}