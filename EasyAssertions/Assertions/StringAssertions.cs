using System;
using System.Text.RegularExpressions;

namespace EasyAssertions
{
    /// <summary>
    /// String-related assertions.
    /// </summary>
    public static class StringAssertions
    {
        /// <summary>
        /// Asserts that a string is empty.
        /// </summary>
        public static Actual<string> ShouldBeEmpty(this string actual, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<string>(actual, message);

                    if (actual != string.Empty)
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.NotEmpty(actual, message));
                });
        }

        /// <summary>
        /// Asserts that a string contains a specified substring.
        /// </summary>
        public static Actual<string> ShouldContain(this string actual, string expectedToContain, string message = null)
        {
            if (expectedToContain == null) throw new ArgumentNullException(nameof(expectedToContain));

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
            if (expectedToNotContain == null) throw new ArgumentNullException(nameof(expectedToNotContain));

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
            if (expectedStart == null) throw new ArgumentNullException(nameof(expectedStart));

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
            if (expectedEnd == null) throw new ArgumentNullException(nameof(expectedEnd));

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
            if (expected == null) throw new ArgumentNullException(nameof(expected));

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

        /// <summary>
        /// Asserts that a string matches the specified regex pattern, using <see cref="RegexOptions.None"/>.
        /// </summary>
        public static Actual<string> ShouldMatch(this string actual, string regexPattern, string message = null)
        {
            if (regexPattern == null) throw new ArgumentNullException(nameof(regexPattern));

            return actual.RegisterAssert(() => AssertMatch(actual, new Regex(regexPattern), message));
        }

        /// <summary>
        /// Asserts that a string matches the specified regex pattern, using the provided <see cref="RegexOptions"/>
        /// </summary>
        public static Actual<string> ShouldMatch(this string actual, string regexPattern, RegexOptions options, string message = null)
        {
            if (regexPattern == null) throw new ArgumentNullException(nameof(regexPattern));

            return actual.RegisterAssert(() => AssertMatch(actual, new Regex(regexPattern, options), message));
        }

        /// <summary>
        /// Asserts that a string matches the specified <see cref="Regex"/>.
        /// </summary>
        public static Actual<string> ShouldMatch(this string actual, Regex regex, string message = null)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            return actual.RegisterAssert(() => AssertMatch(actual, regex, message));
        }

        private static void AssertMatch(string actual, Regex regex, string message)
        {
            ObjectAssertions.AssertType<string>(actual, message);

            if (!regex.IsMatch(actual))
                throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoesNotMatch(regex, actual, message));
        }

        /// <summary>
        /// Asserts that a string does not match the specified regex pattern, using <see cref="RegexOptions.None"/>.
        /// </summary>
        public static Actual<string> ShouldNotMatch(this string actual, string regexPattern, string message = null)
        {
            if (regexPattern == null) throw new ArgumentNullException(nameof(regexPattern));

            return actual.RegisterAssert(() => AssertDoesNotMatch(actual, new Regex(regexPattern), message));
        }

        /// <summary>
        /// Asserts that a string does not match the specified regex pattern, using the provides <see cref="RegexOptions"/>.
        /// </summary>
        public static Actual<string> ShouldNotMatch(this string actual, string regexPattern, RegexOptions options, string message = null)
        {
            if (regexPattern == null) throw new ArgumentNullException(nameof(regexPattern));

            return actual.RegisterAssert(() => AssertDoesNotMatch(actual, new Regex(regexPattern, options), message));
        }

        /// <summary>
        /// Asserts that a string does not match the specified <see cref="Regex"/>.
        /// </summary>
        public static Actual<string> ShouldNotMatch(this string actual, Regex regex, string message = null)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            return actual.RegisterAssert(() => AssertDoesNotMatch(actual, regex, message));
        }

        private static void AssertDoesNotMatch(string actual, Regex regex, string message)
        {
            ObjectAssertions.AssertType<string>(actual, message);

            if (regex.IsMatch(actual))
                throw EasyAssertion.Failure(FailureMessageFormatter.Current.Matches(regex, actual, message));
        }
    }
}