using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace TinyService.Utils
{

    public static class TaskExt
    {
        private static readonly Lazy<Task> CompletedTask = new Lazy<Task>(() => FromResult<object>(null));

        public static Task FromResult()
        {
            return CompletedTask.Value;
        }

        public static Task<T> FromResult<T>(T result)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(result);
            return tcs.Task;
        }


        public static Task Delay(double milliseconds, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            var timer = new System.Timers.Timer();
            timer.Elapsed += (obj, args) => tcs.TrySetResult(true);
            timer.Interval = milliseconds <= 0 ? int.MaxValue : (int)milliseconds;
            timer.AutoReset = false;
            timer.Start();

            CancellationTokenRegistration cancellationTokenRegistration = cancellationToken.Register(() =>
            {
                timer.Stop();
                tcs.TrySetCanceled();
            });
            return tcs.Task.ContinueWith(_ =>
            {
                cancellationTokenRegistration.Dispose();
                timer.Dispose();
            }, TaskContinuationOptions.ExecuteSynchronously);
        }


    }


    public static class TaskExtensions
    {

        public static void WhenCompleted<T>(this Task<T> task, Action<Task<T>> onComplete, Action<Task<T>> onFaulted, bool execSync = false)
        {
            if (task.IsCompleted)
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    onFaulted.Invoke(task);
                    return;
                }

                onComplete.Invoke(task);
                return;
            }

            task.ContinueWith(
                              onComplete,
                execSync ?
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion :
                    TaskContinuationOptions.OnlyOnRanToCompletion);

            task.ContinueWith(
                              onFaulted,
                execSync ?
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted :
                    TaskContinuationOptions.OnlyOnFaulted);
        }

        public static void WhenCompleted(this Task task, Action<Task> onComplete, Action<Task> onFaulted, bool execSync = false)
        {
            if (task.IsCompleted)
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    onFaulted.Invoke(task);
                    return;
                }

                onComplete.Invoke(task);
                return;
            }

            task.ContinueWith(
                              onComplete,
                execSync ?
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion :
                    TaskContinuationOptions.OnlyOnRanToCompletion);

            task.ContinueWith(
                              onFaulted,
                execSync ?
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted :
                    TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
