namespace EasyAssertions
{
    class ExpressionSegment
    {
        public string Expression;
        public int IndexOfNextSegment;

        public ExpressionSegment()
        {
            Expression = string.Empty;
        }
    }
}