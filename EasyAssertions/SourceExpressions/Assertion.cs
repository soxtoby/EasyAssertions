namespace EasyAssertions
{
    internal class Assertion : AssertionComponent
    {
        public Assertion(SourceAddress sourceAddress, string methodName) : base(sourceAddress, methodName) { }

        public override ExpressionSegment GetSegment(string expressionSource, int fromIndex)
        {
            int assertionIndex = GetMethodCallIndex(expressionSource, fromIndex);
            return new ExpressionSegment
                {
                    Expression = expressionSource.Substring(fromIndex, assertionIndex - fromIndex),
                    IndexOfNextSegment = fromIndex
                };
        }
    }
}