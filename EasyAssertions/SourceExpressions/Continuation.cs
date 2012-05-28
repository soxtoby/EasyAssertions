using System;

namespace EasyAssertions
{
    internal class Continuation : AssertionComponent
    {
        public Continuation(SourceAddress sourceAddress, string methodName) : base(sourceAddress, methodName) { }

        public override ExpressionSegment GetSegment(string expressionSource, int fromIndex)
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