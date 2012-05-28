using System;

namespace EasyAssertions
{
    internal class BaseGroup : AssertionComponentGroup
    {
        public BaseGroup(SourceAddress sourceAddress, string methodName) : base(sourceAddress, methodName) { }

        public override ExpressionSegment GetSegment(string expressionSource, int fromIndex)
        {
            throw new InvalidOperationException("Shouldn't need to call this.");
        }
    }
}