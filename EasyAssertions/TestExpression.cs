namespace EasyAssertions
{
    /// <summary>
    /// Provides access to the source representations of the actual and expected values of the currently executing assertion.
    /// </summary>
    public static class TestExpression
    {
        static ITestExpressionProvider? currentProvider;

        private static ITestExpressionProvider CurrentProvider => currentProvider ?? SourceExpressionProvider.ForCurrentThread;

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
        /// Overrides the <see cref="ITestExpressionProvider"/> used to provide source representations
        /// of the current assertion's actual and expected values.
        /// </summary>
        public static void OverrideProvider(ITestExpressionProvider provider)
        {
            currentProvider = provider;
        }

        /// <summary>
        /// Resets the current <see cref="ITestExpressionProvider"/> to the default provider.
        /// </summary>
        public static void DefaultProvider()
        {
            currentProvider = null;
        }
    }

    /// <summary>
    /// Provides source representations for the current assertion's actual and expected values.
    /// </summary>
    public interface ITestExpressionProvider
    {
        /// <summary>
        /// Builds the source representation of the value being asserted on.
        /// </summary>
        string GetActualExpression();
        /// <summary>
        /// Builds the source representation of the value being compared against the actual value.
        /// </summary>
        string GetExpectedExpression();
    }
}