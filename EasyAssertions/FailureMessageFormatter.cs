namespace EasyAssertions
{
    /// <summary>
    /// Provides access to a builder for the standard set of assertion failure messages.
    /// </summary>
    public static class FailureMessageFormatter
    {
        private static IFailureMessageFormatter current;

        /// <summary>
        /// The current failure message formatter.
        /// Can be overridden with <see cref="Override"/>.
        /// </summary>
        public static IFailureMessageFormatter Current => current ?? DefaultFailureMessageFormatter.Instance;

        /// <summary>
        /// Overrides the message formatter used to provide the standard set of assertion failure messages.
        /// </summary>
        public static void Override(IFailureMessageFormatter newFormatter)
        {
            current = newFormatter;
        }

        /// <summary>
        /// Resets the current failure message formatter to the formatter.
        /// </summary>
        public static void Default()
        {
            current = null;
        }
    }
}