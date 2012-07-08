using System;

namespace EasyAssertions
{
    internal class Continuation : AssertionComponent
    {
        public Continuation(SourceAddress sourceAddress, string methodName) : base(sourceAddress, methodName) { }

        public override ExpressionSegment GetActualSegment(string expressionSource, int fromIndex)
        {
            return GetSegment(expressionSource, fromIndex);
        }

        public override ExpressionSegment GetExpectedSegment(string expressionSource, int fromIndex)
        {
            return GetSegment(expressionSource, fromIndex);
        }

        private ExpressionSegment GetSegment(string expressionSource, int fromIndex)
        {
            int endOfContinuationProperty = GetMethodCallIndex(expressionSource, fromIndex) + MethodName.Length + 1;
            return new ExpressionSegment
                {
                    Expression = String.Empty,
                    IndexOfNextSegment = endOfContinuationProperty
                };
        }
    }
}