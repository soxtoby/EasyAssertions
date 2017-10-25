using System;
using System.Threading.Tasks;

namespace EasyAssertions
{
    /// <summary>
    /// Task-related assertions.
    /// </summary>
    public static class TaskAssertions
    {
        internal static Func<Task, TimeSpan, bool> WaitForTask = (task, timeout) => task.Wait(timeout);
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Assert that a task completes successfully.
        /// Times out after one second.
        /// </summary>
        public static Actual<TActual> ShouldComplete<TActual>(this Task<TActual> actualTask, string message = null)
        {
            return actualTask.RegisterAssertion(c => actualTask.ShouldComplete(DefaultTimeout, message));
        }

        /// <summary>
        /// Assert that a task completes successfully within a specified number of milliseconds.
        /// </summary>
        public static Actual<TActual> ShouldComplete<TActual>(this Task<TActual> actualTask, uint millisecondsTimeout, string message = null)
        {
            return actualTask.RegisterAssertion(c => actualTask.ShouldComplete(TimeSpan.FromMilliseconds(millisecondsTimeout), message));
        }

        /// <summary>
        /// Assert that a task completes successfully within a specified time span.
        /// </summary>
        public static Actual<TActual> ShouldComplete<TActual>(this Task<TActual> actualTask, TimeSpan timeout, string message = null)
        {
            actualTask.RegisterAssertion(c => actualTask.ShouldComplete<Task<TActual>>(timeout, message));
            return new Actual<TActual>(actualTask.Result);
        }

        /// <summary>
        /// Assert that a task completes successfully.
        /// Times out after one second.
        /// </summary>
        public static void ShouldComplete(this Task actualTask, string message = null)
        {
            actualTask.RegisterAssertion(c => actualTask.ShouldComplete(DefaultTimeout, message));
        }

        /// <summary>
        /// Assert that a task completes successfully within a specified number of milliseconds.
        /// </summary>
        public static void ShouldComplete(this Task actualTask, uint millisecondsTimeout, string message = null)
        {
            actualTask.RegisterAssertion(c => actualTask.ShouldComplete(TimeSpan.FromMilliseconds(millisecondsTimeout), message));
        }

        /// <summary>
        /// Assert that a task completes successfully within a specified time span.
        /// </summary>
        public static void ShouldComplete<TTask>(this TTask actualTask, TimeSpan timeout, string message = null) 
            where TTask : Task
        {
            if (timeout < TimeSpan.Zero)
                throw NegativeTimeoutException(timeout);

            actualTask.RegisterAssertion(c =>
                {
                    actualTask.ShouldBeA<TTask>(message);

                    if (!WaitForTask(actualTask, timeout))
                        throw c.StandardError.TaskTimedOut(timeout, message);
                });
        }

        /// <summary>
        /// Assert that a task fails.
        /// Times out after 1 second.
        /// </summary>
        public static ActualException<Exception> ShouldFail(this Task actualTask, string message = null)
        {
            Actual<Exception> result = actualTask.RegisterAssertion(c => actualTask.ShouldFailWith<Exception>(DefaultTimeout, message));
            return new ActualException<Exception>(result.Value);
        }

        /// <summary>
        /// Assert that a task fails within a specified number of milliseconds.
        /// </summary>
        public static ActualException<Exception> ShouldFail(this Task actualTask, uint millisecondsTimeout, string message = null)
        {
            Actual<Exception> result = actualTask.RegisterAssertion(c => actualTask.ShouldFailWith<Exception>(TimeSpan.FromMilliseconds(millisecondsTimeout), message));
            return new ActualException<Exception>(result.Value);
        }

        /// <summary>
        /// Assert that a task fails within a specified time span.
        /// </summary>
        public static ActualException<Exception> ShouldFail(this Task actualTask, TimeSpan timeout, string message = null)
        {
            Actual<Exception> result = actualTask.RegisterAssertion(c => actualTask.ShouldFailWith<Exception>(timeout, message));
            return new ActualException<Exception>(result.Value);
        }

        /// <summary>
        /// Assert that a task fails with a particular type of exception.
        /// Times out after 1 second.
        /// </summary>
        public static ActualException<TException> ShouldFailWith<TException>(this Task actualTask, string message = null) 
            where TException : Exception
        {
            Actual<TException> result = actualTask.RegisterAssertion(c => actualTask.ShouldFailWith<TException>(DefaultTimeout, message));
            return new ActualException<TException>(result.Value);
        }

        /// <summary>
        /// Assert that a task fails within a specified number of milliseconds, with a particular type of exception.
        /// </summary>
        public static ActualException<TException> ShouldFailWith<TException>(this Task actualTask, uint millisecondsTimeout, string message = null) 
            where TException : Exception
        {
            Actual<TException> result = actualTask.RegisterAssertion(c => actualTask.ShouldFailWith<TException>(TimeSpan.FromMilliseconds(millisecondsTimeout), message));
            return new ActualException<TException>(result.Value);
        }

        /// <summary>
        /// Assert that a task fails within a specified time span, with a particular type of exception.
        /// </summary>
        public static ActualException<TException> ShouldFailWith<TException>(this Task actualTask, TimeSpan timeout, string message = null) 
            where TException : Exception
        {
            if (timeout < TimeSpan.Zero)
                throw NegativeTimeoutException(timeout);

            return (ActualException<TException>)actualTask.RegisterAssertion(c =>
                {
                    actualTask.ShouldBeA<Task>(message);

                    try
                    {
                        if (!WaitForTask(actualTask, timeout))
                            throw c.StandardError.TaskTimedOut(timeout, message);
                    }
                    catch (AggregateException e)
                    {
                        if (e.InnerException is TException expectedException)
                            return new ActualException<TException>(expectedException);

                        throw StandardErrors.Current.WrongException(typeof(TException), e.InnerException, message: message);
                    }

                    throw c.StandardError.NoException(typeof(TException), message: message);
                });
        }

        private static ArgumentOutOfRangeException NegativeTimeoutException(TimeSpan timeout)
        {
            return new ArgumentOutOfRangeException(nameof(timeout), timeout, $"{nameof(timeout)} must be {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)} or greater.");
        }
    }
}
