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
        public static void ShouldComplete(this Task actualTask, string message = null)
        {
            actualTask.RegisterAssert(() => AssertCompletes(actualTask, DefaultTimeout, message));
        }

        /// <summary>
        /// Assert that a task completes successfully within a specified number of milliseconds.
        /// </summary>
        public static void ShouldComplete(this Task actualTask, uint millisecondsTimeout, string message = null)
        {
            actualTask.RegisterAssert(() => AssertCompletes(actualTask, TimeSpan.FromMilliseconds(millisecondsTimeout), message));
        }

        /// <summary>
        /// Assert that a task completes successfully within a specified time span.
        /// </summary>
        public static void ShouldComplete(this Task actualTask, TimeSpan timeout, string message = null)
        {
            actualTask.RegisterAssert(() => AssertCompletes(actualTask, timeout, message));
        }

        /// <summary>
        /// Assert that a task completes successfully.
        /// Times out after one second.
        /// </summary>
        public static Actual<TActual> ShouldComplete<TActual>(this Task<TActual> actualTask, string message = null)
        {
            actualTask.RegisterAssert(() => AssertCompletes(actualTask, DefaultTimeout, message));
            return new Actual<TActual>(actualTask.Result);
        }

        /// <summary>
        /// Assert that a task completes successfully within a specified number of milliseconds.
        /// </summary>
        public static Actual<TActual> ShouldComplete<TActual>(this Task<TActual> actualTask, uint millisecondsTimeout, string message = null)
        {
            actualTask.RegisterAssert(() => AssertCompletes(actualTask, TimeSpan.FromMilliseconds(millisecondsTimeout), message));
            return new Actual<TActual>(actualTask.Result);
        }

        /// <summary>
        /// Assert that a task completes successfully within a specified time span.
        /// </summary>
        public static Actual<TActual> ShouldComplete<TActual>(this Task<TActual> actualTask, TimeSpan timeout, string message = null)
        {
            actualTask.RegisterAssert(() => AssertCompletes(actualTask, timeout, message));
            return new Actual<TActual>(actualTask.Result);
        }
        
        private static void AssertCompletes<TTask>(TTask actualTask, TimeSpan timeout, string message) where TTask : Task
        {
            if (timeout < TimeSpan.Zero)
                throw NegativeTimeoutException(timeout);

            actualTask.ShouldBeA<TTask>(message);

            if (!WaitForTask(actualTask, timeout))
                throw EasyAssertion.Failure(FailureMessage.Standard.TaskTimedOut(timeout, message));
        }

        /// <summary>
        /// Assert that a task fails with a particular type of exception.
        /// Times out after 1 second.
        /// </summary>
        public static ActualException<TException> ShouldFail<TException>(this Task actualTask, string message = null) where TException : Exception
        {
            actualTask.RegisterAssert(() => AssertFails<TException>(actualTask, DefaultTimeout, message));
            return new ActualException<TException>((TException)actualTask.Exception.InnerException);
        }

        /// <summary>
        /// Assert that a task fails within a specified number of milliseconds, with a particular type of exception.
        /// </summary>
        public static ActualException<TException> ShouldFail<TException>(this Task actualTask, uint millisecondsTimeout, string message = null) where TException : Exception
        {
            actualTask.RegisterAssert(() => AssertFails<TException>(actualTask, TimeSpan.FromMilliseconds(millisecondsTimeout), message));
            return new ActualException<TException>((TException)actualTask.Exception.InnerException);
        }

        /// <summary>
        /// Assert that a task fails within a specified time span, with a particular type of exception.
        /// </summary>
        public static ActualException<TException> ShouldFail<TException>(this Task actualTask, TimeSpan timeout, string message = null) where TException : Exception
        {
            actualTask.RegisterAssert(() => AssertFails<TException>(actualTask, timeout, message));
            return new ActualException<TException>((TException)actualTask.Exception.InnerException);
        }

        /// <summary>
        /// Assert that a task fails.
        /// Times out after 1 second.
        /// </summary>
        public static ActualException<Exception> ShouldFail(this Task actualTask, string message = null)
        {
            actualTask.RegisterAssert(() => AssertFails<Exception>(actualTask, DefaultTimeout, message));
            return new ActualException<Exception>(actualTask.Exception.InnerException);
        }

        /// <summary>
        /// Assert that a task fails within a specified number of milliseconds.
        /// </summary>
        public static ActualException<Exception> ShouldFail(this Task actualTask, uint millisecondsTimeout, string message = null)
        {
            actualTask.RegisterAssert(() => AssertFails<Exception>(actualTask, TimeSpan.FromMilliseconds(millisecondsTimeout), message));
            return new ActualException<Exception>(actualTask.Exception.InnerException);
        }

        /// <summary>
        /// Assert that a task fails within a specified time span.
        /// </summary>
        public static ActualException<Exception> ShouldFail(this Task actualTask, TimeSpan timeout, string message = null)
        {
            actualTask.RegisterAssert(() => AssertFails<Exception>(actualTask, timeout, message));
            return new ActualException<Exception>(actualTask.Exception.InnerException);
        }

        private static void AssertFails<TException>(Task actualTask, TimeSpan timeout, string message) where TException : Exception
        {
            if (timeout < TimeSpan.Zero)
                throw NegativeTimeoutException(timeout);

            actualTask.ShouldBeA<Task>(message);

            try
            {
                if (!WaitForTask(actualTask, timeout))
                    throw EasyAssertion.Failure(FailureMessage.Standard.TaskTimedOut(timeout, message));
            }
            catch (AggregateException e)
            {
                if (e.InnerException is TException)
                    return;

                throw EasyAssertion.Failure(FailureMessage.Standard.WrongException(typeof(TException), e.InnerException.GetType(), message: message), e.InnerException);
            }

            throw EasyAssertion.Failure(FailureMessage.Standard.NoException(typeof(TException), message: message));
        }

        private static ArgumentOutOfRangeException NegativeTimeoutException(TimeSpan timeout)
        {
            return new ArgumentOutOfRangeException(nameof(timeout), timeout, $"{nameof(timeout)} must be {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)} or greater.");
        }
    }
}
