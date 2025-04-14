namespace EventsAndAssignments.Services.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Метод позволяет последовательно выполнять асинхронные задачи
        /// </summary>
        /// <param name="first">Первая задача</param>
        /// <param name="next">Следующая задача</param>
        public static Task Then(this Task first, Func<Task> next)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(next);

            TaskCompletionSource<object> tcs = new();
            first.ContinueWith(_ =>
            {
                if (first.IsFaulted)
                {
                    tcs.TrySetException(first.Exception?.InnerExceptions!);
                }
                else if (first.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    try
                    {
                        Task task = next();
                        task.ContinueWith(_ =>
                        {
                            if (task.IsFaulted)
                            {
                                tcs.TrySetException(task.Exception?.InnerExceptions!);
                            }
                            else if (task.IsCanceled)
                            {
                                tcs.TrySetCanceled();
                            }
                            else
                            {
                                tcs.TrySetResult(null!);
                            }
                        }, TaskContinuationOptions.ExecuteSynchronously);
                    }
                    catch (Exception exc)
                    {
                        tcs.TrySetException(exc);
                    }
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }
    }
}