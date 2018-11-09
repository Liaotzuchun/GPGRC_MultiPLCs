using System.Threading;
using System.Threading.Tasks;

namespace GPGO_MultiPLCs.Helpers
{
    public static class WaitHandleExtensions
    {
        public static Task WaitOneAsync(this WaitHandle waitHandle, CancellationToken cancellationToken)
        {
            return WaitOneAsync(waitHandle, Timeout.Infinite, cancellationToken);
        }

        public static async Task<bool> WaitOneAsync(this WaitHandle waitHandle, int timeout, CancellationToken cancellationToken)
        {
            if (waitHandle is Mutex)
            {
                return false;
            }

            cancellationToken.ThrowIfCancellationRequested();

            var tcs = new TaskCompletionSource<bool>();
            var rwh = ThreadPool.RegisterWaitForSingleObject(waitHandle,
                                                             (state, timedOut) =>
                                                             {
                                                                 ((TaskCompletionSource<bool>)state).SetResult(!timedOut);
                                                             },
                                                             tcs,
                                                             timeout,
                                                             true);

            using (cancellationToken.Register(() =>
                                              {
                                                  if (rwh.Unregister(null))
                                                  {
                                                      tcs.SetCanceled();
                                                  }
                                              }))
            {
                try
                {
                    return await tcs.Task.ConfigureAwait(false);
                }
                finally
                {
                    rwh.Unregister(null);
                }
            }
        }
    }
}