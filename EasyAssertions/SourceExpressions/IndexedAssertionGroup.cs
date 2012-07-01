namespace EasyAssertions
{
    internal class IndexedAssertionGroup : NestedAssertionGroup
    {
        private readonly int index;

        public IndexedAssertionGroup(SourceAddress callAddress, string expressionAlias, int index)
            : base(callAddress, expressionAlias)
        {
            this.index = index;
        }

        public override string GetExpression(string parentExpression)
        {
            return base.GetExpression(parentExpression + '[' + index + ']');
        }
    }
}