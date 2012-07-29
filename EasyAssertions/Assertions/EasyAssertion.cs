using System;
using System.Reflection;

namespace EasyAssertions
{
    /// <summary>
    /// Framework methods for chaining assertions, defining new assertions, or customizing assertion exceptions.
    /// </summary>
    public static class EasyAssertion
    {
        private static Func<string, Exception> createMessageException;
        private static Func<string, Exception, Exception> createInnerExceptionException;

        /// <summary>
        /// Provides access to an object's child properties without changing the assertion chaining context.
        /// </summary>
        public static Actual<TActual> Assert<TActual>(this TActual actual, Action<TActual> assert)
        {
            actual.RegisterAssert(() => RegisterInnerAssert(assert.Method, () => assert(actual)));
            return new Actual<TActual>(actual);
        }

        /// <summary>
        /// Provides access to an object's child properties without changing the assertion chaining context.
        /// </summary>
        public static Actual<TActual> And<TActual>(this Actual<TActual> actual, Action<TActual> assert)
        {
            actual.RegisterAssert(() => RegisterInnerAssert(assert.Method, () => assert(actual.Value)));
            return actual;
        }

        /// <summary>
        /// Registers an assertion action for purposes of tracking the current position in the assertion expression's source code.
        /// Use this for custom assertions that act directly on the actual value.
        /// </summary>
        public static Actual<TActual> RegisterAssert<TActual>(this TActual actual, Action assert)
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod(1);
            assert();
            return new Actual<TActual>(actual);
        }

        /// <summary>
        /// Registers an assertion action with a named parameter for following the assertion expression into the action.
        /// Use this for custom assertions that call other existing assertions.
        /// </summary>
        public static Actual<TActual> RegisterAssert<TActual>(this TActual actual, Action<TActual> assert)
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod(1);
            RegisterInnerAssert(assert.Method, () => assert(actual));
            return new Actual<TActual>(actual);
        }

        /// <summary>
        /// Registers an assertion function with a named parameter for following the assertion expression into the function.
        /// Use this for custom assertions that call other existing assertions.
        /// </summary>
        /// <returns>
        /// The return value of the registered assertion function.
        /// </returns>
        public static Actual<TActual> RegisterAssert<TActual>(this TActual actual, Func<TActual, Actual<TActual>> assert)
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod(1);
            Actual<TActual> ret = null;
            RegisterInnerAssert(assert.Method, () => ret = assert(actual));
            return ret;
        }

        /// <summary>
        /// Registers an assertion action that is associated with an index into the actual object.
        /// The first parameter of the item assertion method is assumed to be the actual value.
        /// Use this when executing a user-provided assertion on an item in a sequence.
        /// </summary>
        public static void RegisterIndexedAssert(int index, MethodInfo itemAssertionMethod, Action assert)
        {
            SourceExpressionProvider.Instance.EnterIndexedAssertion(itemAssertionMethod, index);
            assert();
            SourceExpressionProvider.Instance.ExitNestedAssertion();
        }

        /// <summary>
        /// Registers an assertion action to be executed from within another assertion method,
        /// for purposes of following the assertion expression into the inner assertion method.
        /// The first parameter of the item assertion method is assumed to be the actual value.
        /// Use this when executing a user-provided assertion.
        /// </summary>
        public static void RegisterInnerAssert(MethodInfo innerAssertionMethod, Action assert)
        {
            SourceExpressionProvider.Instance.EnterNestedAssertion(innerAssertionMethod);
            assert();
            SourceExpressionProvider.Instance.ExitNestedAssertion();
        }

        /// <summary>
        /// Overrides the expceptions used when assertions fail.
        /// Test frameworks will detect their own exception types and display the correct assertion failure messages.
        /// </summary>
        public static void UseFrameworkExceptions(Func<string, Exception> messageExceptionFactory, Func<string, Exception, Exception> innerExceptionExceptionFactory)
        {
            createMessageException = messageExceptionFactory;
            createInnerExceptionException = innerExceptionExceptionFactory;
        }

        /// <summary>
        /// Throw <see cref="EasyAssertionException"/>s when assertions fail.
        /// </summary>
        public static void UseEasyAssertionExceptions()
        {
            createMessageException = null;
            createInnerExceptionException = null;
        }

        /// <summary>
        /// Creates an <see cref="Exception"/> to be thrown for a failed assertion.
        /// </summary>
        public static Exception Failure(string failureMessage)
        {
            return createMessageException != null
                ? createMessageException(failureMessage)
                : new EasyAssertionException(failureMessage);
        }

        /// <summary>
        /// Creates an <see cref="Exception"/> to be thrown for a failed assertion.
        /// </summary>
        public static Exception Failure(string failureMessage, Exception innerException)
        {
            return createInnerExceptionException != null
                ? createInnerExceptionException(failureMessage, innerException)
                : new EasyAssertionException(failureMessage, innerException);
        }
    }
}