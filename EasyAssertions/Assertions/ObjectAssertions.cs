using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace EasyAssertions
{
    /// <summary>
    /// Generic assertions.
    /// </summary>
    public static class ObjectAssertions
    {
        /// <summary>
        /// Asserts that two objects are equal, using the default equality comparer.
        /// </summary>
        public static Actual<TActual> ShouldBe<TActual, TExpected>([NotNull] this TActual? actual, TExpected expected, string? message = null)
            where TExpected : TActual
            where TActual : class
        {
            return actual.RegisterNotNullAssertion(c =>
                {
                    if (!c.Test.ObjectsAreEqual(actual, expected))
                    {
                        if (actual is string actualString && expected is string expectedString)
                            throw c.StandardError.NotEqual(expectedString, actualString, message: message);

                        throw c.StandardError.NotEqual(expected, actual, message);
                    }
                });
        }

        /// <summary>
        /// Asserts that two values are equal, using the default equality comparer.
        /// </summary>
        public static Actual<TActual> ShouldBe<TActual>(this TActual actual, TActual expected, string? message = null)
            where TActual : struct
        {
            actual.RegisterAssertion(c =>
                {
                    if (!c.Test.ObjectsAreEqual(actual, expected))
                        throw c.StandardError.NotEqual(expected, actual, message);
                });

            return new Actual<TActual>(actual);
        }

        /// <summary>
        /// Asserts that a nullable value is equal to another value, using the default equality comparer.
        /// </summary>
        public static Actual<TActual> ShouldBe<TActual>([NotNull] this TActual? actual, TActual expected, string? message = null)
            where TActual : struct
        {
            actual.RegisterAssertion(c =>
                {
                    if (!actual.HasValue)
                        throw c.StandardError.NotEqual(expected, actual, message);

                    if (!c.Test.ObjectsAreEqual(actual.Value, expected))
                        throw c.StandardError.NotEqual(expected, actual, message);
                });

            return new Actual<TActual>(actual!.Value);
        }

        /// <summary>
        /// Asserts that two objects are not equal, using the default equality comparer.
        /// </summary>
        public static Actual<TActual> ShouldNotBe<TActual, TNotExpected>(this TActual actual, TNotExpected notExpected, string? message = null)
            where TNotExpected : TActual
            where TActual : class?
        {
            return actual.RegisterAssertion(c =>
                {
                    if (c.Test.ObjectsAreEqual(actual, notExpected))
                        throw c.StandardError.AreEqual(notExpected, actual, message);
                });
        }

        /// <summary>
        /// Asserts that two vales are not equal, using the default equality comparer.
        /// </summary>
        public static Actual<TActual> ShouldNotBe<TActual>(this TActual actual, TActual notExpected, string? message = null)
            where TActual : struct
        {
            return actual.RegisterAssertion(c =>
                {
                    if (c.Test.ObjectsAreEqual(actual, notExpected))
                        throw c.StandardError.AreEqual(notExpected, actual, message);
                });
        }

        /// <summary>
        /// Asserts that the given object is a null reference.
        /// </summary>
        public static void ShouldBeNull<TActual>(this TActual? actual, string? message = null)
            where TActual : class
        {
            actual.RegisterAssertion(c =>
                {
                    if (!Equals(actual, null))
                        throw c.StandardError.NotEqual(null, actual, message);
                });
        }

        /// <summary>
        /// Asserts that the given object is a null reference.
        /// </summary>
        public static void ShouldBeNull<TActual>(this TActual? actual, string? message = null)
            where TActual : struct
        {
            actual.RegisterAssertion(c =>
                {
                    if (!Equals(actual, null))
                        throw c.StandardError.NotEqual(null, actual, message);
                });
        }

        /// <summary>
        /// Asserts that the given object is not a null reference.
        /// </summary>
        public static Actual<TActual> ShouldNotBeNull<TActual>([NotNull] this TActual? actual, string? message = null)
            where TActual : notnull
        {
            return actual.RegisterNotNullAssertion(c =>
                {
                    if (Equals(actual, null))
                        throw c.StandardError.IsNull(message);
                });
        }

        /// <summary>
        /// Asserts that two object instances are the same instance.
        /// </summary>
        public static Actual<TActual> ShouldReferTo<TActual, TExpected>([NotNull] this TActual? actual, TExpected expected, string? message = null)
            where TExpected : TActual
            where TActual : notnull
        {
            return actual.RegisterNotNullAssertion(c =>
                {
                    if (!ReferenceEquals(actual, expected))
                        throw c.StandardError.NotSame(expected, actual, message);
                });
        }

        /// <summary>
        /// Asserts that two object instances are different instances.
        /// </summary>
        public static Actual<TActual> ShouldNotReferTo<TActual, TNotExpected>(this TActual actual, TNotExpected notExpected, string? message = null)
            where TNotExpected : TActual
        {
            return actual.RegisterAssertion(c =>
                {
                    if (ReferenceEquals(actual, notExpected))
                        throw c.StandardError.AreSame(actual, message);
                });
        }

        /// <summary>
        /// Asserts that an object is assignable to a specified type.
        /// </summary>
        public static Actual<TExpected> ShouldBeA<TExpected>([NoEnumeration] [NotNull] this object? actual, string? message = null)
        {
            actual.RegisterNotNullAssertion(c => AssertType<TExpected>(actual, message, c));
            return new Actual<TExpected>((TExpected)actual);
        }

        internal static void AssertType<TExpected>([NotNull] object? actual, string? message, IAssertionContext context)
        {
            if (actual is not TExpected)
                throw context.StandardError.NotEqual(typeof(TExpected), actual?.GetType(), message);
        }
    }
}