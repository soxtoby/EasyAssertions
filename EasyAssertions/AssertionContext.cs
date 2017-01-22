namespace EasyAssertions
{
    public class AssertionContext
    {
        public StandardTests Test => StandardTests.Instance;
        public IStandardErrors StandardError => StandardErrors.Current;
        public IErrorFactory Error => ErrorFactory.Instance;
    }
}