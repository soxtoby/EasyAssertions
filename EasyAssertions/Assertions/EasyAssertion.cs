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
            return actual.RegisterAssertion(c => c.Call(() => assert(actual)));
        }

        /// <summary>
        /// Provides access to an object's child properties without changing the assertion chaining context.
        /// </summary>
        public static Actual<TActual> And<TActual>([NoEnumeration] this Actual<TActual> actual, [InstantHandle] Action<TActual> assert)
        {
            return actual.Value.RegisterAssertion(c => c.Call(() => assert(actual.Value)));
        }

        /// <summary>
        /// Registers an assertion action for purposes of tracking the current position in the assertion expression's source code.
        /// Use this for custom assertions that act directly on the actual value.
        /// </summary>
        public static Actual<TActual> RegisterAssertion<TActual>([NoEnumeration] this TActual actual, [InstantHandle] Action<IAssertionContext> assert, string actualSuffix = "", string expectedSuffix = "")
        {
            SourceExpressionProvider.Current.RunAssertion(assert, actualSuffix, expectedSuffix);
            return new Actual<TActual>(actual);
        }

        /// <summary>
        /// Registers an assertion action for purposes of tracking the current position in the assertion expression's source code.
        /// Use this for custom assertions that act directly on the actual value, and assert that it is not null.
        /// </summary>
        public static Actual<TActual> RegisterNotNullAssertion<TActual>([NoEnumeration] [NotNull] this TActual? actual, [InstantHandle] Action<IAssertionContext> assert, string actualSuffix = "", string expectedSuffix = "")
            where TActual : notnull
        {
            SourceExpressionProvider.Current.RunAssertion(assert, actualSuffix, expectedSuffix);

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
        public static Actual<TReturnActual> RegisterAssertion<TActual, TReturnActual>([NoEnumeration] this TActual actual, [InstantHandle] Func<IAssertionContext, Actual<TReturnActual>> assert, string actualSuffix = "", string expectedSuffix = "")
        {
            Actual<TReturnActual> ret = default!;

            SourceExpressionProvider.Current.RunAssertion(c => ret = assert(c), actualSuffix, expectedSuffix);

            return ret;
        }

        /// <summary>
        /// Registers an assertion function with a named parameter for following the assertion expression into the function.
        /// Use this for custom assertions that call other existing assertions.
        /// </summary>
        /// <returns>
        /// The return value of the registered assertion function.
        /// </returns>
        public static Actual<TReturnActual> RegisterNotNullAssertion<TActual, TReturnActual>([NoEnumeration] [NotNull] this TActual? actual, [InstantHandle] Func<IAssertionContext, Actual<TReturnActual>> assert, string actualSuffix = "", string expectedSuffix = "")
            where TActual : notnull
        {
            Actual<TReturnActual> ret = default!;

            SourceExpressionProvider.Current.RunAssertion(c => ret = assert(c), actualSuffix, expectedSuffix);

            if (actual is null)
                throw new ArgumentException("Actual value was null, but assertion didn't throw", nameof(assert));

            return ret;
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