namespace EasyAssertions
{
    internal class AssertionMethod : AssertionComponent
    {
        public AssertionMethod(SourceAddress sourceAddress, string methodName) : base(sourceAddress, methodName) { }

        public override ExpressionSegment GetSegment(string expressionSource, int fromIndex)
        {
            int assertionIndex = GetMethodCallIndex(expressionSource, fromIndex);
            return new ExpressionSegment
                {
                    Expression = expressionSource.Substring(fromIndex, assertionIndex - fromIndex),
                    IndexOfNextSegment = new BraceMatcher(expressionSource).MatchFrom(assertionIndex) + 1
                };
        }
    }
}
