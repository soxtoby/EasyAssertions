using System.Reflection;

namespace EasyAssertions;

abstract class AssertionCall
{
    protected AssertionCall(MethodBase assertionMethod) => AssertionMethod = assertionMethod;

    protected MethodBase AssertionMethod { get; }
    public string ActualAlias => AssertionMethod.GetParameters().ElementAtOrDefault(0)?.Name ?? string.Empty;
    public string ExpectedAlias => AssertionMethod.GetParameters().ElementAtOrDefault(1)?.Name ?? string.Empty;

    public abstract AssertionFrame CreateFrame(AssertionFrame? outerFrame, string actualSuffix, string expectedSuffix);
}