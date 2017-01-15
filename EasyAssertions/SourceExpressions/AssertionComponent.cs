using System;

namespace EasyAssertions
{
    internal abstract class AssertionComponent
    {
        public readonly SourceAddress SourceAddress;
        protected readonly string MethodName;

        protected AssertionComponent(SourceAddress sourceAddress, string methodName)
        {
            SourceAddress = sourceAddress;
            MethodName = methodName;
        }

        public abstract ExpressionSegment GetActualSegment(string expressionSource, int fromIndex);

        public abstract ExpressionSegment GetExpectedSegment(string expressionSource, int fromIndex);

        protected int GetMethodCallIndex(string expressionSource, int fromIndex)
        {
            return expressionSource.IndexOf("." + MethodName, fromIndex, StringComparison.Ordinal);
        }
    }
}