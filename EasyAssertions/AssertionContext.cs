namespace EasyAssertions
{
    /// <summary>
    /// Helpers for creating custom assertions that behave consistently.
    /// </summary>
    public interface IAssertionContext
    {
        /// <summary>
        /// Provides a set of standard testing functions for implementing assertion logic.
        /// </summary>
        StandardTests Test { get; }
        /// <summary>
        /// The standard set of assertion failure messages.
        /// </summary>
        IStandardErrors StandardError { get; }
        /// <summary>
        /// Provides factory methods for creating custom assertion failure messages.
        /// </summary>
        IErrorFactory Error { get; }
    }

    class AssertionContext : IAssertionContext
    {
        public StandardTests Test => StandardTests.Instance;
        public IStandardErrors StandardError => StandardErrors.Current;
        public IErrorFactory Error => ErrorFactory.Instance;
    }
}