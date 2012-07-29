using NSubstitute;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class StringAssertionTests : AssertionTests
    {
        [Test]
        public void ShouldContain_StringContainsSubstring_ReturnsActualValue()
        {
            Actual<string> result = "foo".ShouldContain("oo");
            Assert.AreEqual("foo", result.And);
        }

        [Test]
        public void ShouldContain_StringDoesNotContainSubstring_FailsWithStringDoesNotContainMessage()
        {
            MockFormatter.DoesNotContain("bar", "foo", "baz").Returns("qux");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => "foo".ShouldContain("bar", "baz"));

            Assert.AreEqual("qux", result.Message);
        }

        [Test]
        public void ShouldNotContain_StringDoesNotContainSubstring_ReturnsActualValue()
        {
            Actual<string> result = "foo".ShouldNotContain("bar");

            Assert.AreEqual("foo", result.And);
        }

        [Test]
        public void ShouldNotContain_StringContainsSubstring_FailsWithStringContainsMessage()
        {
            MockFormatter.Contains("bar", "foobarbaz", "baz").Returns("qux");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => "foobarbaz".ShouldNotContain("bar", "baz"));

            Assert.AreEqual("qux", result.Message);
        }

        [Test]
        public void ShouldStartWith_StringStartsWithExpected_ReturnsActualValue()
        {
            const string actual = "foobar";

            Actual<string> result = actual.ShouldStartWith("foo");

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldStartWith_StringDoesNotStartWithExpected_FailsWithStringDoesNotStartWithMessage()
        {
            const string actual = "foobar";
            const string expectedStart = "bar";
            MockFormatter.DoesNotStartWith(expectedStart, actual, "foo").Returns("baz");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldStartWith(expectedStart, "foo"));

            Assert.AreEqual("baz", result.Message);
        }

        [Test]
        public void ShouldEndWith_StringEndsWithExpected_ReturnsActualValue()
        {
            const string actual = "foobar";

            Actual<string> result = actual.ShouldEndWith("bar");

            Assert.AreSame(actual, result.Value);
        }

        [Test]
        public void ShouldEndWith_StringDoesNotEndWithExpected_FailsWithStringDoesNotEndWithMessage()
        {
            const string actual = "foobar";
            const string expectedEnd = "foo";
            MockFormatter.DoesNotEndWith(expectedEnd, actual, "bar").Returns("baz");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldEndWith(expectedEnd, "bar"));

            Assert.AreEqual("baz", result.Message);
        }
    }
}