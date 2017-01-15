using System;
using System.Threading.Tasks;

namespace EasyAssertions
{
    /// <summary>
    /// Task-related assertions.
    /// </summary>
    public static class TaskAssertions
    {
        /// <summary>
        /// Assert that a task completes successfully within a specified number of milliseconds.
        /// </summary>
        public static void ShouldCompleteWithin(this Task actualTask, uint millisecondsTimeout, string message = null)
        {
            actualTask.RegisterAssert(() => AssertCompletes(actualTask, TimeSpan.FromMilliseconds(millisecondsTimeout), message));
        }

        /// <summary>
        /// Assert that a task completes successfully within a specified time span.
        /// </summary>
        public static void ShouldCompleteWithin(this Task actualTask, TimeSpan timeout, string message = null)
        {
            actualTask.RegisterAssert(() => AssertCompletes(actualTask, timeout, message));
        }

        /// <summary>
        /// Assert that a task completes successfully within a specified number of milliseconds.
        /// </summary>
        public static Actual<TActual> ShouldCompleteWithin<TActual>(this Task<TActual> actualTask, uint millisecondsTimeout, string message = null)
        {
            actualTask.RegisterAssert(() => AssertCompletes(actualTask, TimeSpan.FromMilliseconds(millisecondsTimeout), message));
            return new Actual<TActual>(actualTask.Result);
        }

        /// <summary>
        /// Assert that a task completes successfully within a specified time span.
        /// </summary>
        public static Actual<TActual> ShouldCompleteWithin<TActual>(this Task<TActual> actualTask, TimeSpan timeout, string message = null)
        {
            actualTask.RegisterAssert(() => AssertCompletes(actualTask, timeout, message));
            return new Actual<TActual>(actualTask.Result);
        }
        
        private static void AssertCompletes(Task actual, TimeSpan timeout, string message)
        {
            if (!actual.Wait(timeout))
                throw EasyAssertion.Failure(FailureMessageFormatter.Current.TaskTimedOut(timeout, message));
        }

        /// <summary>
        /// Assert that a task fails within a specified number of milliseconds, with a particular type of exception.
        /// </summary>
        public static ActualException<TException> ShouldFail<TException>(this Task actual, uint millisecondsTimeout, string message = null) where TException : Exception
        {
            actual.RegisterAssert(() => AssertFails<TException>(actual, TimeSpan.FromMilliseconds(millisecondsTimeout), message));
            return new ActualException<TException>((TException)actual.Exception.InnerException);
        }

        /// <summary>
        /// Assert that a task fails within a specified time span, with a particular type of exception.
        /// </summary>
        public static ActualException<TException> ShouldFail<TException>(this Task actual, TimeSpan timeout, string message = null) where TException : Exception
        {
            actual.RegisterAssert(() => AssertFails<TException>(actual, timeout, message));
            return new ActualException<TException>((TException)actual.Exception.InnerException);
        }

        /// <summary>
        /// Assert that a task fails within a specified number of milliseconds.
        /// </summary>
        public static ActualException<Exception> ShouldFail(this Task actual, uint millisecondsTimeout, string message = null)
        {
            actual.RegisterAssert(() => AssertFails<Exception>(actual, TimeSpan.FromMilliseconds(millisecondsTimeout), message));
            return new ActualException<Exception>(actual.Exception.InnerException);
        }

        /// <summary>
        /// Assert that a task fails within a specified time span.
        /// </summary>
        public static ActualException<Exception> ShouldFail(this Task actual, TimeSpan timeout, string message = null)
        {
            actual.RegisterAssert(() => AssertFails<Exception>(actual, timeout, message));
            return new ActualException<Exception>(actual.Exception.InnerException);
        }

        private static void AssertFails<TException>(Task actual, TimeSpan timeout, string message) where TException : Exception
        {
            try
            {
                if (!actual.Wait(timeout))
                    throw EasyAssertion.Failure(FailureMessageFormatter.Current.TaskTimedOut(timeout, message));
            }
            catch (AggregateException e)
            {
                if (e.InnerException is TException)
                    return;

                throw EasyAssertion.Failure(FailureMessageFormatter.Current.WrongException(typeof(TException), e.InnerException.GetType(), message: message), e.InnerException);
            }

            throw EasyAssertion.Failure(FailureMessageFormatter.Current.NoException(typeof(TException), message: message));
        }
    }
}
