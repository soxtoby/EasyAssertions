namespace EasyAssertions
{
    public static class TestExpression
    {
        static TestExpressionProvider currentProvider;

        private static TestExpressionProvider CurrentProvider { get { return (currentProvider ?? SourceExpressionProvider.Instance); } }

        public static string Get()
        {
            return CurrentProvider.GetExpression();
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
        string GetExpression();
    }
}