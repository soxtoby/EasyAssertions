using System.Reflection;
using NUnit.Framework;

namespace EasyAssertions.UnitTests;

public class AssertionStatementTests
{
    [Test]
    public void GetExpectedExpression_SourceFileNotFound_IsEmpty()
    {
        var assertion = new AssertionMethod(MethodBase.GetCurrentMethod()!, new SourceAddress("/_/Missing/Tests.cs", 1, 1, null), 0);
        var sut = new AssertionStatement(assertion, null, string.Empty, string.Empty);

        var result = sut.GetExpectedExpression();

        Assert.AreEqual(string.Empty, result);
    }
}