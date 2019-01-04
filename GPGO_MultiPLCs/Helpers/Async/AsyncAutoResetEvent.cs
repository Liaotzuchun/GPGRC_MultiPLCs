﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPGO_MultiPLCs.Helpers
{
    public class AsyncAutoResetEvent
    {
        private static readonly Task s_completed = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> m_waits = new Queue<TaskCompletionSource<bool>>();
        private bool m_signaled;

        public bool IsSet()
        {
            lock (m_waits)
            {
                return m_signaled;
            }
        }

        public void Set()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (m_waits)
            {
                if (m_waits.Count > 0)
                {
                    toRelease = m_waits.Dequeue();
                }
                else if (!m_signaled)
                {
                    m_signaled = true;
                }
            }

            toRelease?.SetResult(true);
        }

        public Task WaitAsync()
        {
            lock (m_waits)
            {
                if (m_signaled)
                {
                    m_signaled = false;

                    return s_completed;
                }

                var tcs = new TaskCompletionSource<bool>();
                m_waits.Enqueue(tcs);

                return tcs.Task;
            }
        }
    }
}