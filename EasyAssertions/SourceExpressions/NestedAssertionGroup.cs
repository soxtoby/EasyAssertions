namespace EasyAssertions
{
    internal class NestedAssertionGroup : AssertionComponentGroup
    {
        private readonly string expressionAlias;

        public NestedAssertionGroup(AssertionComponentGroup parentGroup, string expressionAlias)
            : base(parentGroup)
        {
            this.expressionAlias = expressionAlias;
        }

        public override string GetExpression()
        {
            string expression = base.GetExpression();
            return expression.Replace(expressionAlias, GetParentExpression());
        }

        protected virtual string GetParentExpression()
        {
            return ParentGroup.GetExpression();
        }
    }
}