namespace EasyAssertions
{
    internal class IndexedAssertionGroup : NestedAssertionGroup
    {
        private readonly int index;

        public IndexedAssertionGroup(SourceAddress callAddress, string actualAlias, string expectedAlias, int index)
            : base(callAddress, actualAlias, expectedAlias)
        {
            this.index = index;
        }

        public override string GetActualExpression(string parentExpression)
        {
            return base.GetActualExpression(parentExpression + '[' + index + ']');
        }
    }
}