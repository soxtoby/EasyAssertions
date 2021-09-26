namespace EasyAssertions
{
    abstract class AssertionFrame
    {
        protected readonly AssertionFrame? OuterFrame;
        protected AssertionFrame(AssertionFrame? outerFrame) => OuterFrame = outerFrame;

        protected string OuterActualAlias =>
            OuterFrame?.InnerActualAlias
            ?? string.Empty;

        string InnerActualAlias =>
            LastCall.ActualAlias.NullIfEmpty()
            ?? OuterActualAlias;

        protected string OuterExpectedAlias =>
            OuterFrame?.InnerExpectedAlias
            ?? string.Empty;

        string InnerExpectedAlias =>
            LastCall.ExpectedAlias.NullIfEmpty()
            ?? OuterExpectedAlias;

        protected abstract AssertionCall LastCall { get; }

        public abstract string GetActualExpression();
        public abstract string GetExpectedExpression();
        public abstract bool TryChainAssertion(AssertionCall assertion);
    }
}