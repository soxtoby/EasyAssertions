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

        public override string GetActualExpression(string parentExpression)
        {
            return base.GetActualExpression(parentExpression + '[' + index + ']');
        }
    }
}