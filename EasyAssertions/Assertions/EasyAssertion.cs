using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace EasyAssertions
{
    /// <summary>
    /// Framework methods for chaining assertions, defining new assertions, or customizing assertion exceptions.
    /// </summary>
    public static class EasyAssertion
    {
        /// <summary>
        /// Provides access to an object's child properties without changing the assertion chaining context.
        /// </summary>
        public static Actual<TActual> Assert<TActual>([NoEnumeration] this TActual actual, [InstantHandle] Action<TActual> assert)
        {
            return actual.RegisterUserAssertion(assert, () => assert(actual));
        }

        /// <summary>
        /// Provides access to an object's child properties without changing the assertion chaining context.
        /// </summary>
        public static Actual<TActual> And<TActual>([NoEnumeration] this Actual<TActual> actual, [InstantHandle] Action<TActual> assert)
        {
            return actual.Value.RegisterUserAssertion(assert, () => assert(actual.Value));
        }

        /// <summary>
        /// Registers an assertion action for purposes of tracking the current position in the assertion expression's source code.
        /// Use this for custom assertions that act directly on the actual value.
        /// </summary>
        public static Actual<TActual> RegisterAssertion<TActual>([NoEnumeration] this TActual actual, [InstantHandle] Action<IAssertionContext> assert)
        {
            SourceExpressionProvider.ForCurrentThread.EnterAssertion(1);
            assert(new AssertionContext());
            SourceExpressionProvider.ForCurrentThread.ExitAssertion();
            return new Actual<TActual>(actual);
        }

        /// <summary>
        /// Registers an assertion action for purposes of tracking the current position in the assertion expression's source code.
        /// Use this for custom assertions that act directly on the actual value, and assert that it is not null.
        /// </summary>
        public static Actual<TActual> RegisterNotNullAssertion<TActual>([NoEnumeration] [NotNull] this TActual? actual, [InstantHandle] Action<IAssertionContext> assert)
            where TActual : notnull
        {
            SourceExpressionProvider.ForCurrentThread.EnterAssertion(1);
            assert(new AssertionContext());
            SourceExpressionProvider.ForCurrentThread.ExitAssertion();

            if (actual is null)
                throw new ArgumentException("Actual value was null, but assertion didn't throw", nameof(assert));

            return new Actual<TActual>(actual);
        }

        /// <summary>
        /// Registers an assertion function with a named parameter for following the assertion expression into the function.
        /// Use this for custom assertions that call other existing assertions.
        /// </summary>
        /// <returns>
        /// The return value of the registered assertion function.
        /// </returns>
        public static Actual<TReturnActual> RegisterAssertion<TActual, TReturnActual>([NoEnumeration] this TActual actual, [InstantHandle] Func<IAssertionContext, Actual<TReturnActual>> assert)
        {
            SourceExpressionProvider.ForCurrentThread.EnterAssertion(1);
            var ret = assert(new AssertionContext());
            SourceExpressionProvider.ForCurrentThread.ExitAssertion();
            return ret;
        }

        /// <summary>
        /// Registers an assertion function with a named parameter for following the assertion expression into the function.
        /// Use this for custom assertions that call other existing assertions.
        /// </summary>
        /// <returns>
        /// The return value of the registered assertion function.
        /// </returns>
        public static Actual<TReturnActual> RegisterNotNullAssertion<TActual, TReturnActual>([NoEnumeration] [NotNull] this TActual? actual, [InstantHandle] Func<IAssertionContext, Actual<TReturnActual>> assert)
            where TActual : notnull
        {
            SourceExpressionProvider.ForCurrentThread.EnterAssertion(1);
            var ret = assert(new AssertionContext());
            SourceExpressionProvider.ForCurrentThread.ExitAssertion();

            if (actual is null)
                throw new ArgumentException("Actual value was null, but assertion didn't throw", nameof(assert));

            return ret;
        }

        /// <summary>
        /// Registers an assertion action that is associated with an index into the actual object.
        /// The first parameter of the item assertion method is assumed to be the actual value.
        /// Use this when executing a user-provided assertion on an item in a sequence.
        /// </summary>
        public static void RegisterIndexedAssertion<TActual>([NoEnumeration] this TActual actual, int index, [InstantHandle] Action<IAssertionContext> assert)
        {
            SourceExpressionProvider.ForCurrentThread.EnterIndexedAssertion(index, 1);
            assert(new AssertionContext());
            SourceExpressionProvider.ForCurrentThread.ExitAssertion();
        }

        /// <summary>
        /// Registers a user assertion, for purposes of following the assertion expression into the user's assertion function.
        /// </summary>
        /// <param name="actual">The value being asserted on.</param>
        /// <param name="userAssertion">A reference to the user's assertion delegate. The first parameter is assumed to be the actual value.</param>
        /// <param name="runUserAssertion">An <see cref="Action"/> that executes the user's assertion delegate.</param>
        public static Actual<TActual> RegisterUserAssertion<TActual>([NoEnumeration] this TActual actual, Delegate userAssertion, [InstantHandle] Action runUserAssertion)
        {
            SourceExpressionProvider.ForCurrentThread.EnterAssertion(userAssertion.Method, 1);
            runUserAssertion();
            SourceExpressionProvider.ForCurrentThread.ExitAssertion();
            return new Actual<TActual>(actual);
        }

        /// <summary>
        /// Overrides the exceptions used when assertions fail.
        /// Test frameworks will detect their own exception types and display the correct assertion failure messages.
        /// </summary>
        public static void UseFrameworkExceptions(Func<string, Exception>? messageExceptionFactory, Func<string, Exception, Exception>? innerExceptionExceptionFactory)
        {
            ErrorFactory.Instance.UseFrameworkExceptions(messageExceptionFactory, innerExceptionExceptionFactory);
        }

        /// <summary>
        /// Throw <see cref="EasyAssertionException"/>s when assertions fail.
        /// </summary>
        public static void UseEasyAssertionExceptions()
        {
            ErrorFactory.Instance.UseEasyAssertionExceptions();
        }
    }
}