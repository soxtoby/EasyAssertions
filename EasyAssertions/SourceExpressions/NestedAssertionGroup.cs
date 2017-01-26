using System.Text.RegularExpressions;

namespace EasyAssertions
{
    internal class NestedAssertionGroup : AssertionComponentGroup
    {
        private const string WordBoundary = @"\b";
        private readonly string expressionAlias;

        public NestedAssertionGroup(SourceAddress callAddress, string expressionAlias)
        {
            Address = callAddress;
            this.expressionAlias = expressionAlias;
        }

        public override SourceAddress Address { get; }

        public override string GetActualExpression(string parentExpression)
        {
            return ReplaceAliasWithActualExpression(base.GetActualExpression(parentExpression), parentExpression)
                ?? parentExpression;
        }

        public override string GetExpectedExpression(string actualExpression)
        {
            string expectedExpression = base.GetExpectedExpression(actualExpression);
            return ReplaceAliasWithActualExpression(expectedExpression, actualExpression)
                ?? expectedExpression;
        }

        private string ReplaceAliasWithActualExpression(string expression, string actualExpression)
        {
            return string.IsNullOrEmpty(expression) || !ExpressionAliasPattern.IsMatch(expression)
                ? null
                : ExpressionAliasPattern.Replace(expression, actualExpression);
        }

        private Regex ExpressionAliasPattern => new Regex(WordBoundary + Regex.Escape(expressionAlias) + WordBoundary);
    }
}