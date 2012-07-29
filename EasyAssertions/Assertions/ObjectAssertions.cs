
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
        public static Actual<TActual> ShouldBe<TActual, TExpected>(this TActual actual, TExpected expected, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() =>
                {
                    if (!Compare.ObjectsAreEqual(actual, expected))
                    {
                        string actualString = actual as string;
                        string expectedString = expected as string;
                        if (actualString != null && expectedString != null)
                            throw EasyAssertions.Failure(FailureMessageFormatter.Current.NotEqual(expectedString, actualString, message));

                        throw EasyAssertions.Failure(FailureMessageFormatter.Current.NotEqual(expected, actual, message));
                    }
                });
        }

        /// <summary>
        /// Asserts that a nullable value is equal to another value, using the default equality comparer.
        /// </summary>
        public static Actual<TActual> ShouldBe<TActual>(this TActual? actual, TActual expected, string message = null) where TActual : struct
        {
            actual.RegisterAssert(() =>
                {
                    if (!actual.HasValue)
                        throw EasyAssertions.Failure(FailureMessageFormatter.Current.NotEqual(expected, actual, message));

                    if (!Compare.ObjectsAreEqual(actual.Value, expected))
                        throw EasyAssertions.Failure(FailureMessageFormatter.Current.NotEqual(expected, actual, message));
                });

            return new Actual<TActual>(actual.Value);
        }

        /// <summary>
        /// Asserts that two objects are not equal, using the default equality comparer.
        /// </summary>
        public static Actual<TActual> ShouldNotBe<TActual, TNotExpected>(this TActual actual, TNotExpected notExpected, string message = null) where TNotExpected : TActual
        {
            return actual.RegisterAssert(() =>
                {
                    if (Compare.ObjectsAreEqual(actual, notExpected))
                        throw EasyAssertions.Failure(FailureMessageFormatter.Current.AreEqual(notExpected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that the given object is a null reference.
        /// </summary>
        public static void ShouldBeNull<TActual>(this TActual actual, string message = null)
        {
            actual.RegisterAssert(() =>
                {
                    if (!Equals(actual, null))
                        throw EasyAssertions.Failure(FailureMessageFormatter.Current.NotEqual(null, actual, message));
                });
        }

        /// <summary>
        /// Asserts that the given object is not a null reference.
        /// </summary>
        public static Actual<TActual> ShouldNotBeNull<TActual>(this TActual actual, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    if (Equals(actual, null))
                        throw EasyAssertions.Failure(FailureMessageFormatter.Current.IsNull(message));
                });
        }

        /// <summary>
        /// Asserts that two object instances are the same instance.
        /// </summary>
        public static Actual<TActual> ShouldBeThis<TActual, TExpected>(this TActual actual, TExpected expected, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() =>
                {
                    if (!ReferenceEquals(actual, expected))
                        throw EasyAssertions.Failure(FailureMessageFormatter.Current.NotSame(expected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that two object instances are different instances.
        /// </summary>
        public static Actual<TActual> ShouldNotBeThis<TActual, TNotExpected>(this TActual actual, TNotExpected notExpected, string message = null) where TNotExpected : TActual
        {
            return actual.RegisterAssert(() =>
                {
                    if (ReferenceEquals(actual, notExpected))
                        throw EasyAssertions.Failure(FailureMessageFormatter.Current.AreSame(actual, message));
                });
        }

        /// <summary>
        /// Asserts that an object is assignable to a specified type.
        /// </summary>
        public static Actual<TExpected> ShouldBeA<TExpected>(this object actual, string message = null)
        {
            actual.RegisterAssert(() =>
                {
                    if (!(actual is TExpected))
                        throw EasyAssertions.Failure(FailureMessageFormatter.Current.NotEqual(typeof(TExpected),
                            actual == null ? null : actual.GetType(), message));
                });

            return new Actual<TExpected>((TExpected)actual);
        }
    }
}
