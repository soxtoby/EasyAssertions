using System.Text.RegularExpressions;

namespace EasyAssertions;

class AssertionStatement : AssertionFrame
{
    const string WordBoundary = @"\b";
    readonly string actualSuffix;
    readonly string expectedSuffix;
    readonly IReadOnlyList<AssertionMethod> assertions;
    readonly Lazy<IEnumerable<AssertionSource>> sources;

    public AssertionStatement(AssertionMethod firstAssertion, AssertionFrame? outerFrame, string actualSuffix, string expectedSuffix)
        : this(new[] { firstAssertion }, outerFrame, actualSuffix, expectedSuffix) { }

    AssertionStatement(IReadOnlyList<AssertionMethod> assertions, AssertionFrame? outerFrame, string actualSuffix, string expectedSuffix)
        : base(outerFrame)
    {
        this.assertions = assertions;
        this.actualSuffix = actualSuffix;
        this.expectedSuffix = expectedSuffix;
        sources = new Lazy<IEnumerable<AssertionSource>>(ParseSource);
    }

    public override AssertionFrame? ChainAssertion(AssertionCall assertion)
    {
        if (assertion is not AssertionMethod assertionMethod)
            return null;

        if (!assertionMethod.CallAddress.Equals(Address))
            return null;

        if (assertionMethod.CallOffset == Offset)
            return null; // Must be running statement in a loop - this counts as a new statement

        return new AssertionStatement(assertions.Append(assertionMethod).ToList(), OuterFrame, actualSuffix, expectedSuffix);
    }

    public override string GetActualExpression()
    {
        var expression = sources.Value
            .Select(s => s.ActualExpressionSegment)
            .Join(string.Empty);

        if (ExpressionContainsAlias(expression, OuterActualAlias))
            expression = ReplaceAliasWithExpression(expression, OuterActualAlias, OuterFrame!.GetActualExpression());
        else if (OuterFrame is not null)
            expression = OuterFrame.GetActualExpression();

        return expression + actualSuffix;
    }

    public override string GetExpectedExpression()
    {
        var expression = sources.Value.LastOrDefault()?.ExpectedExpression ?? string.Empty;

        if (ExpressionContainsAlias(expression, OuterActualAlias))
            expression = ReplaceAliasWithExpression(expression, OuterActualAlias, OuterFrame!.GetActualExpression());

        if (ExpressionContainsAlias(expression, OuterExpectedAlias))
            expression = ReplaceAliasWithExpression(expression, OuterExpectedAlias, OuterFrame!.GetExpectedExpression());

        return expression + expectedSuffix;
    }

    IEnumerable<AssertionSource> ParseSource()
    {
        var sourceCalls = new List<AssertionSource>();

        if (Utils.TryReadSource(Address, out var remainingSource))
        {
            foreach (var call in assertions)
            {
                var callIndex = remainingSource.FindCode($".{call.AssertionName}");
                if (callIndex >= 0)
                {
                    var actualSegment = remainingSource[..callIndex].Trim();
                    remainingSource = remainingSource[callIndex..];

                    if (call.IsProperty)
                    {
                        remainingSource = remainingSource[(1 + call.AssertionName.Length)..]; // +1 for '.'
                        sourceCalls.Add(new AssertionSource(call, actualSegment.ToString(), Array.Empty<string>()));
                    }
                    else
                    {
                        var openingParen = remainingSource.FindCode("(");
                        var closingParen = remainingSource.FindCode(")", openingParen + 1);

                        var argSource = remainingSource[(openingParen + 1)..closingParen];
                        var arguments = argSource.SplitCode(",");

                        remainingSource = remainingSource[(closingParen + 1)..];

                        sourceCalls.Add(new AssertionSource(call, actualSegment.ToString(), arguments));
                    }
                }
                else
                {
                    sourceCalls.Add(new AssertionSource(call, string.Empty, Array.Empty<string>()));
                }
            }
        }

        return sourceCalls;
    }

    static bool ExpressionContainsAlias(string expression, string alias)
    {
        return alias != string.Empty
            && ExpressionAliasPattern(alias).IsMatch(expression);
    }

    static string ReplaceAliasWithExpression(string expressionWithAlias, string alias, string outerExpression)
    {
        return ExpressionAliasPattern(alias).Replace(expressionWithAlias, outerExpression);
    }

    protected override AssertionCall LastCall => assertions.Last();

    SourceAddress Address => assertions.First().CallAddress;
    int Offset => assertions.First().CallOffset;

    static Regex ExpressionAliasPattern(string alias) => new(WordBoundary + Regex.Escape(alias) + WordBoundary);

    public override string ToString() => sources.Value.Join(string.Empty);

    record AssertionSource(AssertionMethod Call, string ActualExpressionSegment, IReadOnlyCollection<string> Arguments)
    {
        public string ExpectedExpression => Arguments.FirstOrDefault() ?? string.Empty;
        public override string ToString() => $"{ActualExpressionSegment}.{Call.AssertionName}{(Call.IsProperty ? string.Empty : $"({Arguments.Join(", ")})")}";
    }
}