namespace EasyAssertions
{
    class AssertionMethod : AssertionComponent
    {
        public AssertionMethod(SourceAddress sourceAddress, string methodName) : base(sourceAddress, methodName) { }

        public override ExpressionSegment GetActualSegment(string expressionSource, int fromIndex)
        {
            var assertionIndex = GetMethodCallIndex(expressionSource, fromIndex);
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
            var assertionIndex = GetMethodCallIndex(expressionSource, fromIndex);
            if (assertionIndex == -1)
                return new ExpressionSegment { IndexOfNextSegment = fromIndex };

            var startOfFirstParam = AfterOpeningParen(expressionSource, assertionIndex);
            var endOfFirstParam = BeforeNextComma(expressionSource, startOfFirstParam);
            var endOfMethodCall = AfterClosingParen(expressionSource, assertionIndex);

            if (EndOfFirstParamIsInvalid(endOfFirstParam, endOfMethodCall))
                endOfFirstParam = BeforeClosingParen(endOfMethodCall);

            return new ExpressionSegment
                {
                    Expression = expressionSource.Substring(startOfFirstParam, endOfFirstParam - startOfFirstParam),
                    IndexOfNextSegment = endOfMethodCall
                };
        }

        static int AfterOpeningParen(string expressionSource, int assertionIndex)
        {
            return expressionSource.IndexOf('(', assertionIndex) + 1;
        }

        static int BeforeNextComma(string expressionSource, int startOfFirstParam)
        {
            return BraceMatcher.FindNext(expressionSource, ',', startOfFirstParam);
        }

        static int AfterClosingParen(string expressionSource, int assertionIndex)
        {
            return BraceMatcher.FindClosingBrace(expressionSource, assertionIndex) + 1;
        }

        static bool EndOfFirstParamIsInvalid(int endOfFirstParam, int endOfMethodCall)
        {
            return endOfFirstParam < 0 || endOfFirstParam > endOfMethodCall;
        }

        static int BeforeClosingParen(int endOfMethodCall)
        {
            return endOfMethodCall - 1;
        }
    }
}