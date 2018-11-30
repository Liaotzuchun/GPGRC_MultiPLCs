using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace GPGO_MultiPLCs.Helpers
{
    /// <summary>
    /// 提供先進先出可await的task自動消費集合
    /// </summary>
    public sealed class FIFOTaskActionList
    {
        private readonly ConcurrentQueue<Task> Tasks = new ConcurrentQueue<Task>();

        public async Task Run(Action act)
        {
            var task = new Task(act);
            Tasks.Enqueue(task);
            while (Tasks.TryPeek(out var nextTask))
            {
                if (nextTask.Id == task.Id)
                {
                    nextTask.Start();
                    await nextTask;
                    Tasks.TryDequeue(out _);

                    break;
                }

                await nextTask;
            }
        }

        public async Task<TResult> Run<TResult>(Func<TResult> act)
        {
            var result = default(TResult);
            var task = new Task<TResult>(act);
            Tasks.Enqueue(task);
            while (Tasks.TryPeek(out var nextTask))
            {
                if (nextTask.Id == task.Id)
                {
                    var ttask = (Task<TResult>)nextTask;
                    ttask.Start();
                    result = await ttask;
                    Tasks.TryDequeue(out _);

                    break;
                }

                await nextTask;
            }

            return result;
        }
    }
}