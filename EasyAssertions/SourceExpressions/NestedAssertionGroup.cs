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
            
            return string.IsNullOrEmpty(expression)
                ? parentExpression
                : Regex.Replace(expression, WordBoundary + expressionAlias + WordBoundary, parentExpression);
        }
    }
}