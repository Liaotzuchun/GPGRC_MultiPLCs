using System;
using System.Threading;
using System.Threading.Tasks;

namespace GPGO_MultiPLCs.Helpers
{
    public class AsyncCountdownEvent
    {
        private readonly AsyncManualResetEvent m_amre = new AsyncManualResetEvent();
        private int m_count;

        public void Signal()
        {
            if (m_count <= 0)
            {
                throw new InvalidOperationException();
            }

            var newCount = Interlocked.Decrement(ref m_count);
            if (newCount == 0)
            {
                m_amre.Set();
            }
            else if (newCount < 0)
            {
                throw new InvalidOperationException();
            }
        }

        public Task SignalAndWait()
        {
            Signal();

            return WaitAsync();
        }

        public Task WaitAsync()
        {
            return m_amre.WaitAsync();
        }

        public AsyncCountdownEvent(int initialCount)
        {
            if (initialCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCount));
            }

            m_count = initialCount;
        }
    }
}