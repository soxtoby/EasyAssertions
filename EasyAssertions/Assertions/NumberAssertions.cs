using System;

namespace EasyAssertions
{
    /// <summary>
    /// Number-related assertions.
    /// </summary>
    public static class NumberAssertions
    {
        /// <summary>
        /// Asserts that two <see cref="float"/> values are within a specified tolerance of eachother.
        /// </summary>
        public static Actual<float> ShouldBe(this float actual, float expected, float tolerance, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    if (!Compare.AreWithinTolerance(actual, expected, tolerance))
                        throw EasyAssertions.Failure(FailureMessageFormatter.Current.NotEqual(expected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that two <see cref="float"/> values are not within a specified tolerance of eachother.
        /// </summary>
        public static Actual<float> ShouldNotBe(this float actual, float notExpected, float tolerance, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    if (Compare.AreWithinTolerance(actual, notExpected, tolerance))
                        throw EasyAssertions.Failure(FailureMessageFormatter.Current.AreEqual(notExpected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that two <see cref="double"/> values are within a specified tolerance of eachother.
        /// </summary>
        public static Actual<double> ShouldBe(this double actual, double expected, double delta, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    if (!Compare.AreWithinTolerance(actual, expected, delta))
                        throw EasyAssertions.Failure(FailureMessageFormatter.Current.NotEqual(expected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that two <see cref="double"/> values are not within a specified tolerance of eachother.
        /// </summary>
        public static Actual<double> ShouldNotBe(this double actual, double notExpected, double delta, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    if (Compare.AreWithinTolerance(actual, notExpected, delta))
                        throw EasyAssertions.Failure(FailureMessageFormatter.Current.AreEqual(notExpected, actual, message));
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
                        throw EasyAssertions.Failure(FailureMessageFormatter.Current.NotGreaterThan(expected, actual, message));
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
                    throw EasyAssertions.Failure(FailureMessageFormatter.Current.NotLessThan(expected, actual, message));
            });
        }
    }
}