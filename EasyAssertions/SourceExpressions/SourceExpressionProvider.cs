using System.Linq.Expressions;

namespace EasyAssertions;

class SourceExpressionProvider : ITestExpressionProvider
{
    /// <summary>
    /// Flows across awaits and into tasks started by the current flow.
    /// Each flow replaces the stack rather than changing it, so concurrent flows don't see each other's assertions.
    /// </summary>
    static readonly AsyncLocal<AssertionStack?> LocalStack = new();

    public static SourceExpressionProvider Instance { get; } = new();

    SourceExpressionProvider() { }

    public void RunAssertion(Action<IAssertionContext> assert, string actualSuffix = "", string expectedSuffix = "") =>
        TrackAssertion(assert, StackAnalyser.MethodCall(2), actualSuffix, expectedSuffix);

    public void InvokeAssertion(Expression<Action> callAssertionMethod, string actualSuffix = "", string expectedSuffix = "") =>
        TrackAssertion(c => callAssertionMethod.Compile()(), new AssertionInvocation(callAssertionMethod), actualSuffix, expectedSuffix);

    static void TrackAssertion(Action<IAssertionContext> assert, AssertionCall assertion, string actualSuffix, string expectedSuffix)
    {
        Stack = Stack.Enter(assertion, actualSuffix, expectedSuffix);

        try
        {
            assert(new AssertionContext());
            Stack = Stack.Exit(true);
        }
        catch
        {
            Stack = Stack.Exit(false);
            throw;
        }
    }

    public string GetActualExpression() => NormalizeIndentation(Stack.LastFrame?.GetActualExpression() ?? string.Empty);
    public string GetExpectedExpression() => NormalizeIndentation(Stack.LastFrame?.GetExpectedExpression() ?? string.Empty);

    static string NormalizeIndentation(string input)
    {
        var lines = input.Split('\n');

        var minIndent = lines
            .Skip(1)
            .Select(l => l.Length - l.TrimStart(' ').Length)
            .Aggregate(int.MaxValue, Math.Min);

        return minIndent <= 4
            ? input
            : lines.Take(1)
                .Concat(lines
                    .Skip(1)
                    .Select(l => l.Substring(minIndent - 4)))
                .Join("\n");
    }

    static AssertionStack Stack
    {
        get => LocalStack.Value ?? AssertionStack.Empty;
        set => LocalStack.Value = value;
    }
}
