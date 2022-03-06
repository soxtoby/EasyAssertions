namespace EasyAssertions;

class InvocationFrame : AssertionFrame
{
    readonly AssertionInvocation invocation;
    readonly string actualSuffix;
    readonly string expectedSuffix;

    public InvocationFrame(AssertionInvocation invocation, AssertionFrame? outerFrame, string actualSuffix, string expectedSuffix)
        : base(outerFrame)
    {
        this.invocation = invocation;
        this.actualSuffix = actualSuffix;
        this.expectedSuffix = expectedSuffix;
    }

    protected override AssertionCall LastCall => invocation;
    public override bool TryChainAssertion(AssertionCall _) => false;
    public override string GetActualExpression() => (OuterFrame?.GetActualExpression() ?? string.Empty) + actualSuffix;
    public override string GetExpectedExpression() => (OuterFrame?.GetExpectedExpression() ?? string.Empty) + expectedSuffix;
    public override string ToString() => invocation.ToString();
}