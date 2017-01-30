using System.Text.RegularExpressions;

namespace EasyAssertions
{
    internal class NestedAssertionGroup : AssertionComponentGroup
    {
        private const string WordBoundary = @"\b";
        private readonly string actualAlias;
        private readonly string expectedAlias;

        public NestedAssertionGroup(SourceAddress callAddress, string actualAlias, string expectedAlias)
        {
            Address = callAddress;
            this.actualAlias = actualAlias;
            this.expectedAlias = expectedAlias;
        }

        public override SourceAddress Address { get; }

        public override string GetActualExpression(string parentExpression)
        {
            string aliasedExpression = base.GetActualExpression(parentExpression);
            string resolvedExpression = ReplaceAliasWithExpression(aliasedExpression, actualAlias, parentExpression);

            return resolvedExpression == aliasedExpression // If actual alias wasn't used explicitly, just use the expression we have so far
                ? parentExpression
                : resolvedExpression;
        }

        public override string GetExpectedExpression(string actualExpression, string parentExpression)
        {
            string expectedExpression = base.GetExpectedExpression(actualExpression, parentExpression);
            return ReplaceAliasWithExpression(ReplaceAliasWithExpression(expectedExpression, actualAlias, actualExpression), expectedAlias, parentExpression);
        }

        private static string ReplaceAliasWithExpression(string expressionWithAlias, string alias, string parentExpression)
        {
            return string.IsNullOrEmpty(alias)
                ? expressionWithAlias
                : ExpressionAliasPattern(alias).Replace(expressionWithAlias, parentExpression);
        }

        private static Regex ExpressionAliasPattern(string alias) => new Regex(WordBoundary + Regex.Escape(alias) + WordBoundary);
    }
}