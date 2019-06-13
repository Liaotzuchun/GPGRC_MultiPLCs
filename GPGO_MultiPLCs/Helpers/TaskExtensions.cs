using System;
using System.Threading;
using System.Threading.Tasks;

namespace GPGO_MultiPLCs.Helpers
{
    public static class TaskExtensions
    {
        /// <summary>
        /// task逾時後回傳task的預設回傳值，task未中止，仍會執行直到完成
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async ValueTask<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                if (await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token)).ConfigureAwait(false) == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    return await task.ConfigureAwait(false);
                }
                else
                {
                    return default;
                }
            }
        }

        /// <summary>
        /// task逾時後回傳false，task未中止，仍會執行直到完成
        /// </summary>
        /// <param name="task"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async ValueTask<bool> TimeoutAfter(this Task task, TimeSpan timeout)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                if (await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token)).ConfigureAwait(false) == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    await task.ConfigureAwait(false);

                    return true; //時限內完成
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// task逾時後中止task
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="actionTask"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async ValueTask<TResult> CancelAfter<TResult>(this Func<CancellationToken, Task<TResult>> actionTask, TimeSpan timeout)
        {
            using (var taskCts = new CancellationTokenSource())
            {
                using (var timerCts = new CancellationTokenSource())
                {
                    var task = actionTask(taskCts.Token);
                    if (await Task.WhenAny(task, Task.Delay(timeout, timerCts.Token)).ConfigureAwait(false) == task)
                    {
                        timerCts.Cancel();
                    }
                    else
                    {
                        taskCts.Cancel();
                    }

                    return await task.ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// task逾時後中止task，並回傳false
        /// </summary>
        /// <param name="actionTask"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async ValueTask<bool> CancelAfter(this Func<CancellationToken, Task> actionTask, TimeSpan timeout)
        {
            using (var taskCts = new CancellationTokenSource())
            {
                using (var timerCts = new CancellationTokenSource())
                {
                    var task = actionTask(taskCts.Token);
                    if (await Task.WhenAny(task, Task.Delay(timeout, timerCts.Token)).ConfigureAwait(false) == task)
                    {
                        timerCts.Cancel();

                        return true; //時限內完成
                    }
                    else
                    {
                        taskCts.Cancel();

                        return false;
                    }
                }
            }
        }
    }
}
