using System;
using System.Linq.Expressions;

namespace EasyAssertions
{
    /// <summary>
    /// Provides assertions on functions, which are unwieldly to pass into extension methods.
    /// </summary>
    public static class Should
    {
        /// <summary>
        /// Assert that a function will throw an exception.
        /// </summary>
        public static Actual<TException> Throw<TException>(Expression<Func<object>> expression, string message = null) where TException : Exception
        {
            return Test<TException>(expression, message, () => expression.Compile()());
        }

        /// <summary>
        /// Assert that an action will throw an exception.
        /// </summary>
        public static Actual<TException> Throw<TException>(Expression<Action> expression, string message = null) where TException : Exception
        {
            return Test<TException>(expression, message, expression.Compile());
        }

        private static Actual<TException> Test<TException>(LambdaExpression expression, string message, Action executeExpression) where TException : Exception
        {
            try
            {
                executeExpression();
            }
            catch (TException e)
            {
                return new Actual<TException>(e);
            }
            catch (Exception actual)
            {
                throw EasyAssertion.Failure(FailureMessageFormatter.Current.WrongException(typeof(TException), actual.GetType(), expression, message), actual);
            }

            throw EasyAssertion.Failure(FailureMessageFormatter.Current.NoException(typeof(TException), expression, message));
        }
    }
}