using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace EasyAssertions
{
    /// <summary>
    /// Assertions for async streams.
    /// </summary>
    public static class AsyncStreamAssertions
    {
        /// <summary>
        /// Assert that an async stream completes successfully.
        /// Times out after one second.
        /// </summary>
        public static Actual<IReadOnlyList<TActual>> ShouldComplete<TActual>([NotNull] this IAsyncEnumerable<TActual>? actual, string? message = null)
        {
            return actual.RegisterNotNullAssertion(c => actual.ShouldComplete(TaskAssertions.DefaultTimeout, message));
        }

        /// <summary>
        /// Assert that an async stream completes successfully within a specified number of milliseconds.
        /// </summary>
        public static Actual<IReadOnlyList<TActual>> ShouldComplete<TActual>([NotNull] this IAsyncEnumerable<TActual>? actual, uint millisecondsTimeout, string? message = null)
        {
            return actual.RegisterNotNullAssertion(c => actual.ShouldComplete(TimeSpan.FromMilliseconds(millisecondsTimeout), message));
        }

        /// <summary>
        /// Assert that an async stream completes successfully within a specified time span.
        /// </summary>
        public static Actual<IReadOnlyList<TActual>> ShouldComplete<TActual>([NotNull] this IAsyncEnumerable<TActual>? actual, TimeSpan timeout, string? message = null)
        {
            if (timeout < TimeSpan.Zero)
                throw TaskAssertions.NegativeTimeoutException(timeout);

            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<IAsyncEnumerable<TActual>>(message);

                    var task = actual.ToListAsync().AsTask();

                    if (!TaskAssertions.WaitForTask(task, timeout))
                        throw c.StandardError.TaskTimedOut(timeout, message);

                    return new Actual<IReadOnlyList<TActual>>(task.Result);
                });
        }

        /// <summary>
        /// Assert that an async stream fails before it completes.
        /// Times out after 1 second.
        /// </summary>
        public static ActualException<Exception> ShouldFail<TResult>(this IAsyncEnumerable<TResult> actualStream, string? message = null)
        {
            Actual<Exception> result = actualStream.RegisterNotNullAssertion(c => actualStream.ShouldFail(TaskAssertions.DefaultTimeout, message));
            return new ActualException<Exception>(result.Value);
        }

        /// <summary>
        /// Assert that an async stream fails within a specified number of milliseconds.
        /// </summary>
        public static ActualException<Exception> ShouldFail<TResult>(this IAsyncEnumerable<TResult> actualStream, uint millisecondsTimeout, string? message = null)
        {
            Actual<Exception> result = actualStream.RegisterNotNullAssertion(c => actualStream.ShouldFail(TimeSpan.FromMilliseconds(millisecondsTimeout), message));
            return new ActualException<Exception>(result.Value);
        }

        /// <summary>
        /// Assert that an async stream fails within a specified time span.
        /// </summary>
        public static ActualException<Exception> ShouldFail<TResult>(this IAsyncEnumerable<TResult> actualStream, TimeSpan timeout, string? message = null)
        {
            if (timeout < TimeSpan.Zero)
                throw TaskAssertions.NegativeTimeoutException(timeout);

            return (ActualException<Exception>)actualStream.RegisterNotNullAssertion(c =>
                {
                    try
                    {
                        if (!TaskAssertions.WaitForTask(actualStream.ToListAsync().AsTask(), timeout))
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
}