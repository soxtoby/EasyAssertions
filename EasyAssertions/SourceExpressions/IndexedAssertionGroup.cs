namespace EasyAssertions
{
    internal class IndexedAssertionGroup : NestedAssertionGroup
    {
        private readonly int index;

        public IndexedAssertionGroup(AssertionComponentGroup parentGroup, string expressionAlias, int index)
            : base(parentGroup, expressionAlias)
        {
            this.index = index;
        }

        protected override string GetParentExpression()
        {
            return ParentGroup.GetExpression() + '[' + index + ']';
        }
    }
}