using System;

namespace EasyAssertions
{
    /// <summary>
    /// Number-related assertions.
    /// </summary>
    public static class NumberAssertions
    {
        private const string EqualityComparisonError = "Don't compare floating point numbers directly with other types. Please specify a tolerance instead.";

        [Obsolete(EqualityComparisonError, true)]
        public static void ShouldBe(this object actual, float expected, string message = null) { }

        [Obsolete(EqualityComparisonError, true)]
        public static void ShouldBe(this object actual, double expected, string message = null) { }

        [Obsolete(EqualityComparisonError, true)]
        public static void ShouldNotBe(this object actual, float expected, string message = null) { }

        [Obsolete(EqualityComparisonError, true)]
        public static void ShouldNotBe(this object actual, double expected, string message = null) { }

        /// <summary>
        /// Asserts that two <see cref="float"/> values are within a specified tolerance of eachother.
        /// </summary>
        public static Actual<float> ShouldBe(this float actual, float expected, double tolerance, string message = null)
        {
            return actual.RegisterAssertion(c =>
                {
                    if (!c.Test.AreWithinTolerance(actual, expected, tolerance))
                        throw c.StandardError.NotEqual(expected, actual, message);
                });
        }

        /// <summary>
        /// Asserts that a <see cref="float"/> and a <see cref="double"/> are within a specified tolerance of eachother.
        /// </summary>
        public static Actual<float> ShouldBe(this float actual, double expected, double tolerance, string message = null)
        {
            return actual.RegisterAssertion(c =>
                {
                    if (!c.Test.AreWithinTolerance(actual, expected, tolerance))
                        throw c.StandardError.NotEqual(expected, actual, message);
                });
        }

        /// <summary>
        /// Asserts that two <see cref="double"/> values are within a specified tolerance of eachother.
        /// </summary>
        public static Actual<double> ShouldBe(this double actual, float expected, double tolerance, string message = null)
        {
            return actual.RegisterAssertion(c =>
                {
                    if (!c.Test.AreWithinTolerance(actual, expected, tolerance))
                        throw c.StandardError.NotEqual(expected, actual, message);
                });
        }

        /// <summary>
        /// Asserts that a <see cref="double"/> and a <see cref="float"/> are within a specified tolerance of eachother.
        /// </summary>
        public static Actual<double> ShouldBe(this double actual, double expected, double tolerance, string message = null)
        {
            return actual.RegisterAssertion(c =>
                {
                    if (!c.Test.AreWithinTolerance(actual, expected, tolerance))
                        throw c.StandardError.NotEqual(expected, actual, message);
                });
        }

        /// <summary>
        /// Asserts that two <see cref="float"/> values are not within a specified tolerance of eachother.
        /// </summary>
        public static Actual<float> ShouldNotBe(this float actual, float notExpected, float tolerance, string message = null)
        {
            return actual.RegisterAssertion(c =>
                {
                    if (c.Test.AreWithinTolerance(actual, notExpected, tolerance))
                        throw c.StandardError.AreEqual(notExpected, actual, message);
                });
        }

        /// <summary>
        /// Asserts that two <see cref="double"/> values are not within a specified tolerance of eachother.
        /// </summary>
        public static Actual<double> ShouldNotBe(this double actual, double notExpected, double tolerance, string message = null)
        {
            return actual.RegisterAssertion(c =>
                {
                    if (c.Test.AreWithinTolerance(actual, notExpected, tolerance))
                        throw c.StandardError.AreEqual(notExpected, actual, message);
                });
        }

        /// <summary>
        /// Asserts that one value is greater than another.
        /// </summary>
        public static Actual<TActual> ShouldBeGreaterThan<TActual, TExpected>(this TActual actual, TExpected expected, string message = null)
            where TActual : IComparable<TExpected>
        {
            return actual.RegisterAssertion(c =>
                {
                    if (actual.CompareTo(expected) <= 0)
                        throw c.StandardError.NotGreaterThan(expected, actual, message);
                });
        }

        /// <summary>
        /// Asserts that one value is less than another.
        /// </summary>
        public static Actual<TActual> ShouldBeLessThan<TActual, TExpected>(this TActual actual, TExpected expected, string message = null)
            where TActual : IComparable<TExpected>
        {
            return actual.RegisterAssertion(c =>
            {
                if (actual.CompareTo(expected) >= 0)
                    throw c.StandardError.NotLessThan(expected, actual, message);
            });
        }

        /// <summary>
        /// Asserts that a <see cref="float"/> value is NaN.
        /// </summary>
        public static void ShouldBeNaN(this float actual, string message = null)
        {
            actual.RegisterAssertion(c =>
                {
                    if (!float.IsNaN(actual))
                        throw c.StandardError.NotEqual(float.NaN, actual, message);
                });
        }

        /// <summary>
        /// Asserts that a <see cref="float"/> value is not NaN.
        /// </summary>
        public static Actual<float> ShouldNotBeNaN(this float actual, string message = null)
        {
            return actual.RegisterAssertion(c =>
                {
                    if (float.IsNaN(actual))
                        throw c.StandardError.AreEqual(float.NaN, actual, message);
                });
        }

        /// <summary>
        /// Asserts that a <see cref="double"/> value is NaN.
        /// </summary>
        public static void ShouldBeNaN(this double actual, string message = null)
        {
            actual.RegisterAssertion(c =>
                {
                    if (!double.IsNaN(actual))
                        throw c.StandardError.NotEqual(double.NaN, actual, message);
                });
        }

        /// <summary>
        /// Asserts that a <see cref="double"/> value is not NaN.
        /// </summary>
        public static Actual<double> ShouldNotBeNaN(this double actual, string message = null)
        {
            return actual.RegisterAssertion(c =>
                {
                    if (double.IsNaN(actual))
                        throw c.StandardError.AreEqual(double.NaN, actual, message);
                });
        }
    }
}
