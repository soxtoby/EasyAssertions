using System.Text.RegularExpressions;

namespace EasyAssertions
{
    class NestedAssertionGroup : AssertionComponentGroup
    {
        const string WordBoundary = @"\b";
        readonly string actualAlias;
        readonly string expectedAlias;

        public NestedAssertionGroup(SourceAddress callAddress, string actualAlias, string expectedAlias)
        {
            Address = callAddress;
            this.actualAlias = actualAlias;
            this.expectedAlias = expectedAlias;
        }

        public override SourceAddress Address { get; }

        public override string GetActualExpression(string parentExpression)
        {
            var aliasedExpression = base.GetActualExpression(parentExpression);
            var resolvedExpression = ReplaceAliasWithExpression(aliasedExpression, actualAlias, parentExpression);

            return resolvedExpression == aliasedExpression // If actual alias wasn't used explicitly, just use the expression we have so far
                ? parentExpression
                : resolvedExpression;
        }

        public override string GetExpectedExpression(string actualExpression, string parentExpression)
        {
            var expectedExpression = base.GetExpectedExpression(actualExpression, parentExpression);
            return ReplaceAliasWithExpression(ReplaceAliasWithExpression(expectedExpression, actualAlias, actualExpression), expectedAlias, parentExpression);
        }

        static string ReplaceAliasWithExpression(string expressionWithAlias, string alias, string parentExpression)
        {
            return string.IsNullOrEmpty(alias)
                ? expressionWithAlias
                : ExpressionAliasPattern(alias).Replace(expressionWithAlias, parentExpression);
        }

        static Regex ExpressionAliasPattern(string alias) => new Regex(WordBoundary + Regex.Escape(alias) + WordBoundary);
    }
}