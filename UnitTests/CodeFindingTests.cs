using NUnit.Framework;

namespace EasyAssertions.UnitTests;

public class CodeFindingTests
{
    [Test]
    public void FindCode_NothingToSkip()
    {
        var result = "foo bar baz".AsSpan().FindCode("bar");

        Assert.AreEqual(4, result);
    }

    [Test]
    public void FindCode_SkipsOverBracePairs()
    {
        Assert.AreEqual(10, "foo (bar) bar baz".AsSpan().FindCode("bar"));
        Assert.AreEqual(12, "foo ((bar)) bar baz".AsSpan().FindCode("bar"));
        Assert.AreEqual(2, "())".AsSpan().FindCode(")"));
    }

    [Test]
    public void FindCode_SkipsOverStrings()
    {
        Assert.AreEqual(10, @"foo ""bar"" bar baz".AsSpan().FindCode("bar"));
        Assert.AreEqual(11, @"foo @""bar"" bar baz".AsSpan().FindCode("bar"));
        Assert.AreEqual(11, @"foo $""bar"" bar baz".AsSpan().FindCode("bar"));
        Assert.AreEqual(12, @"foo $@""bar"" bar baz".AsSpan().FindCode("bar"));
        Assert.AreEqual(12, @"foo @$""bar"" bar baz".AsSpan().FindCode("bar"));
        Assert.AreEqual(13, @"foo @""""""bar"" bar baz".AsSpan().FindCode("bar"));
        Assert.AreEqual(17, @"foo $""{(""bar"")}"" bar baz".AsSpan().FindCode("bar"));
    }

    [Test]
    public void FindCode_WithStartingIndex()
    {
        Assert.AreEqual(8, "foo bar bar baz".AsSpan().FindCode("bar", 5));
    }

    [Test]
    public void SplitCode()
    {
        CollectionAssert.AreEqual(new[] { @"""foo##""", "bar", @"$""{baz##}"""}, @"""foo##""##bar##$""{baz##}""".AsSpan().SplitCode("##"));
    }
}