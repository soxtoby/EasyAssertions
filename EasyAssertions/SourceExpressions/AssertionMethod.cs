namespace EasyAssertions
{
    internal class AssertionMethod : AssertionComponent
    {
        public AssertionMethod(SourceAddress sourceAddress, string methodName) : base(sourceAddress, methodName) { }

        public override ExpressionSegment GetActualSegment(string expressionSource, int fromIndex)
        {
            int assertionIndex = GetMethodCallIndex(expressionSource, fromIndex);
            if (assertionIndex == -1)
                return new ExpressionSegment { IndexOfNextSegment = fromIndex };

            return new ExpressionSegment
                {
                    Expression = expressionSource.Substring(fromIndex, assertionIndex - fromIndex),
                    IndexOfNextSegment = AfterClosingParen(expressionSource, assertionIndex)
                };
        }

        public override ExpressionSegment GetExpectedSegment(string expressionSource, int fromIndex)
        {
            int assertionIndex = GetMethodCallIndex(expressionSource, fromIndex);
            if (assertionIndex == -1)
                return new ExpressionSegment { IndexOfNextSegment = fromIndex };

            int startOfFirstParam = AfterOpeningParen(expressionSource, assertionIndex);
            int endOfFirstParam = BeforeNextComma(expressionSource, startOfFirstParam);
            int endOfMethodCall = AfterClosingParen(expressionSource, assertionIndex);

            if (EndOfFirstParamIsInvalid(endOfFirstParam, endOfMethodCall))
                endOfFirstParam = BeforeClosingParen(endOfMethodCall);

            return new ExpressionSegment
                {
                    Expression = expressionSource.Substring(startOfFirstParam, endOfFirstParam - startOfFirstParam),
                    IndexOfNextSegment = endOfMethodCall
                };
        }

        private static int AfterOpeningParen(string expressionSource, int assertionIndex)
        {
            return expressionSource.IndexOf('(', assertionIndex) + 1;
        }

        private static int BeforeNextComma(string expressionSource, int startOfFirstParam)
        {
            return BraceMatcher.FindNext(expressionSource, ',', startOfFirstParam);
        }

        private static int AfterClosingParen(string expressionSource, int assertionIndex)
        {
            return BraceMatcher.FindClosingBrace(expressionSource, assertionIndex) + 1;
        }

        private static bool EndOfFirstParamIsInvalid(int endOfFirstParam, int endOfMethodCall)
        {
            return endOfFirstParam < 0 || endOfFirstParam > endOfMethodCall;
        }

        private static int BeforeClosingParen(int endOfMethodCall)
        {
            return endOfMethodCall - 1;
        }
    }
}