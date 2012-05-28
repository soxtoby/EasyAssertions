using System;

namespace EasyAssertions
{
    internal class ContinuationGroup : AssertionComponentGroup
    {
        private readonly string expressionAlias;

        public ContinuationGroup(SourceAddress sourceAddress, string methodName, string expressionAlias)
            : base(sourceAddress, methodName)
        {
            this.expressionAlias = expressionAlias;
        }

        public override string GetExpression()
        {
            string expression = base.GetExpression();
            return expression.Replace(expressionAlias, ParentGroup.GetExpression());
        }

        public override ExpressionSegment GetSegment(string expressionSource, int fromIndex)
        {
            int endOfMethodName = GetMethodCallIndex(expressionSource, fromIndex);
            return new ExpressionSegment
                {
                    Expression = String.Empty,
                    IndexOfNextSegment = new BraceMatcher(expressionSource).MatchFrom(endOfMethodName)
                };
        }
    }
}