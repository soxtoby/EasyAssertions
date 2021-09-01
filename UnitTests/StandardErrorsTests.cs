using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using static System.Environment;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class StandardErrorsTests
    {
        private ITestExpressionProvider expressionProvider = null!;
        private readonly IStandardErrors sut = StandardErrors.Current;
        private const string ActualExpression = "test expression";
        private const string ExpectedExpression = "expected expression";

        [SetUp]
        public void SetUp()
        {
            expressionProvider = Substitute.For<ITestExpressionProvider>();
            expressionProvider.GetActualExpression().Returns(ActualExpression);
            expressionProvider.GetExpectedExpression().Returns(ExpectedExpression);
            TestExpression.OverrideProvider(expressionProvider);
        }

        [TearDown]
        public void TearDown()
        {
            TestExpression.DefaultProvider();
        }

        [Test]
        public void NotEqual_Objects_ToStringed()
        {
            var result = sut.NotEqual(new FakeObject("foo"), new FakeObject("bar")).Message;
            Assert.AreEqual(ActualExpression + NewLine
                + "should be " + ExpectedExpression + NewLine
                + "          <foo>" + NewLine
                + "but was   <bar>", result);
        }

        [Test]
        public void NotEqual_ExpectedIsLiteral_ExpectedExpressionNotIncluded()
        {
            expressionProvider.GetExpectedExpression().Returns("1");

            var result = sut.NotEqual(1, 2).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should be <1>" + NewLine
                + "but was   <2>", result);
        }

        [Test]
        public void NotEqual_Objects_IncludesMessage()
        {
            var result = sut.NotEqual(new FakeObject("foo"), new FakeObject("bar"), "baz").Message;
            StringAssert.EndsWith(NewLine + "baz", result);
        }

        [Test]
        public void NotEqual_SingleLineStrings()
        {
            var result = sut.NotEqual("acd", "abc").Message;
            Assert.AreEqual(ActualExpression + NewLine
                + "should be " + ExpectedExpression + NewLine
                + "          \"acd\"" + NewLine
                + "but was   \"abc\"" + NewLine
                 + "            ^" + NewLine
                + "Difference at index 1.", result);
        }

        [Test]
        public void StringsNotEqual_DifferenceOnLineTwo()
        {
            var result = sut.NotEqual("abc\ndfe", "abc\ndef").Message;
            Assert.AreEqual(ActualExpression + NewLine
                + "should be " + ExpectedExpression + NewLine
                + "          \"abc\\ndfe\"" + NewLine
                + "but was   \"abc\\ndef\"" + NewLine
                  + "                 ^" + NewLine
                + "Difference at index 5.", result);
        }

        [Test]
        public void StringsNotEqual_DifferenceFarAwayFromBeginning()
        {
            var result = sut.NotEqual("0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz",
                                                                             "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnpoqrstuvwxyz").Message;
            Assert.AreEqual(ActualExpression + NewLine
                + "should be " + ExpectedExpression + NewLine
                + "          \"...fghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz\"" + NewLine
                + "but was   \"...fghijklmnopqrstuvwxyz0123456789abcdefghijklmnpoqrstuvwxyz\"" + NewLine
                 + "                                                           ^" + NewLine
                + "Difference at index 60.", result);
        }

        [Test]
        public void StringsNotEqual_DifferenceFarAwayFromEnd()
        {
            var result = sut.NotEqual("0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz",
                                         "0123456789abdcefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz").Message;
            Assert.AreEqual(ActualExpression + NewLine
                + "should be " + ExpectedExpression + NewLine
                + "          \"0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijk...\"" + NewLine
                + "but was   \"0123456789abdcefghijklmnopqrstuvwxyz0123456789abcdefghijk...\"" + NewLine
                 + "                       ^" + NewLine
                + "Difference at index 12.", result);
        }

        [Test]
        public void StringsNotEqual_DifferenceFarAwayFromBothEnds()
        {
            var result = sut.NotEqual("0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789",
                                         "0123456789abcdefghijkmlnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789").Message;
            Assert.AreEqual(ActualExpression + NewLine
                + "should be " + ExpectedExpression + NewLine
                + "          \"...456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijkl...\"" + NewLine
                + "but was   \"...456789abcdefghijkmlnopqrstuvwxyz0123456789abcdefghijkl...\"" + NewLine
                 + "                               ^" + NewLine
                + "Difference at index 21.", result);
        }

        [Test]
        public void StringsNotEqual_ExpectedIsLiteral_ExpectedExpressionNotIncluded()
        {
            expressionProvider.GetExpectedExpression().Returns("@\"acd\"");

            var result = sut.NotEqual("acd", "abc").Message;
            Assert.AreEqual(ActualExpression + NewLine
                + "should be \"acd\"" + NewLine
                + "but was   \"abc\"" + NewLine
                 + "            ^" + NewLine
                + "Difference at index 1.", result);
        }

        [Test]
        public void StringsNotEqual_ActualShorterThanExpected()
        {
            var result = sut.NotEqual("ab", "a").Message;
            Assert.AreEqual(ActualExpression + NewLine
                + "should be " + ExpectedExpression + NewLine
                + "          \"ab\"" + NewLine
                + "but was   \"a\"" + NewLine
                 + "            ^" + NewLine
                + "Difference at index 1.", result);
        }

        [Test]
        public void StringsNotEqual_ActualLongerThanExpected()
        {
            var result = sut.NotEqual("a", "ab").Message;
            Assert.AreEqual(ActualExpression + NewLine
                + "should be " + ExpectedExpression + NewLine
                + "          \"a\"" + NewLine
                + "but was   \"ab\"" + NewLine
                 + "            ^" + NewLine
                + "Difference at index 1.", result);
        }

        [Test]
        public void StringsNotEqual_LongActualShorterThanExpected()
        {
            var expected = "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz";
            var actual = "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxy";

            var result = sut.NotEqual(expected, actual).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should be " + ExpectedExpression + NewLine
                + "          \"...fghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz\"" + NewLine
                + "but was   \"...fghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxy\"" + NewLine
                 + "                                                                      ^" + NewLine
                + "Difference at index 71.", result);
        }

        [Test]
        public void StringsNotEqual_CaseInsensitive_DifferenceIgnoresCase()
        {
            var result = sut.NotEqual("abc", "Acb", Case.Insensitive).Message;
            Assert.AreEqual(ActualExpression + NewLine
                + "should be " + ExpectedExpression + NewLine
                + "          \"abc\"" + NewLine
                + "but was   \"Acb\"" + NewLine
                 + "            ^" + NewLine
                + "Difference at index 1.", result);
        }

        [Test]
        public void StringsNotEqual_IncludesMessage()
        {
            var result = sut.NotEqual("acd", "abc", message: "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void AreEqual_ObjectsToStringed()
        {
            var result = sut.AreEqual(new FakeObject("foo"), new FakeObject("bar")).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should not be " + ExpectedExpression + NewLine
                + "              <foo>" + NewLine
                + "but was       <bar>", result);
        }

        [Test]
        public void AreEqual_IncludesMessage()
        {
            var result = sut.AreEqual(null, null, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void IsNull()
        {
            var result = sut.IsNull().Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should not be null, but was.", result);
        }

        [Test]
        public void IsNull_IncludesMessage()
        {
            var result = sut.IsNull("foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void NotSame_ObjectsToStringed()
        {
            var expected = new FakeObject("foo");
            var actual = new FakeObject("bar");

            var result = sut.NotSame(expected, actual).Message;

            Assert.AreEqual(ActualExpression + NewLine
                 + "should be instance " + ExpectedExpression + NewLine
                 + "                   <foo>" + NewLine
                 + "but was            <bar>", result);
        }

        [Test]
        public void NotSame_IncludesMessage()
        {
            var result = sut.NotSame(new object(), new object(), "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void AreSame_ObjectToStringed()
        {
            var result = sut.AreSame(new FakeObject("foo")).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "shouldn't be instance " + ExpectedExpression + NewLine
                + "                      <foo>", result);
        }

        [Test]
        public void AreSame_IncludesMessage()
        {
            var result = sut.AreSame(null, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void NotEmpty_SingleElement()
        {
            FakeObject[] enumerable = { new FakeObject("foo") };

            var result = sut.NotEmpty(enumerable).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should be empty" + NewLine
                + "but had 1 element: <foo>", result);
        }

        [Test]
        public void NotEmpty_TwoElements()
        {
            FakeObject[] enumerable = { new FakeObject("foo"), new FakeObject("bar") };

            var result = sut.NotEmpty(enumerable).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should be empty" + NewLine
                + "but had 2 elements: [" + NewLine
                + "    <foo>," + NewLine
                + "    <bar>" + NewLine
                + "]", result);
        }

        [Test]
        public void NotEmpty_ManyItems_OnlyFirstTenDisplayed()
        {
            var enumerable = Enumerable.Range(1, 11).Select(i => new FakeObject(i.ToString()));

            var result = sut.NotEmpty(enumerable).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should be empty" + NewLine
                + "but had 11 elements: [" + NewLine
                + "    <1>," + NewLine
                + "    <2>," + NewLine
                + "    <3>," + NewLine
                + "    <4>," + NewLine
                + "    <5>," + NewLine
                + "    <6>," + NewLine
                + "    <7>," + NewLine
                + "    <8>," + NewLine
                + "    <9>," + NewLine
                + "    <10>," + NewLine
                + "    ..." + NewLine
                + "]", result);
        }

        [Test]
        public void NotEmpty_OnlyEnumeratesOnce()
        {
            var actual = MakeEnumerable(1);

            sut.NotEmpty(actual);

            Assert.AreEqual(1, actual.EnumerationCount);
        }

        [Test]
        public void NotEmpty_IncludesMessage()
        {
            FakeObject[] enumerable = { new FakeObject(string.Empty) };

            var result = sut.NotEmpty(enumerable, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void StringNotEmpty()
        {
            var result = sut.NotEmpty("foo").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should be empty" + NewLine
                + "but was \"foo\"", result);
        }

        [Test]
        public void StringNotEmpty_IncludesMessage()
        {
            var result = sut.NotEmpty("foo", "bar").Message;

            StringAssert.EndsWith(NewLine + "bar", result);
        }

        [Test]
        public void IsEmpty()
        {
            var result = sut.IsEmpty().Message;

            Assert.AreEqual(ActualExpression + NewLine + "should not be empty, but was.", result);
        }

        [Test]
        public void IsEmpty_IncludesMessage()
        {
            var result = sut.IsEmpty("foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void CollectionDoesNotContain_EmptyCollection()
        {
            var result = sut.DoesNotContain(1, Enumerable.Empty<int>()).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain " + ExpectedExpression + NewLine
                + "               <1>" + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void CollectionDoesNotContain_SingleItem()
        {
            var result = sut.DoesNotContain(1, new[] { 2 }).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain " + ExpectedExpression + NewLine
                + "               <1>" + NewLine
                + "but was       [<2>]", result);
        }

        [Test]
        public void CollectionDoesNotContain_MultipleItems()
        {
            var result = sut.DoesNotContain(1, new[] { 2, 3 }).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain " + ExpectedExpression + NewLine
                + "               <1>" + NewLine
                + "but was [" + NewLine
                + "    <2>," + NewLine
                + "    <3>" + NewLine
                + "]", result);
        }

        [Test]
        public void CollectionDoesNotContain_LongActual_OnlyFirst10Displayed()
        {
            var result = sut.DoesNotContain(0, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain " + ExpectedExpression + NewLine
                + "               <0>" + NewLine
                + "but was [" + NewLine
                + "    <1>," + NewLine
                + "    <2>," + NewLine
                + "    <3>," + NewLine
                + "    <4>," + NewLine
                + "    <5>," + NewLine
                + "    <6>," + NewLine
                + "    <7>," + NewLine
                + "    <8>," + NewLine
                + "    <9>," + NewLine
                + "    <10>," + NewLine
                + "    ..." + NewLine
                + "]", result);
        }

        [Test]
        public void CollectionDoesNotContain_IncludesItemType()
        {
            var result = sut.DoesNotContain(1, Enumerable.Empty<int>(), "foo").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain foo " + ExpectedExpression + NewLine
                + "                   <1>" + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void CollectionDoesNotContain_OnlyEnumeratesOnce()
        {
            var actual = MakeEnumerable(1);

            sut.DoesNotContain(2, actual);

            Assert.AreEqual(1, actual.EnumerationCount);
        }

        [Test]
        public void CollectionDoesNotContain_IncludesMessage()
        {
            var result = sut.DoesNotContain(0, Enumerable.Empty<int>(), message: "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void CollectionDoesNotContainItems()
        {
            var result = sut.DoesNotContainItems(new[] { 1, 2 }, new[] { 1 }, (a, e) => a == e).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain " + ExpectedExpression + NewLine
                + "but was missing item 1 <2>" + NewLine
                + "and was [ <1> ]", result);
        }

        [Test]
        public void CollectionDoesNotContainItems_ExpectedIsNewCollection_ExpectedExpressionNotIncluded()
        {
            expressionProvider.GetExpectedExpression().Returns("new[] { 1 }");

            var result = sut.DoesNotContainItems(new[] { 1 }, Enumerable.Empty<int>(), (a, e) => a == e).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain expected item 0 <1>" + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void CollectionDoesNotContainItems_IncludesMessage()
        {
            var result = sut.DoesNotContainItems(new[] { 1 }, Enumerable.Empty<int>(), (a, e) => a == e, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void CollectionDoesNotContainItems_ExpectedDuplicates_ActualIsMissingDuplicate()
        {
            var result = sut.DoesNotContainItems(new[] { 1, 1 }, new[] { 1 }, (a, e) => a == e, "foo").Message;

            Assert.AreEqual($@"{ActualExpression}
should contain {ExpectedExpression}
but was missing item 1 <1>
and was [ <1> ]
foo", result);
        }

        [Test]
        public void CollectionDoesNotContainItems_OnlyEnumeratesOnce()
        {
            var expected = MakeEnumerable(1);
            var actual = MakeEnumerable(2);

            sut.DoesNotContainItems(expected, actual, (a, e) => a == e);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void CollectionContains()
        {
            var result = sut.Contains(new FakeObject("foo"), Enumerable.Empty<object>()).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "shouldn't contain " + ExpectedExpression + NewLine
                + "                  <foo>" + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void CollectionContains_IncludesItemType()
        {
            var result = sut.Contains(new FakeObject("foo"), Enumerable.Empty<object>(), "bar").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "shouldn't contain bar " + ExpectedExpression + NewLine
                + "                      <foo>" + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void CollectionContains_OnlyEnumeratesOnce()
        {
            var actual = MakeEnumerable(2);

            sut.Contains(1, actual);

            Assert.AreEqual(1, actual.EnumerationCount);
        }

        [Test]
        public void CollectionContains_IncludesMessage()
        {
            var result = sut.Contains((object?)null, Enumerable.Empty<object>(), message: "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void CollectionOnlyContains()
        {
            var result = sut.OnlyContains(new[] { new FakeObject("foo") }, Enumerable.Empty<object>()).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain more than just " + ExpectedExpression + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void CollectionOnlyContains_OnlyEnumeratesOnce()
        {
            var expected = MakeEnumerable(1);
            var actual = MakeEnumerable(2);

            sut.OnlyContains(expected, actual);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void CollectionOnlyContains_IncludesMessage()
        {
            var result = sut.OnlyContains(new object[0], new object[0], "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void CollectionContainsItems()
        {
            var result = sut.Contains(new[] { 1 }, new[] { 2, 1 }).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "shouldn't contain " + ExpectedExpression + NewLine
                + "but contained <1>" + NewLine
                + "and was [" + NewLine
                + "    <2>," + NewLine
                + "    <1>" + NewLine
                + "]" + NewLine
                + "Match at index 1.", result);
        }

        [Test]
        public void CollectionContainsItems_ExpectedIsNewCollection()
        {
            expressionProvider.GetExpectedExpression().Returns("new[] { 1 }");

            var result = sut.Contains(new[] { 1 }, new[] { 2, 1 }).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "shouldn't contain <1>" + NewLine
                + "but was [" + NewLine
                + "    <2>," + NewLine
                + "    <1>" + NewLine
                + "]" + NewLine
                + "Match at index 1.", result);
        }

        [Test]
        public void CollectionContainsItems_OnlyEnumeratesOnce()
        {
            var expectedToNotContain = MakeEnumerable(1);
            var actual = MakeEnumerable(2);

            sut.Contains(expectedToNotContain, actual);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expectedToNotContain.EnumerationCount);
        }

        [Test]
        public void CollectionContainsItems_IncludesMessage()
        {
            var result = sut.Contains(new[] { 1 }, new[] { 1 }, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void DoesNotOnlyContain_MissingItem()
        {
            var result = sut.DoesNotOnlyContain(new[] { 1 }, new[] { 2 }, (a, e) => a == e).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain " + ExpectedExpression + NewLine
                + "but was missing item 0 <1>" + NewLine
                + "and was [ <2> ]", result);
        }

        [Test]
        public void DoesNotOnlyContain_ExpectedIsEmpty_NotEmptyMessage()
        {
            FakeObject[] enumerable = { new FakeObject("foo") };

            var result = sut.DoesNotOnlyContain(Enumerable.Empty<object>(), enumerable, (a, e) => a == e).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should be empty" + NewLine
                + "but had 1 element: <foo>", result);
        }

        [Test]
        public void DoesNotOnlyContain_ExtraItem_ContainsExtraItemMessage()
        {
            var result = sut.DoesNotOnlyContain(new[] { 1 }, new[] { 1, 2 }, (a, e) => a == e).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should only contain " + ExpectedExpression + NewLine
                + "but also contains [ <2> ]", result);
        }

        [Test]
        public void DoesNotOnlyContain_ExpectedContainsDuplicates_ActualIsMissingDuplicate()
        {
            var result = sut.DoesNotOnlyContain(new[] { 1, 1, 1 }, new[] { 1, 1 }, (a, e) => a == e, "foo").Message;

            Assert.AreEqual($@"{ActualExpression}
should contain {ExpectedExpression}
but was missing item 2 <1>
and was [
    <1>,
    <1>
]
foo", result);
        }

        [Test]
        public void DoesNotOnlyContain_ExpectedContainsDuplicates_ActualHasExtraDuplicate()
        {
            var result = sut.DoesNotOnlyContain(new[] { 1, 1 }, new[] { 1, 1, 1 }, (a, e) => a == e, "foo").Message;

            Assert.AreEqual($@"{ActualExpression}
should only contain {ExpectedExpression}
but also contains [ <1> ]
foo", result);
        }

        [Test]
        public void DoesNotOnlyContain_OnlyEnumeratesOnce()
        {
            var actual = MakeEnumerable(1, 2);
            var expected = MakeEnumerable(1);

            sut.DoesNotOnlyContain(expected, actual, (a, e) => a == e);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void ContainsExtraItem()
        {
            var result = sut.ContainsExtraItem(new[] { 1 }, new[] { 1, 2 }, (a, e) => a == e).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should only contain " + ExpectedExpression + NewLine
                + "but also contains [ <2> ]", result);
        }

        [Test]
        public void ContainsExtraItem_ExtraDuplicate()
        {
            var result = sut.ContainsExtraItem(new[] { 1, 1 }, new[] { new Equatable(1), new Equatable(1), new Equatable(1) }, (a, e) => a.Value == e).Message;

            Assert.AreEqual($@"{ActualExpression}
should only contain {ExpectedExpression}
but also contains [ <Eq(1)> ]", result);
        }

        [Test]
        public void ContainsExtraItem_ExpectedIsNewCollection_ExpectedExpressionNotIncluded()
        {
            expressionProvider.GetExpectedExpression().Returns("new[] { 1 }");

            var result = sut.ContainsExtraItem(new[] { 1 }, new[] { 1, 2 }, (a, e) => a == e).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should only contain [ <1> ]" + NewLine
                + "but also contains [ <2> ]", result);
        }

        [Test]
        public void ContainsExtraItem_OnlyEnumeratesOnce()
        {
            var expected = MakeEnumerable(1);
            var actual = MakeEnumerable(2);

            sut.ContainsExtraItem(expected, actual, (a, e) => a == e);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void ContainsExtraItem_IncludesMessage()
        {
            var result = sut.DoesNotOnlyContain(new[] { 1 }, new[] { 1, 2 }, (a, e) => a == e, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void ContainsDuplicate()
        {
            var result = sut.ContainsDuplicate(new[] { 1, 2, 1, 1 }, "foo").Message;

            Assert.AreEqual($@"{ActualExpression}
should not contain duplicates
but <1>
was found at indices 0, 2 and 3.
foo", result);
        }

        [Test]
        public void ContainsDuplicate_OnlyEnumeratesOnce()
        {
            var actual = MakeEnumerable(1, 1);

            sut.ContainsDuplicate(actual);

            Assert.AreEqual(1, actual.EnumerationCount);
        }

        [Test]
        public void DoNotMatch_NonMatchingCollections()
        {
            var result = sut.DoNotMatch(new[] { 1, 2, 3 }, new[] { 1, 3, 2 }, StandardTests.Instance.ObjectsAreEqual).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "doesn't match " + ExpectedExpression + ". Differs at index 1." + NewLine
                + "should match <2>" + NewLine
                + "but was      <3>", result);
        }

        [Test]
        public void DoNotMatch_ExpectedIsNewCollection_ExpectedExpressionNotIncluded()
        {
            expressionProvider.GetExpectedExpression().Returns("new List<int>() { 1, 2, 3 }");

            var result = sut.DoNotMatch(new[] { 1, 2, 3 }, new[] { 1, 3, 2 }, StandardTests.Instance.ObjectsAreEqual).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "differs at index 1." + NewLine
                + "should match <2>" + NewLine
                + "but was      <3>", result);
        }

        [Test]
        public void DoNotMatch_LengthMismatch()
        {
            var result = sut.DoNotMatch(new[] { 1, 2 }, Enumerable.Empty<int>(), StandardTests.Instance.ObjectsAreEqual).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "doesn't match " + ExpectedExpression + '.' + NewLine
                + "should have 2 elements" + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void DoNotMatch_Predicate()
        {
            var result = sut.DoNotMatch(new[] { 1, 2, 3 }, new[] { "1", "3", "2" }, (s, i) => s == i.ToString(CultureInfo.InvariantCulture)).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "doesn't match " + ExpectedExpression + ". "
                + "Differs at index 1." + NewLine
                + "should match <2>" + NewLine
                + "but was      \"3\"", result);
        }

        [Test]
        public void DoNotMatch_OnlyEnumeratesOnce()
        {
            var expected = MakeEnumerable(1);
            var actual = MakeEnumerable(2);

            sut.DoNotMatch(expected, actual, StandardTests.Instance.ObjectsAreEqual);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void DoNotMatch_IncludesMessage()
        {
            var result = sut.DoNotMatch(new[] { 1 }, new[] { 2 }, StandardTests.Instance.ObjectsAreEqual, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void ItemsNotSame_ToStringsItemsAtFirstDifference()
        {
            var same = new FakeObject("foo");

            var result = sut.ItemsNotSame(new[] { same, new FakeObject("bar") }, new[] { same, new FakeObject("baz") }).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "differs at index 1." + NewLine
                + "should be instance <bar>" + NewLine
                + "but was            <baz>", result);
        }

        [Test]
        public void ItemsNotSame_OnlyEnumeratesOnce()
        {
            var expected = MakeEnumerable(1);
            var actual = MakeEnumerable(2);

            sut.ItemsNotSame(expected, actual);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void ItemsNotSame_IncludesMessage()
        {
            var result = sut.ItemsNotSame(new[] { new object() }, new[] { new object() }, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void CollectionDoesNotStartWith_ActualIsShorterThanExpectedStart()
        {
            var result = sut.DoesNotStartWith(new[] { 1, 2 }, new[] { 1 }, StandardTests.Instance.ObjectsAreEqual, "foo").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should have at least 2 elements" + NewLine
                + "but had 1 element: <1>" + NewLine
                + "foo", result);
        }

        [Test]
        public void CollectionDoesNotStartWith_ActualIsLongerThanExpectedStart()
        {
            var result = sut.DoesNotStartWith(new[] { 1, 2 }, new[] { 1, 3, 2 }, StandardTests.Instance.ObjectsAreEqual, "foo").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "differs at index 1." + NewLine
                + "should be <2>" + NewLine
                + "but was   <3>" + NewLine
                + "foo", result);
        }

        [Test]
        public void CollectionDoesNotStartWith_OnlyEnumeratesOnce()
        {
            var actual = MakeEnumerable(1);
            var expected = MakeEnumerable(2);

            sut.DoesNotStartWith(expected, actual, StandardTests.Instance.ObjectsAreEqual);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void CollectionDoesNotEndWith_ActualIsShorterThanExpectedEnd()
        {
            var result = sut.DoesNotEndWith(new[] { 1, 2 }, new[] { 1 }, StandardTests.Instance.ObjectsAreEqual, "foo").Message;

            Assert.AreEqual($@"{ActualExpression}
should have at least 2 elements
but had 1 element: <1>
foo", result);
        }

        [Test]
        public void CollectionDoesNotEndWith_ActualIsLongerThanExpectedEnd()
        {
            var result = sut.DoesNotEndWith(new[] { 1, 2 }, new[] { 1, 3, 2 }, StandardTests.Instance.ObjectsAreEqual, "foo").Message;

            Assert.AreEqual($@"{ActualExpression}
differs at index 1.
should be <1>
but was   <3>
foo", result);
        }

        [Test]
        public void CollectionDoesNotEndWith_OnlyEnumeratesOnce()
        {
            var actual = MakeEnumerable(1);
            var expected = MakeEnumerable(2);

            sut.DoesNotEndWith(expected, actual, StandardTests.Instance.ObjectsAreEqual);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void TreesDoNotMatch_NonMatchingNodes()
        {
            var result = sut.TreesDoNotMatch(new[] { 1, 2.Node(21, 22) }, new[] { 1, 2.Node(21, 23) }, n => n, TestNodesMatch).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "doesn't match " + ExpectedExpression + "." + NewLine
                + "Differs at root -> 2, child index 1." + NewLine
                + "should be <22>" + NewLine
                + "but was   <23>", result);
        }

        [Test]
        public void TreesDoNotMatch_EmptyActual()
        {
            var result = sut.TreesDoNotMatch(new[] { 1.Node(11) }, new[] { 1.Node() }, n => n, TestNodesMatch).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "doesn't match " + ExpectedExpression + "." + NewLine
                + "root -> 1 node should have 1 child" + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void TreesDoNotMatch_SingleNodeActual()
        {
            var result = sut.TreesDoNotMatch(new TestNode<int>[] { 1, 2 }, new[] { 3 }, i => Enumerable.Empty<int>(), StandardTests.Instance.ObjectsAreEqual).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "doesn't match " + ExpectedExpression + "." + NewLine
                + "root node should have 2 children" + NewLine
                + "but had 1 child: <3>", result);
        }

        [Test]
        public void TreesDoNotMatch_MultipleNodeActual()
        {
            var result = sut.TreesDoNotMatch(new TestNode<int>[] { 1 }, new[] { 2, 3 }, i => Enumerable.Empty<int>(), StandardTests.Instance.ObjectsAreEqual).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "doesn't match " + ExpectedExpression + "." + NewLine
                + "root node should have 1 child" + NewLine
                + "but had 2 children: [" + NewLine
                + "    <2>," + NewLine
                + "    <3>" + NewLine
                + "]", result);
        }

        [Test]
        public void TreesDoNotMatch_ChildLengthMismatch_IncludesMessage()
        {
            var result = sut.TreesDoNotMatch(new TestNode<int>[] { 1 }, Enumerable.Empty<int>(), NoChildren, StandardTests.Instance.ObjectsAreEqual, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void TreesDoNotMatch_NonMatchingNode_IncludesMessage()
        {
            var result = sut.TreesDoNotMatch(new TestNode<int>[] { 1 }, new[] { 2 }, NoChildren, StandardTests.Instance.ObjectsAreEqual, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        private static IEnumerable<int> NoChildren(int i)
        {
            return Enumerable.Empty<int>();
        }

        private static bool TestNodesMatch(object a, object e)
        {
            return (int)e == ((TestNode<int>)a).Value;
        }

        [Test]
        public void LengthMismatch_EmptyEnumerable()
        {
            var result = sut.LengthMismatch(2, Enumerable.Empty<object>()).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should have 2 elements" + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void LengthMismatch_SingleElementExpected()
        {
            var result = sut.LengthMismatch(1, Enumerable.Empty<object>()).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should have 1 element" + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void LengthMismatch_SingleElement()
        {
            var result = sut.LengthMismatch(2, new[] { new FakeObject("foo") }).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should have 2 elements" + NewLine
                + "but had 1 element: <foo>", result);
        }

        [Test]
        public void LengthMismatch_OnlyEnumeratesOnce()
        {
            var actual = MakeEnumerable(1);

            sut.LengthMismatch(2, actual);

            Assert.AreEqual(1, actual.EnumerationCount);
        }

        [Test]
        public void LengthMismatch_IncludesMessage()
        {
            var result = sut.LengthMismatch(1, Enumerable.Empty<object>(), "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void StringDoesNotContain()
        {
            var result = sut.DoesNotContain("bar", "foo").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain " + ExpectedExpression + NewLine
                + "               \"bar\"" + NewLine
                + "but was        \"foo\"", result);
        }

        [Test]
        public void StringDoesNotContain_ActualIsLong_EndOfActualClipped()
        {
            var result = sut.DoesNotContain("foo",
                "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain " + ExpectedExpression + NewLine
                + "               \"foo\"" + NewLine
                + "but was        \"0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijk...\"", result);
        }

        [Test]
        public void StringDoesNotContain_IncludesMessage()
        {
            var result = sut.DoesNotContain(string.Empty, string.Empty, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void StringContains()
        {
            var result = sut.Contains("bar", "foobarbaz").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "shouldn't contain " + ExpectedExpression + NewLine
                + "                  \"bar\"" + NewLine
                + "but was           \"foobarbaz\"" + NewLine
                 + "                      ^" + NewLine
                + "Match at index 3.", result);
        }

        [Test]
        public void StringContains_IncludesMessage()
        {
            var result = sut.Contains("bar", "foobarbaz", "qux").Message;

            StringAssert.EndsWith(NewLine + "qux", result);
        }

        [Test]
        public void DoesNotStartWith()
        {
            var result = sut.DoesNotStartWith("foo", "bar").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should start with " + ExpectedExpression + NewLine
                + "                  \"foo\"" + NewLine
                + "but starts with   \"bar\"", result);
        }

        [Test]
        public void DoesNotStartWith_ActualIsLong_EndOfActualClipped()
        {
            var result = sut.DoesNotStartWith("foo",
                "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should start with " + ExpectedExpression + NewLine
                + "                  \"foo\"" + NewLine
                + "but starts with   \"0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijk...\"", result);
        }

        [Test]
        public void DoesNotStartWith_IncludesMessage()
        {
            var result = sut.DoesNotStartWith(string.Empty, string.Empty, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void DoesNotEndWith()
        {
            var result = sut.DoesNotEndWith("foo", "bar").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should end with " + ExpectedExpression + NewLine
                + "                \"foo\"" + NewLine
                + "but ends with   \"bar\"", result);
        }

        [Test]
        public void DoesNotEndWith_ActualIsLong_StartOfActualClipped()
        {
            var result = sut.DoesNotEndWith("foo",
                "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should end with " + ExpectedExpression + NewLine
                + "                \"foo\"" + NewLine
                + "but ends with   \"...fghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz\"", result);
        }

        [Test]
        public void DoesNotEndWith_IncludesMessage()
        {
            var result = sut.DoesNotEndWith(string.Empty, string.Empty, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void NoException_SimpleFunction()
        {
            Expression<Action> function = () => "".Trim();
            var result = sut.NoException(typeof(InvalidOperationException), function).Message;
            Assert.AreEqual("\"\".Trim()" + NewLine
                + "should throw <InvalidOperationException>" + NewLine
                + "but didn't throw at all.", result);
        }

        [Test]
        public void NoException_ClosureObjectMethod()
        {
            var str = "";
            Expression<Action> function = () => str.Trim();
            var result = sut.NoException(typeof(InvalidOperationException), function).Message;
            Assert.AreEqual("str.Trim()" + NewLine
               + "should throw <InvalidOperationException>" + NewLine
               + "but didn't throw at all.", result);
        }

        [Test]
        public void NoException_NoFunctionExpression()
        {
            var result = sut.NoException(typeof(InvalidOperationException)).Message;

            Assert.AreEqual($@"{ActualExpression}
should throw <InvalidOperationException>
but didn't throw at all.", result);
        }

        [Test]
        public void NoException_IncludesMessage()
        {
            Expression<Action> function = () => "".Trim();
            var result = sut.NoException(typeof(InvalidOperationException), function, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void WrongException_SimpleFunction()
        {
            var actualException = new Exception();
            Expression<Action> function = () => "".Trim();
            var result = sut.WrongException(typeof(InvalidOperationException), actualException, function);
            Assert.AreEqual("\"\".Trim()" + NewLine
                + "should throw <InvalidOperationException>" + NewLine
                + "but threw    <Exception>", result.Message);
            Assert.AreSame(actualException, result.InnerException);
        }

        [Test]
        public void WrongException_ClosureObjectMethod()
        {
            var actualException = new Exception();
            var str = "";
            Expression<Action> function = () => str.Trim();
            var result = sut.WrongException(typeof(InvalidOperationException), actualException, function);
            Assert.AreEqual("str.Trim()" + NewLine
                + "should throw <InvalidOperationException>" + NewLine
                + "but threw    <Exception>", result.Message);
        }

        [Test]
        public void WrongException_NoFunctionExpression()
        {
            var actualException = new Exception();
            var result = sut.WrongException(typeof(InvalidOperationException), actualException);

            Assert.AreEqual($@"{ActualExpression}
should throw <InvalidOperationException>
but threw    <Exception>", result.Message);
        }

        [Test]
        public void WrongException_IncludesMessage()
        {
            var actualException = new Exception();
            Expression<Action> function = () => "".Trim();
            var result = sut.WrongException(typeof(InvalidOperationException), actualException, function, "foo");

            StringAssert.EndsWith(NewLine + "foo", result.Message);
        }

        [Test]
        public void NotGreaterThan()
        {
            var result = sut.NotGreaterThan(2, 1).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should be greater than " + ExpectedExpression + NewLine
                + "                       <2>" + NewLine
                + "but was                <1>", result);
        }

        [Test]
        public void NotGreaterThan_IncludesMessage()
        {
            var result = sut.NotGreaterThan(2, 1, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void NotLessThan()
        {
            var result = sut.NotLessThan(1, 2).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should be less than " + ExpectedExpression + NewLine
                + "                    <1>" + NewLine
                + "but was             <2>", result);
        }

        [Test]
        public void NotLessThan_IncludesMessage()
        {
            var result = sut.NotLessThan(1, 2, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void DoesNotMatch()
        {
            var result = sut.DoesNotMatch(new Regex("foo"), "bar").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should match " + ExpectedExpression + NewLine
                + "             /foo/" + NewLine
                + "but was \"bar\"", result);
        }

        [Test]
        public void DoesNotMatch_IncludesMessage()
        {
            var result = sut.DoesNotMatch(new Regex(""), "", "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void Matches()
        {
            var result = sut.Matches(new Regex("foo"), "bar").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "shouldn't match " + ExpectedExpression + NewLine
                + "                /foo/" + NewLine
                + "but was \"bar\"", result);
        }

        [Test]
        public void Matches_IncludesMessage()
        {
            var result = sut.Matches(new Regex(""), "", "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void TimedOut()
        {
            var result = sut.TaskTimedOut(TimeSpan.FromMilliseconds(1), "foo").Message;

            Assert.AreEqual($@"{ActualExpression}
timed out after 1ms.
foo", result);
        }

        [Test]
        public void Matches_Collections()
        {
            var result = sut.Matches(new[] { 1, 2 }, new[] { 1, 2 }, "foo").Message;

            Assert.AreEqual($@"{ActualExpression}
should not match {ExpectedExpression}
but was [
    <1>,
    <2>
]
foo", result);
        }

        [Test]
        public void Matches_Collections_ExpectedIsNewCollection()
        {
            expressionProvider.GetExpectedExpression().Returns("new[] { 1, 2 }");

            var result = sut.Matches(new[] { 1, 2 }, new[] { 1, 2 }, "foo").Message;

            Assert.AreEqual($@"{ActualExpression}
should not match [
    <1>,
    <2>
]
but did.
foo", result);

        }

        private static TestEnumerable<T> MakeEnumerable<T>(params T[] items) => new TestEnumerable<T>(items);
    }
}
