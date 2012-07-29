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
            return actual.RegisterAssert(() =>
                {
                    if (!actual.Contains(expectedToContain))
                        throw EasyAssertions.Failure(FailureMessageFormatter.Current.DoesNotContain(expectedToContain, actual, message));
                });
        }

        /// <summary>
        /// Asserts that a string does not contain a specified substring.
        /// </summary>
        public static Actual<string> ShouldNotContain(this string actual, string expectedToNotContain, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    if (actual.Contains(expectedToNotContain))
                        throw EasyAssertions.Failure(FailureMessageFormatter.Current.Contains(expectedToNotContain, actual, message));
                });
        }

        /// <summary>
        /// Asserts that a string begins with a specified substring.
        /// </summary>
        public static Actual<string> ShouldStartWith(this string actual, string expectedStart, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    if (!actual.StartsWith(expectedStart))
                        throw EasyAssertions.Failure(FailureMessageFormatter.Current.DoesNotStartWith(expectedStart, actual, message));
                });
        }

        /// <summary>
        /// Asserts that a string ends with a specified substring.
        /// </summary>
        public static Actual<string> ShouldEndWith(this string actual, string expectedEnd, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    if (!actual.EndsWith(expectedEnd))
                        throw EasyAssertions.Failure(FailureMessageFormatter.Current.DoesNotEndWith(expectedEnd, actual, message));
                });
        }
    }
}