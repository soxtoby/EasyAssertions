namespace EasyAssertions
{
    /// <summary>
    /// Provides access to the source representations of the actual and expected values of the currently executing assertion.
    /// </summary>
    public static class TestExpression
    {
        static TestExpressionProvider currentProvider;

        private static TestExpressionProvider CurrentProvider { get { return (currentProvider ?? SourceExpressionProvider.ForCurrentThread); } }

        /// <summary>
        /// Builds the source representation of the value being asserted on.
        /// Assumes that all assertions are extension methods.
        /// </summary>
        public static string GetActual()
        {
            return CurrentProvider.GetActualExpression();
        }

        /// <summary>
        /// Builds the source representation of the value being compared against the actual value.
        /// Assumes that all assertions take the expected value as the first parameter after the actual value.
        /// </summary>
        public static string GetExpected()
        {
            return CurrentProvider.GetExpectedExpression();
        }

        /// <summary>
        /// Overrides the <see cref="TestExpressionProvider"/> used to provide source representations
        /// of the current assertion's actual and expected values.
        /// </summary>
        public static void OverrideProvider(TestExpressionProvider provider)
        {
            currentProvider = provider;
        }

        /// <summary>
        /// Resets the current <see cref="TestExpressionProvider"/> to the default provider.
        /// </summary>
        public static void DefaultProvider()
        {
            currentProvider = null;
        }
    }

    /// <summary>
    /// Provides source representations for the current assertion's actual and expected values.
    /// </summary>
    public interface TestExpressionProvider
    {
        string GetActualExpression();
        string GetExpectedExpression();
    }
}