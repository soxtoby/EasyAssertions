using NSubstitute;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class StringAssertionTests : AssertionTests
    {
        [Test]
        public void ShouldBeEmpty_StringIsEmpty_ReturnsEmptyString()
        {
            Actual<string> result = string.Empty.ShouldBeEmpty();

            Assert.AreEqual(string.Empty, result.And);
        }

        [Test]
        public void ShouldBeEmpty_IsNotEmpty_FailsWithStringNotEmptyMessage()
        {
            Error.NotEmpty("foo", "bar").Returns(ExpectedException);

            AssertThrowsExpectedError(() => "foo".ShouldBeEmpty("bar"));
        }

        [Test]
        public void ShouldBeEmpty_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            Error.NotEqual(typeof(string), null, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() => ((string?)null).ShouldBeEmpty("foo"));
        }

        [Test]
        public void ShouldBeEmpty_CorrectlyRegistersAssertion()
        {
            string actualExpression = string.Empty;

            actualExpression.ShouldBeEmpty();

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldContain_StringContainsSubstring_ReturnsActualValue()
        {
            Actual<string> result = "foo".ShouldContain("oo");
            Assert.AreEqual("foo", result.And);
        }

        [Test]
        public void ShouldContain_StringDoesNotContainSubstring_FailsWithStringDoesNotContainMessage()
        {
            Error.DoesNotContain("bar", "foo", "baz").Returns(ExpectedException);

            AssertThrowsExpectedError(() => "foo".ShouldContain("bar", "baz"));
        }

        [Test]
        public void ShouldContain_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            Error.NotEqual(typeof(string), null, "message").Returns(ExpectedException);

            AssertThrowsExpectedError(() => ((string?)null).ShouldContain("expected", "message"));
        }

        [Test]
        public void ShouldContain_ExpectedIsNull_ThrowsArgumentNullException()
        {
            AssertArgumentNullException("expectedToContain", () => "".ShouldContain(null!));
        }

        [Test]
        public void ShouldContain_CorrectlyRegistersAssertion()
        {
            const string actualExpression = "foo";
            const string expectedExpression = "bar";
            Error.DoesNotContain(Arg.Any<string>(), Arg.Any<string>()).Returns(ExpectedException);

            AssertThrowsExpectedError(() => actualExpression.ShouldContain(expectedExpression));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
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
            Error.Contains("bar", "foobarbaz", "baz").Returns(ExpectedException);

            AssertThrowsExpectedError(() => "foobarbaz".ShouldNotContain("bar", "baz"));
        }

        [Test]
        public void ShouldNotContain_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            Error.NotEqual(typeof(string), null, "message").Returns(ExpectedException);

            AssertThrowsExpectedError(() => ((string?)null).ShouldNotContain("expected", "message"));
        }

        [Test]
        public void ShouldNotContain_ExpectedIsNull_ThrowsArgumentNullException()
        {
            AssertArgumentNullException("expectedToNotContain", () => "".ShouldNotContain(null!));
        }

        [Test]
        public void ShouldNotContain_CorrectlyRegistersAssertion()
        {
            const string actualExpression = "foo";
            const string expectedExpression = "foo";
            Error.Contains(Arg.Any<string>(), Arg.Any<string>()).Returns(ExpectedException);

            AssertThrowsExpectedError(() => actualExpression.ShouldNotContain(expectedExpression));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
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
            Error.DoesNotStartWith(expectedStart, actual, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() => actual.ShouldStartWith(expectedStart, "foo"));
        }

        [Test]
        public void ShouldStartWith_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            Error.NotEqual(typeof(string), null, "message").Returns(ExpectedException);

            AssertThrowsExpectedError(() => ((string?)null).ShouldStartWith("expected", "message"));
        }

        [Test]
        public void ShouldStartWith_ExpectedIsNull_ThrowsArgumentNullException()
        {
            AssertArgumentNullException("expectedStart", () => "".ShouldStartWith(null!));
        }

        [Test]
        public void ShouldStartWith_CorrectlyRegistersAssertion()
        {
            const string actualExpression = "foo";
            const string expectedExpression = "bar";
            Error.DoesNotStartWith(Arg.Any<string>(), Arg.Any<string>()).Returns(ExpectedException);

            AssertThrowsExpectedError(() => actualExpression.ShouldStartWith(expectedExpression));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
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
            Error.DoesNotEndWith(expectedEnd, actual, "bar").Returns(ExpectedException);

            AssertThrowsExpectedError(() => actual.ShouldEndWith(expectedEnd, "bar"));
        }

        [Test]
        public void ShouldEndWith_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            Error.NotEqual(typeof(string), null, "message").Returns(ExpectedException);

            AssertThrowsExpectedError(() => ((string?)null).ShouldEndWith("expected", "message"));

        }

        [Test]
        public void ShouldEndWith_ExpectedIsNull_ThrowsArgumentNullException()
        {
            AssertArgumentNullException("expectedEnd", () => "".ShouldEndWith(null!));
        }

        [Test]
        public void ShouldEndWith_CorrectlyRegistersAssertion()
        {
            const string actualExpression = "foo";
            const string expectedExpression = "bar";
            Error.DoesNotEndWith(Arg.Any<string>(), Arg.Any<string>()).Returns(ExpectedException);

            AssertThrowsExpectedError(() => actualExpression.ShouldEndWith(expectedExpression));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
        }

        [Test]
        public void ShouldBe_CaseSensitive_StringsAreEqual_ReturnsActualValue()
        {
            Actual<string> result = "foo".ShouldBe("foo", Case.Sensitive);

            Assert.AreEqual("foo", result.And);
        }

        [Test]
        public void ShouldBe_CaseSensitive_StringCaseDoesNotMatch_FailsWithStringsNotEqualMessage()
        {
            Error.NotEqual("foo", "fOo", Case.Sensitive, "bar").Returns(ExpectedException);

            AssertThrowsExpectedError(() => "fOo".ShouldBe("foo", Case.Sensitive, "bar"));
        }

        [Test]
        public void ShouldBe_CaseInsensitive_StringsAreEqual_ReturnsActualValue()
        {
            Actual<string> result = "FOO".ShouldBe("foo", Case.Insensitive);

            Assert.AreEqual("FOO", result.And);
        }

        [Test]
        public void ShouldBe_CaseInsensitive_StringsNotEqual_FailsWithStringsNotEqualMessage()
        {
            Error.NotEqual("bar", "foo", Case.Insensitive, "baz").Returns(ExpectedException);

            AssertThrowsExpectedError(() => "foo".ShouldBe("bar", Case.Insensitive, "baz"));
        }

        [Test]
        public void ShouldBe_CustomCaseSensitivity_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            Error.NotEqual(typeof(string), null, "bar").Returns(ExpectedException);

            AssertThrowsExpectedError(() => ((string?)null).ShouldBe("foo", Case.Insensitive, "bar"));
        }

        [Test]
        public void ShouldBe_CustomCaseSensitivity_ExpectedIsNull_ThrowsArgumentNullException()
        {
            AssertArgumentNullException("expected", () => "foo".ShouldBe(null!, Case.Insensitive));
        }

        [Test]
        public void ShouldBe_CustomCaseSensitivity_CorrectlyRegistersAssertion()
        {
            const string actualExpression = "foo";
            const string expectedExpression = "bar";
            Error.NotEqual(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Case>()).Returns(ExpectedException);

            AssertThrowsExpectedError(() => actualExpression.ShouldBe(expectedExpression, Case.Insensitive));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
        }

        [Test]
        public void ShouldMatchPattern_MatchesRegex_ReturnsActualValue()
        {
            Actual<string> result = "foo".ShouldMatch(".*");

            Assert.AreEqual("foo", result.And);
        }

        [Test]
        public void ShouldMatchPattern_DoesNotMatchRegex_FailsWithDoesNotMatchMessage()
        {
            Error.DoesNotMatch(Arg.Is<Regex>(r => r.ToString() == "bar"), "foo", "baz").Returns(ExpectedException);

            AssertThrowsExpectedError(() => "foo".ShouldMatch("bar", "baz"));
        }

        [Test]
        public void ShouldMatchPattern_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            Error.NotEqual(typeof(string), null, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() => ((string?)null).ShouldMatch("", "foo"));
        }

        [Test]
        public void ShouldMatchPattern_RegexPatternIsNull_ThrowsArgumentNullException()
        {
            AssertArgumentNullException("regexPattern", () => "".ShouldMatch((string)null!));
        }

        [Test]
        public void ShouldMatchPattern_CorrectlyRegistersAssertion()
        {
            const string actualExpression = "foo";
            const string expectedExpression = "bar";
            Error.DoesNotMatch(Arg.Any<Regex>(), Arg.Any<string>()).Returns(ExpectedException);

            AssertThrowsExpectedError(() => actualExpression.ShouldMatch(expectedExpression));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
        }

        [Test]
        public void ShouldMatchPattern_WithRegexOptions_MatchesRegex_ReturnsActualValue()
        {
            Actual<string> result = "foo".ShouldMatch("FOO", RegexOptions.IgnoreCase);

            Assert.AreEqual("foo", result.And);
        }

        [Test]
        public void ShouldMatchPattern_WithRegexOptions_DoesNotMatchRegex_FailsWithDoesNotMatchMessage()
        {
            Error.DoesNotMatch(Arg.Is<Regex>(r => r.ToString() == "bar" && r.Options == RegexOptions.IgnoreCase), "foo", "baz").Returns(ExpectedException);

            AssertThrowsExpectedError(() => "foo".ShouldMatch("bar", RegexOptions.IgnoreCase, "baz"));
        }

        [Test]
        public void ShouldMatchPattern_WithRegexOptions_RegexPatternIsNull_ThrowsArgumentNullException()
        {
            AssertArgumentNullException("regexPattern", () => "".ShouldMatch(null!, RegexOptions.IgnoreCase));
        }

        [Test]
        public void ShouldMatchPattern_WithRegexOptions_CorrectlyRegistersAssertion()
        {
            const string actualExpression = "foo";
            const string expectedExpression = "bar";
            Error.DoesNotMatch(Arg.Any<Regex>(), Arg.Any<string>()).Returns(ExpectedException);

            AssertThrowsExpectedError(() => actualExpression.ShouldMatch(expectedExpression, RegexOptions.None));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
        }

        [Test]
        public void ShouldMatchRegex_MatchesRegex_ReturnsActualValue()
        {
            Actual<string> result = "foo".ShouldMatch(new Regex(".*"));

            Assert.AreEqual("foo", result.And);
        }

        [Test]
        public void ShouldMatchRegex_DoesNotMatchRegex_FailsWithDoesNotMatchMessage()
        {
            Regex regex = new Regex("bar");
            Error.DoesNotMatch(regex, "foo", "baz").Returns(ExpectedException);

            AssertThrowsExpectedError(() => "foo".ShouldMatch(regex, "baz"));
        }

        [Test]
        public void ShouldMatchRegex_RegexIsNull_ThrowsArgumentNullException()
        {
            AssertArgumentNullException("regex", () => "".ShouldMatch((Regex)null!));
        }

        [Test]
        public void ShouldMatchRegex_CorrectlyRegistersAssertion()
        {
            const string actualExpression = "foo";
            Regex expectedExpression = new Regex("bar");
            Error.DoesNotMatch(Arg.Any<Regex>(), Arg.Any<string>()).Returns(ExpectedException);

            AssertThrowsExpectedError(() => actualExpression.ShouldMatch(expectedExpression));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
        }

        [Test]
        public void ShouldNotMatchPattern_DoesNotMatchRegex_ReturnsActualValue()
        {
            Actual<string> result = "foo".ShouldNotMatch("bar");

            Assert.AreEqual("foo", result.And);
        }

        [Test]
        public void ShouldNotMatchPattern_MatchesRegex_FailsWithMatchesMessage()
        {
            Error.Matches(Arg.Is<Regex>(r => r.ToString() == ".*"), "foo", "bar").Returns(ExpectedException);

            AssertThrowsExpectedError(() => "foo".ShouldNotMatch(".*", "bar"));
        }

        [Test]
        public void ShouldNotMatchPattern_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            Error.NotEqual(typeof(string), null, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() => ((string?)null).ShouldNotMatch("", "foo"));
        }

        [Test]
        public void ShouldNotMatchPattern_RegexPatternIsNull_ThrowsArgumentNullException()
        {
            AssertArgumentNullException("regexPattern", () => "".ShouldNotMatch((string)null!));
        }

        [Test]
        public void ShouldNotMatchPattern_CorrectlyRegistersAssertion()
        {
            const string actualExpression = "foo";
            const string expectedExpression = "foo";
            Error.Matches(Arg.Any<Regex>(), Arg.Any<string>()).Returns(ExpectedException);

            AssertThrowsExpectedError(() => actualExpression.ShouldNotMatch(expectedExpression));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
        }

        [Test]
        public void ShouldNotMatchPattern_WithRegexOptions_DoesNotMatchRegex_ReturnsActualValue()
        {
            Actual<string> result = "foo".ShouldNotMatch("bar", RegexOptions.IgnoreCase);

            Assert.AreEqual("foo", result.And);
        }

        [Test]
        public void ShouldNotMatchPattern_WithRegexOptions_MatchesRegex_FailsWithMatchesMessage()
        {
            Error.Matches(Arg.Is<Regex>(r => r.ToString() == ".*" && r.Options == RegexOptions.IgnoreCase), "foo", "bar").Returns(ExpectedException);

            AssertThrowsExpectedError(() => "foo".ShouldNotMatch(".*", RegexOptions.IgnoreCase, "bar"));
        }

        [Test]
        public void ShouldNotMatchPattern_WithRegexOptions_RegexPatternIsNull_ThrowsArgumentNullException()
        {
            AssertArgumentNullException("regexPattern", () => "".ShouldNotMatch(null!, RegexOptions.IgnoreCase));
        }

        [Test]
        public void ShouldNotMatchPattern_WithRegexOptions_CorrectlyRegistersAssertion()
        {
            const string actualExpression = "foo";
            const string expectedExpression = "foo";
            Error.Matches(Arg.Any<Regex>(), Arg.Any<string>()).Returns(ExpectedException);

            AssertThrowsExpectedError(() => actualExpression.ShouldNotMatch(expectedExpression, RegexOptions.None));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
        }

        [Test]
        public void ShouldNotMatchRegex_DoesNotMatchRegex_ReturnsActualValue()
        {
            Actual<string> result = "foo".ShouldNotMatch(new Regex("bar"));

            Assert.AreEqual("foo", result.And);
        }

        [Test]
        public void ShouldNotMatchRegex_MatchesRegex_FailsWithMatchesMessage()
        {
            Regex regex = new Regex("foo");
            Error.Matches(regex, "foo", "bar").Returns(ExpectedException);

            AssertThrowsExpectedError(() => "foo".ShouldNotMatch(regex, "bar"));
        }

        [Test]
        public void ShouldNotMatchRegex_RegexIsNull_ThrowsArgumentNullException()
        {
            AssertArgumentNullException("regex", () => "".ShouldNotMatch((Regex)null!));
        }

        [Test]
        public void ShouldNotMatchRegex_CorrectlyRegistersAssertion()
        {
            const string actualExpression = "foo";
            Regex expectedExpression = new Regex(".");
            Error.Matches(Arg.Any<Regex>(), Arg.Any<string>()).Returns(ExpectedException);

            AssertThrowsExpectedError(() => actualExpression.ShouldNotMatch(expectedExpression));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
        }
    }
}