using System;
using System.Linq.Expressions;

namespace EasyAssertions
{
    /// <summary>
    /// Provides assertions on functions, which are unwieldy to pass into extension methods.
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
                throw EasyAssertion.Failure(FailureMessage.Standard.WrongException(typeof(TException), actual.GetType(), expression, message), actual);
            }

            throw EasyAssertion.Failure(FailureMessage.Standard.NoException(typeof(TException), expression, message));
        }
    }
}