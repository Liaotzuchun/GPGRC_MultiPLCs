using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPGO_MultiPLCs.Helpers
{
    public class AsyncSemaphore
    {
        private static readonly Task s_completed = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> m_waiters = new Queue<TaskCompletionSource<bool>>();
        private int m_currentCount;

        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (m_waiters)
            {
                if (m_waiters.Count > 0)
                {
                    toRelease = m_waiters.Dequeue();
                }
                else
                {
                    ++m_currentCount;
                }
            }

            toRelease?.SetResult(true);
        }

        public Task WaitAsync()
        {
            lock (m_waiters)
            {
                if (m_currentCount > 0)
                {
                    --m_currentCount;

                    return s_completed;
                }

                var waiter = new TaskCompletionSource<bool>();
                m_waiters.Enqueue(waiter);

                return waiter.Task;
            }
        }

        public AsyncSemaphore(int initialCount)
        {
            if (initialCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCount));
            }

            m_currentCount = initialCount;
        }
    }
}