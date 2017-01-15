using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EasyAssertions
{
    /// <summary>
    /// Provides assertions on functions, which are unwieldly to pass into extension methods.
    /// </summary>
    public static class Should
    {
        /// <summary>
        /// Assert that a function will throw a particular type of exception.
        /// </summary>
        public static ActualException<TException> Throw<TException>(Expression<Func<object>> expression, string message = null) where TException : Exception
        {
            return Test<TException>(expression, message, () => expression.Compile()());
        }

        /// <summary>
        /// Assert that an action will throw a particular type of exception.
        /// </summary>
        public static ActualException<TException> Throw<TException>(Expression<Action> expression, string message = null) where TException : Exception
        {
            return Test<TException>(expression, message, expression.Compile());
        }

        /// <summary>
        /// Assert that a function will throw an exception.
        /// </summary>
        public static ActualException<Exception> Throw(Expression<Func<object>> expression, string message = null)
        {
            return Test<Exception>(expression, message, () => expression.Compile()());
        }

        /// <summary>
        /// Assert that an action will throw an exception.
        /// </summary>
        public static ActualException<Exception> Throw(Expression<Action> expression, string message = null)
        {
            return Test<Exception>(expression, message, expression.Compile());
        }

        private static ActualException<TException> Test<TException>(LambdaExpression expression, string message, Action executeExpression) where TException : Exception
        {
            try
            {
                executeExpression();
            }
            catch (TException e)
            {
                return new ActualException<TException>(e);
            }
            catch (Exception actual)
            {
                throw EasyAssertion.Failure(FailureMessageFormatter.Current.WrongException(typeof(TException), actual.GetType(), expression, message), actual);
            }

            throw EasyAssertion.Failure(FailureMessageFormatter.Current.NoException(typeof(TException), expression, message));
        }

        /// <summary>
        /// Assert that an asynchronous function will throw a particular type of exception.
        /// </summary>
        public static Task<ActualException<TException>> Throw<TException>(Expression<Func<Task>> expression, string message = null) where TException : Exception
        {
            return TestAsync<TException>(expression, message, async () => await expression.Compile()());
        }

        /// <summary>
        /// Assert that an asynchronous function will throw an exception.
        /// </summary>
        public static Task<ActualException<Exception>> Throw(Expression<Func<Task>> expression, string message = null)
        {
            return TestAsync<Exception>(expression, message, async () => await expression.Compile()());
        }

        private static async Task<ActualException<TException>> TestAsync<TException>(LambdaExpression expression, string message, Func<Task> executeExpression) where TException : Exception
        {
            try
            {
                await executeExpression();
            }
            catch (TException e)
            {
                return new ActualException<TException>(e);
            }
            catch (Exception actual)
            {
                throw EasyAssertion.Failure(FailureMessageFormatter.Current.WrongException(typeof(TException), actual.GetType(), expression, message), actual);
            }

            throw EasyAssertion.Failure(FailureMessageFormatter.Current.NoException(typeof(TException), expression, message));
        }
    }
}