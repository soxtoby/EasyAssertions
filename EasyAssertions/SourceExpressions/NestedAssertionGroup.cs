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
            string expression = base.GetActualExpression(parentExpression);
            
            return string.IsNullOrEmpty(expression) || !ExpressionAliasPattern.IsMatch(expression)
                ? parentExpression
                : ExpressionAliasPattern.Replace(expression, parentExpression);
        }

        private Regex ExpressionAliasPattern => new Regex(WordBoundary + Regex.Escape(expressionAlias) + WordBoundary);
    }
}