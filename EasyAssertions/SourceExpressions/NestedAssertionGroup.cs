using System.Text.RegularExpressions;

namespace EasyAssertions
{
    internal class NestedAssertionGroup : AssertionComponentGroup
    {
        private const string WordBoundary = @"\b";
        private readonly SourceAddress address;
        private readonly string expressionAlias;

        public NestedAssertionGroup(SourceAddress callAddress, string expressionAlias)
        {
            address = callAddress;
            this.expressionAlias = expressionAlias;
        }

        public override SourceAddress Address { get { return address; } }

        public override string GetActualExpression(string parentExpression)
        {
            string expression = base.GetActualExpression(parentExpression);
            return Regex.Replace(expression, WordBoundary + expressionAlias + WordBoundary, parentExpression);
        }
    }
}