namespace EasyAssertions
{
    /// <summary>
    /// Helpers for creating custom assertions that behave consistently.
    /// </summary>
    public interface IAssertionContext
    {
        StandardTests Test { get; }
        IStandardErrors StandardError { get; }
        IErrorFactory Error { get; }
    }

    class AssertionContext : IAssertionContext
    {
        public StandardTests Test => StandardTests.Instance;
        public IStandardErrors StandardError => StandardErrors.Current;
        public IErrorFactory Error => ErrorFactory.Instance;
    }
}