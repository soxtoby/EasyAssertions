using System;
using System.Diagnostics.CodeAnalysis;
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
        public static Actual<string> ShouldBeEmpty([NotNull] this string? actual, string? message = null)
        {
            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<string>(message);

                    if (actual != string.Empty)
                        throw c.StandardError.NotEmpty(actual, message);
                });
        }

        /// <summary>
        /// Asserts that a string contains a specified substring.
        /// </summary>
        public static Actual<string> ShouldContain([NotNull] this string? actual, string expectedToContain, string? message = null)
        {
            if (expectedToContain == null) throw new ArgumentNullException(nameof(expectedToContain));

            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<string>(message);

                    if (!actual.Contains(expectedToContain))
                        throw c.StandardError.DoesNotContain(expectedToContain, actual, message);
                });
        }

        /// <summary>
        /// Asserts that a string does not contain a specified substring.
        /// </summary>
        public static Actual<string> ShouldNotContain([NotNull] this string? actual, string expectedToNotContain, string? message = null)
        {
            if (expectedToNotContain == null) throw new ArgumentNullException(nameof(expectedToNotContain));

            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<string>(message);

                    if (actual.Contains(expectedToNotContain))
                        throw c.StandardError.Contains(expectedToNotContain, actual, message);
                });
        }

        /// <summary>
        /// Asserts that a string begins with a specified substring.
        /// </summary>
        public static Actual<string> ShouldStartWith([NotNull] this string? actual, string expectedStart, string? message = null)
        {
            if (expectedStart == null) throw new ArgumentNullException(nameof(expectedStart));

            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<string>(message);

                    if (!actual.StartsWith(expectedStart))
                        throw c.StandardError.DoesNotStartWith(expectedStart, actual, message);
                });
        }

        /// <summary>
        /// Asserts that a string ends with a specified substring.
        /// </summary>
        public static Actual<string> ShouldEndWith([NotNull] this string? actual, string expectedEnd, string? message = null)
        {
            if (expectedEnd == null) throw new ArgumentNullException(nameof(expectedEnd));

            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<string>(message);

                    if (!actual.EndsWith(expectedEnd))
                        throw c.StandardError.DoesNotEndWith(expectedEnd, actual, message);
                });
        }

        /// <summary>
        /// Asserts that two strings are equal, optionally ignoring case.
        /// </summary>
        public static Actual<string> ShouldBe([NotNull] this string? actual, string expected, Case caseSensitivity, string? message = null)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));

            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<string>(message);

                    var stringComparison = caseSensitivity == Case.Sensitive
                        ? StringComparison.Ordinal
                        : StringComparison.OrdinalIgnoreCase;

                    if (!actual.Equals(expected, stringComparison))
                        throw c.StandardError.NotEqual(expected, actual, caseSensitivity, message);
                });
        }

        /// <summary>
        /// Asserts that a string matches the specified regex pattern, using <see cref="RegexOptions.None"/>.
        /// </summary>
        public static Actual<string> ShouldMatch([NotNull] this string? actual, string regexPattern, string? message = null)
        {
            if (regexPattern == null) throw new ArgumentNullException(nameof(regexPattern));

            return actual.RegisterNotNullAssertion(c => AssertMatch(actual, new Regex(regexPattern), message, c));
        }

        /// <summary>
        /// Asserts that a string matches the specified regex pattern, using the provided <see cref="RegexOptions"/>
        /// </summary>
        public static Actual<string> ShouldMatch([NotNull] this string? actual, string regexPattern, RegexOptions options, string? message = null)
        {
            if (regexPattern == null) throw new ArgumentNullException(nameof(regexPattern));

            return actual.RegisterNotNullAssertion(c => AssertMatch(actual, new Regex(regexPattern, options), message, c));
        }

        /// <summary>
        /// Asserts that a string matches the specified <see cref="Regex"/>.
        /// </summary>
        public static Actual<string> ShouldMatch([NotNull] this string? actual, Regex regex, string? message = null)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            return actual.RegisterNotNullAssertion(c => AssertMatch(actual, regex, message, c));
        }

        static void AssertMatch([NotNull] string? actual, Regex regex, string? message, IAssertionContext context)
        {
            actual.ShouldBeA<string>(message);

            if (!regex.IsMatch(actual))
                throw context.StandardError.DoesNotMatch(regex, actual, message);
        }

        /// <summary>
        /// Asserts that a string does not match the specified regex pattern, using <see cref="RegexOptions.None"/>.
        /// </summary>
        public static Actual<string> ShouldNotMatch([NotNull] this string? actual, string regexPattern, string? message = null)
        {
            if (regexPattern == null) throw new ArgumentNullException(nameof(regexPattern));

            return actual.RegisterNotNullAssertion(c => AssertDoesNotMatch(actual, new Regex(regexPattern), message, c));
        }

        /// <summary>
        /// Asserts that a string does not match the specified regex pattern, using the provides <see cref="RegexOptions"/>.
        /// </summary>
        public static Actual<string> ShouldNotMatch([NotNull] this string? actual, string regexPattern, RegexOptions options, string? message = null)
        {
            if (regexPattern == null) throw new ArgumentNullException(nameof(regexPattern));

            return actual.RegisterNotNullAssertion(c => AssertDoesNotMatch(actual, new Regex(regexPattern, options), message, c));
        }

        /// <summary>
        /// Asserts that a string does not match the specified <see cref="Regex"/>.
        /// </summary>
        public static Actual<string> ShouldNotMatch([NotNull] this string? actual, Regex regex, string? message = null)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            return actual.RegisterNotNullAssertion(c => AssertDoesNotMatch(actual, regex, message, c));
        }

        static void AssertDoesNotMatch(string? actual, Regex regex, string? message, IAssertionContext context)
        {
            actual.ShouldBeA<string>(message);

            if (regex.IsMatch(actual))
                throw context.StandardError.Matches(regex, actual, message);
        }
    }
}