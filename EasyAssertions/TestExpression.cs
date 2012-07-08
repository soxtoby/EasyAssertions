namespace EasyAssertions
{
    public static class TestExpression
    {
        static TestExpressionProvider currentProvider;

        private static TestExpressionProvider CurrentProvider { get { return (currentProvider ?? SourceExpressionProvider.Instance); } }

        public static string GetActual()
        {
            return CurrentProvider.GetActualExpression();
        }

        public static string GetExpected()
        {
            return CurrentProvider.GetExpectedExpression();
        }

        public static void OverrideProvider(TestExpressionProvider provider)
        {
            currentProvider = provider;
        }

        public static void DefaultProvider()
        {
            currentProvider = null;
        }
    }

    public interface TestExpressionProvider
    {
        string GetActualExpression();
        string GetExpectedExpression();
    }
}