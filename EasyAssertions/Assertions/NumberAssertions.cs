using System;

namespace EasyAssertions
{
    /// <summary>
    /// Number-related assertions.
    /// </summary>
    public static class NumberAssertions
    {
        private const string EqualityComparisonError = "Don't compare floating point numbers for direct equality. Please specify a tolerance instead.";

        [Obsolete(EqualityComparisonError, true)]
        public static void ShouldBe(this object actual, float expected, string message = null) { }

        [Obsolete(EqualityComparisonError, true)]
        public static void ShouldBe(this object actual, double expected, string message = null) { }

        [Obsolete(EqualityComparisonError, true)]
        public static void ShouldBe(this float actual, float expected, string message = null) { }

        [Obsolete(EqualityComparisonError, true)]
        public static void ShouldBe(this double actual, double expected, string message = null) { }

        /// <summary>
        /// Asserts that two <see cref="float"/> values are within a specified tolerance of eachother.
        /// </summary>
        public static Actual<float> ShouldBe(this object actual, float expected, float tolerance, string message = null)
        {
            actual.RegisterAssert(() =>
                {
                    if (!(actual is float))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.NotEqual(typeof(float), actual == null ? null : actual.GetType(), message));
                    if (!Compare.AreWithinTolerance((float)actual, expected, tolerance))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.NotEqual(expected, actual, message));
                });
            return new Actual<float>((float)actual);
        }

        /// <summary>
        /// Asserts that two <see cref="float"/> values are not within a specified tolerance of eachother.
        /// </summary>
        public static Actual<float> ShouldNotBe(this float actual, float notExpected, float tolerance, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    if (Compare.AreWithinTolerance(actual, notExpected, tolerance))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.AreEqual(notExpected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that two <see cref="double"/> values are within a specified tolerance of eachother.
        /// </summary>
        public static Actual<double> ShouldBe(this object actual, double expected, double delta, string message = null)
        {
            actual.RegisterAssert(() =>
                {
                    if (!(actual is double))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.NotEqual(typeof(double), actual == null ? null : actual.GetType(), message));
                    if (!Compare.AreWithinTolerance((double)actual, expected, delta))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.NotEqual(expected, actual, message));
                });
            return new Actual<double>((double)actual);
        }

        /// <summary>
        /// Asserts that two <see cref="double"/> values are not within a specified tolerance of eachother.
        /// </summary>
        public static Actual<double> ShouldNotBe(this double actual, double notExpected, double delta, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    if (Compare.AreWithinTolerance(actual, notExpected, delta))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.AreEqual(notExpected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that one value is greater than another.
        /// </summary>
        public static Actual<TActual> ShouldBeGreaterThan<TActual, TExpected>(this TActual actual, TExpected expected, string message = null) where TActual : IComparable<TExpected>
        {
            return actual.RegisterAssert(() =>
                {
                    if (actual.CompareTo(expected) <= 0)
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.NotGreaterThan(expected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that one value is less than another.
        /// </summary>
        public static Actual<TActual> ShouldBeLessThan<TActual, TExpected>(this TActual actual, TExpected expected, string message = null) where TActual : IComparable<TExpected>
        {
            return actual.RegisterAssert(() =>
            {
                if (actual.CompareTo(expected) >= 0)
                    throw EasyAssertion.Failure(FailureMessageFormatter.Current.NotLessThan(expected, actual, message));
            });
        }
    }
}
