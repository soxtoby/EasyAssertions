namespace EasyAssertions;

/// <summary>
/// Task-related assertions.
/// </summary>
public static class ValueTaskAssertions
{
    /// <summary>
    /// Assert that a task completes successfully.
    /// Times out after one second.
    /// </summary>
    public static Actual<TActual> ShouldComplete<TActual>(this ValueTask<TActual> actualTask, string? message = null)
    {
        return actualTask.RegisterNotNullAssertion(c => actualTask.ShouldComplete(TaskAssertions.DefaultTimeout, message));
    }

    /// <summary>
    /// Assert that a task completes successfully within a specified number of milliseconds.
    /// </summary>
    public static Actual<TActual> ShouldComplete<TActual>(this ValueTask<TActual> actualTask, uint millisecondsTimeout, string? message = null)
    {
        return actualTask.RegisterNotNullAssertion(c => actualTask.ShouldComplete(TimeSpan.FromMilliseconds(millisecondsTimeout), message));
    }

    /// <summary>
    /// Assert that a task completes successfully within a specified time span.
    /// </summary>
    public static Actual<TActual> ShouldComplete<TActual>(this ValueTask<TActual> actualTask, TimeSpan timeout, string? message = null)
    {
        return actualTask.RegisterNotNullAssertion(c => AssertCompletes(actualTask, timeout, message));
    }

    static Actual<TActual> AssertCompletes<TActual>(ValueTask<TActual> actualTask, TimeSpan timeout, string? message)
    {
        if (timeout < TimeSpan.Zero)
            throw TaskAssertions.NegativeTimeoutException(timeout);

        return actualTask.RegisterNotNullAssertion(c =>
            {
                var task = actualTask.AsTask();

                if (!TaskAssertions.WaitForTask(task, timeout))
                    throw c.StandardError.TaskTimedOut(timeout, message);

                return new Actual<TActual>(task.Result);
            });
    }

    /// <summary>
    /// Assert that a task completes successfully.
    /// Times out after one second.
    /// </summary>
    public static void ShouldComplete(this ValueTask actualTask, string? message = null)
    {
        actualTask.RegisterNotNullAssertion(c => actualTask.ShouldComplete(TaskAssertions.DefaultTimeout, message));
    }

    /// <summary>
    /// Assert that a task completes successfully within a specified number of milliseconds.
    /// </summary>
    public static void ShouldComplete(this ValueTask actualTask, uint millisecondsTimeout, string? message = null)
    {
        actualTask.RegisterNotNullAssertion(c => actualTask.ShouldComplete(TimeSpan.FromMilliseconds(millisecondsTimeout), message));
    }

    /// <summary>
    /// Assert that a task completes successfully within a specified time span.
    /// </summary>
    public static void ShouldComplete(this ValueTask actualTask, TimeSpan timeout, string? message = null)
    {
        if (timeout < TimeSpan.Zero)
            throw TaskAssertions.NegativeTimeoutException(timeout);

        actualTask.RegisterNotNullAssertion(c =>
            {
                if (!TaskAssertions.WaitForTask(actualTask.AsTask(), timeout))
                    throw c.StandardError.TaskTimedOut(timeout, message);
            });
    }

    /// <summary>
    /// Assert that a task fails.
    /// Times out after 1 second.
    /// </summary>
    public static ActualException<Exception> ShouldFail(this ValueTask actualTask, string? message = null)
    {
        var result = actualTask.RegisterNotNullAssertion(c => actualTask.ShouldFail(TaskAssertions.DefaultTimeout, message));
        return new ActualException<Exception>(result.Value);
    }

    /// <summary>
    /// Assert that a task fails within a specified number of milliseconds.
    /// </summary>
    public static ActualException<Exception> ShouldFail(this ValueTask actualTask, uint millisecondsTimeout, string? message = null)
    {
        var result = actualTask.RegisterNotNullAssertion(c => actualTask.ShouldFail(TimeSpan.FromMilliseconds(millisecondsTimeout), message));
        return new ActualException<Exception>(result.Value);
    }

    /// <summary>
    /// Assert that a task fails within a specified time span.
    /// </summary>
    public static ActualException<Exception> ShouldFail(this ValueTask actualTask, TimeSpan timeout, string? message = null)
    {
        if (timeout < TimeSpan.Zero)
            throw TaskAssertions.NegativeTimeoutException(timeout);

        return (ActualException<Exception>)actualTask.RegisterNotNullAssertion(c =>
            {
                try
                {
                    if (!TaskAssertions.WaitForTask(actualTask.AsTask(), timeout))
                        throw c.StandardError.TaskTimedOut(timeout, message);
                }
                catch (AggregateException e)
                {
                    if (e.InnerException is not null)
                        return new ActualException<Exception>(e.InnerException);
                }

                throw c.StandardError.NoException(typeof(Exception), message: message);
            });
    }

    /// <summary>
    /// Assert that a task fails.
    /// Times out after 1 second.
    /// </summary>
    public static ActualException<Exception> ShouldFail<TResult>(this ValueTask<TResult> actualTask, string? message = null)
    {
        Actual<Exception> result = actualTask.RegisterNotNullAssertion(c => actualTask.ShouldFail(TaskAssertions.DefaultTimeout, message));
        return new ActualException<Exception>(result.Value);
    }

    /// <summary>
    /// Assert that a task fails within a specified number of milliseconds.
    /// </summary>
    public static ActualException<Exception> ShouldFail<TResult>(this ValueTask<TResult> actualTask, uint millisecondsTimeout, string? message = null)
    {
        Actual<Exception> result = actualTask.RegisterNotNullAssertion(c => actualTask.ShouldFail(TimeSpan.FromMilliseconds(millisecondsTimeout), message));
        return new ActualException<Exception>(result.Value);
    }

    /// <summary>
    /// Assert that a task fails within a specified time span.
    /// </summary>
    public static ActualException<Exception> ShouldFail<TResult>(this ValueTask<TResult> actualTask, TimeSpan timeout, string? message = null)
    {
        if (timeout < TimeSpan.Zero)
            throw TaskAssertions.NegativeTimeoutException(timeout);

        return (ActualException<Exception>)actualTask.RegisterNotNullAssertion(c =>
            {
                try
                {
                    if (!TaskAssertions.WaitForTask(actualTask.AsTask(), timeout))
                        throw c.StandardError.TaskTimedOut(timeout, message);
                }
                catch (AggregateException e)
                {
                    if (e.InnerException is not null)
                        return new ActualException<Exception>(e.InnerException);
                }

                throw c.StandardError.NoException(typeof(Exception), message: message);
            });
    }
}