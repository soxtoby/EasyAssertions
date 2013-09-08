using System;

namespace EasyAssertions
{
    /// <summary>
    /// String-related assertions.
    /// </summary>
    public static class StringAssertions
    {
        /// <summary>
        /// Asserts that a string contains a specified substring.
        /// </summary>
        public static Actual<string> ShouldContain(this string actual, string expectedToContain, string message = null)
        {
            if (expectedToContain == null) throw new ArgumentNullException("expectedToContain");

            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<string>(actual, message);

                    if (!actual.Contains(expectedToContain))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoesNotContain(expectedToContain, actual, message));
                });
        }

        /// <summary>
        /// Asserts that a string does not contain a specified substring.
        /// </summary>
        public static Actual<string> ShouldNotContain(this string actual, string expectedToNotContain, string message = null)
        {
            if (expectedToNotContain == null) throw new ArgumentNullException("expectedToNotContain");

            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<string>(actual, message);

                    if (actual.Contains(expectedToNotContain))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.Contains(expectedToNotContain, actual, message));
                });
        }

        /// <summary>
        /// Asserts that a string begins with a specified substring.
        /// </summary>
        public static Actual<string> ShouldStartWith(this string actual, string expectedStart, string message = null)
        {
            if (expectedStart == null) throw new ArgumentNullException("expectedStart");

            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<string>(actual, message);

                    if (!actual.StartsWith(expectedStart))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoesNotStartWith(expectedStart, actual, message));
                });
        }

        /// <summary>
        /// Asserts that a string ends with a specified substring.
        /// </summary>
        public static Actual<string> ShouldEndWith(this string actual, string expectedEnd, string message = null)
        {
            if (expectedEnd == null) throw new ArgumentNullException("expectedEnd");

            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<string>(actual, message);

                    if (!actual.EndsWith(expectedEnd))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoesNotEndWith(expectedEnd, actual, message));
                });
        }

        /// <summary>
        /// Asserts that two strings are equal, optionally ignoring case.
        /// </summary>
        public static Actual<string> ShouldBe(this string actual, string expected, Case caseSensitivity, string message = null)
        {
            if (expected == null) throw new ArgumentNullException("expected");

            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<string>(actual, message);

                    StringComparison stringComparison = caseSensitivity == Case.Sensitive
                        ? StringComparison.Ordinal
                        : StringComparison.OrdinalIgnoreCase;

                    if (!actual.Equals(expected, stringComparison))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.NotEqual(expected, actual, caseSensitivity, message));
                });
        }
    }
}