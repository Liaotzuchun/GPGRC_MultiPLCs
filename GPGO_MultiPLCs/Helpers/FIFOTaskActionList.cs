using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace GPGO_MultiPLCs.Helpers
{
    /// <summary>
    /// 同一時間只會允許單一任務執行的FIFO任務執行集合
    /// </summary>
    public sealed class FIFOTaskActionList
    {
        private readonly ConcurrentQueue<Task> Tasks = new ConcurrentQueue<Task>();

        public async Task Run(Task act)
        {
            Tasks.Enqueue(act);
            while (Tasks.TryPeek(out var nextTask))
            {
                if (nextTask.Id == act.Id)
                {
                    nextTask.Start();
                    await nextTask;
                    Tasks.TryDequeue(out _);

                    break;
                }

                await nextTask;
                await Task.Delay(300);
            }
        }

        public async Task<TResult> Run<TResult>(Task<TResult> act)
        {
            var result = default(TResult);
            Tasks.Enqueue(act);
            while (Tasks.TryPeek(out var nextTask))
            {
                if (nextTask.Id == act.Id)
                {
                    var ttask = (Task<TResult>)nextTask;
                    ttask.Start();
                    result = await ttask;
                    Tasks.TryDequeue(out _);

                    break;
                }

                await nextTask;
                await Task.Delay(300);
            }

            return result;
        }
    }
}