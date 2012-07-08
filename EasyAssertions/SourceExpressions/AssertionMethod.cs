namespace EasyAssertions
{
    internal class AssertionMethod : AssertionComponent
    {
        public AssertionMethod(SourceAddress sourceAddress, string methodName) : base(sourceAddress, methodName) { }

        public override ExpressionSegment GetActualSegment(string expressionSource, int fromIndex)
        {
            int assertionIndex = GetMethodCallIndex(expressionSource, fromIndex);
            return new ExpressionSegment
                {
                    Expression = expressionSource.Substring(fromIndex, assertionIndex - fromIndex),
                    IndexOfNextSegment = IndexOfNextSegment(expressionSource, assertionIndex)
                };
        }

        public override ExpressionSegment GetExpectedSegment(string expressionSource, int fromIndex)
        {
            int assertionIndex = GetMethodCallIndex(expressionSource, fromIndex);
            int startOfFirstParam = expressionSource.IndexOf('(', assertionIndex) + 1;
            int endOfFirstParam = BraceMatcher.FindNext(expressionSource, ',', startOfFirstParam);
            int endOfMethodCall = IndexOfNextSegment(expressionSource, assertionIndex);

            if (endOfFirstParam < 0 || endOfFirstParam > endOfMethodCall)
                endOfFirstParam = endOfMethodCall - 1;

            return new ExpressionSegment
                {
                    Expression = expressionSource.Substring(startOfFirstParam, endOfFirstParam - startOfFirstParam),
                    IndexOfNextSegment = endOfMethodCall
                };
        }

        private static int IndexOfNextSegment(string expressionSource, int assertionIndex)
        {
            return BraceMatcher.FindClosingBrace(expressionSource, assertionIndex) + 1;
        }
    }
}