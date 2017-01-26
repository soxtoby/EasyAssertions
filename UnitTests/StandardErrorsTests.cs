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
        private TestExpressionProvider expressionProvider;
        private readonly IStandardErrors sut = StandardErrors.Current;
        private const string ActualExpression = "test expression";
        private const string ExpectedExpression = "expected expression";

        [SetUp]
        public void SetUp()
        {
            expressionProvider = Substitute.For<TestExpressionProvider>();
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
            string result = sut.NotEqual(new FakeObject("foo"), new FakeObject("bar")).Message;
            Assert.AreEqual(ActualExpression + NewLine
                + "should be " + ExpectedExpression + NewLine
                + "          <foo>" + NewLine
                + "but was   <bar>", result);
        }

        [Test]
        public void NotEqual_ExpectedIsLiteral_ExpectedExpressionNotIncluded()
        {
            expressionProvider.GetExpectedExpression().Returns("1");

            string result = sut.NotEqual(1, 2).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should be <1>" + NewLine
                + "but was   <2>", result);
        }

        [Test]
        public void NotEqual_Objects_IncludesMessage()
        {
            string result = sut.NotEqual(new FakeObject("foo"), new FakeObject("bar"), "baz").Message;
            StringAssert.EndsWith(NewLine + "baz", result);
        }

        [Test]
        public void NotEqual_SingleLineStrings()
        {
            string result = sut.NotEqual("acd", "abc").Message;
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
            string result = sut.NotEqual("abc\ndfe", "abc\ndef").Message;
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
            string result = sut.NotEqual("0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz",
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
            string result = sut.NotEqual("0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz",
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
            string result = sut.NotEqual("0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789",
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

            string result = sut.NotEqual("acd", "abc").Message;
            Assert.AreEqual(ActualExpression + NewLine
                + "should be \"acd\"" + NewLine
                + "but was   \"abc\"" + NewLine
                 + "            ^" + NewLine
                + "Difference at index 1.", result);
        }

        [Test]
        public void StringsNotEqual_ActualShorterThanExpected()
        {
            string result = sut.NotEqual("ab", "a").Message;
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
            string result = sut.NotEqual("a", "ab").Message;
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
            string expected = "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz";
            string actual = "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxy";

            string result = sut.NotEqual(expected, actual).Message;

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
            string result = sut.NotEqual("abc", "Acb", Case.Insensitive).Message;
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
            string result = sut.NotEqual("acd", "abc", message: "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void AreEqual_ObjectsToStringed()
        {
            string result = sut.AreEqual(new FakeObject("foo"), new FakeObject("bar")).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should not be " + ExpectedExpression + NewLine
                + "              <foo>" + NewLine
                + "but was       <bar>", result);
        }

        [Test]
        public void AreEqual_IncludesMessage()
        {
            string result = sut.AreEqual(null, null, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void IsNull()
        {
            string result = sut.IsNull().Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should not be null, but was.", result);
        }

        [Test]
        public void IsNull_IncludesMessage()
        {
            string result = sut.IsNull("foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void NotSame_ObjectsToStringed()
        {
            FakeObject expected = new FakeObject("foo");
            FakeObject actual = new FakeObject("bar");

            string result = sut.NotSame(expected, actual).Message;

            Assert.AreEqual(ActualExpression + NewLine
                 + "should be instance " + ExpectedExpression + NewLine
                 + "                   <foo>" + NewLine
                 + "but was            <bar>", result);
        }

        [Test]
        public void NotSame_IncludesMessage()
        {
            string result = sut.NotSame(new object(), new object(), "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void AreSame_ObjectToStringed()
        {
            string result = sut.AreSame(new FakeObject("foo")).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "shouldn't be instance " + ExpectedExpression + NewLine
                + "                      <foo>", result);
        }

        [Test]
        public void AreSame_IncludesMessage()
        {
            string result = sut.AreSame(null, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void NotEmpty_SingleElement()
        {
            FakeObject[] enumerable = { new FakeObject("foo") };

            string result = sut.NotEmpty(enumerable).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should be empty" + NewLine
                + "but had 1 element: <foo>", result);
        }

        [Test]
        public void NotEmpty_TwoElements()
        {
            FakeObject[] enumerable = { new FakeObject("foo"), new FakeObject("bar") };

            string result = sut.NotEmpty(enumerable).Message;

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
            IEnumerable<FakeObject> enumerable = Enumerable.Range(1, 11).Select(i => new FakeObject(i.ToString()));

            string result = sut.NotEmpty(enumerable).Message;

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
            TestEnumerable<int> actual = MakeEnumerable(1);

            sut.NotEmpty(actual);

            Assert.AreEqual(1, actual.EnumerationCount);
        }

        [Test]
        public void NotEmpty_IncludesMessage()
        {
            FakeObject[] enumerable = { new FakeObject(string.Empty) };

            string result = sut.NotEmpty(enumerable, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void StringNotEmpty()
        {
            string result = sut.NotEmpty("foo").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should be empty" + NewLine
                + "but was \"foo\"", result);
        }

        [Test]
        public void StringNotEmpty_IncludesMessage()
        {
            string result = sut.NotEmpty("foo", "bar").Message;

            StringAssert.EndsWith(NewLine + "bar", result);
        }

        [Test]
        public void IsEmpty()
        {
            string result = sut.IsEmpty().Message;

            Assert.AreEqual(ActualExpression + NewLine + "should not be empty, but was.", result);
        }

        [Test]
        public void IsEmpty_IncludesMessage()
        {
            string result = sut.IsEmpty("foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void CollectionDoesNotContain_EmptyCollection()
        {
            string result = sut.DoesNotContain(1, Enumerable.Empty<int>()).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain " + ExpectedExpression + NewLine
                + "               <1>" + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void CollectionDoesNotContain_SingleItem()
        {
            string result = sut.DoesNotContain(1, new[] { 2 }).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain " + ExpectedExpression + NewLine
                + "               <1>" + NewLine
                + "but was       [<2>]", result);
        }

        [Test]
        public void CollectionDoesNotContain_MultipleItems()
        {
            string result = sut.DoesNotContain(1, new[] { 2, 3 }).Message;

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
            string result = sut.DoesNotContain(0, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }).Message;

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
            string result = sut.DoesNotContain(1, Enumerable.Empty<int>(), "foo").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain foo " + ExpectedExpression + NewLine
                + "                   <1>" + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void CollectionDoesNotContain_OnlyEnumeratesOnce()
        {
            TestEnumerable<int> actual = MakeEnumerable(1);

            sut.DoesNotContain(2, actual);

            Assert.AreEqual(1, actual.EnumerationCount);
        }

        [Test]
        public void CollectionDoesNotContain_IncludesMessage()
        {
            string result = sut.DoesNotContain(0, Enumerable.Empty<int>(), message: "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void CollectionDoesNotContainItems()
        {
            string result = sut.DoesNotContainItems(new[] { 1, 2 }, new[] { 1 }).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain " + ExpectedExpression + NewLine
                + "but was missing item 1 <2>" + NewLine
                + "and was [<1>]", result);
        }

        [Test]
        public void CollectionDoesNotContainItems_ExpectedIsNewCollection_ExpectedExpressionNotIncluded()
        {
            expressionProvider.GetExpectedExpression().Returns("new[] { 1 }");

            string result = sut.DoesNotContainItems(new[] { 1 }, Enumerable.Empty<int>()).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain expected item 0 <1>" + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void CollectionDoesNotContainItems_IncludesMessage()
        {
            string result = sut.DoesNotContainItems(new[] { 1 }, Enumerable.Empty<int>(), "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void CollectionDoesNotContainItems_OnlyEnumeratesOnce()
        {
            TestEnumerable<int> expected = MakeEnumerable(1);
            TestEnumerable<int> actual = MakeEnumerable(2);

            sut.DoesNotContainItems(expected, actual);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void CollectionContains()
        {
            string result = sut.Contains(new FakeObject("foo"), Enumerable.Empty<object>()).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "shouldn't contain " + ExpectedExpression + NewLine
                + "                  <foo>" + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void CollectionContains_IncludesItemType()
        {
            string result = sut.Contains(new FakeObject("foo"), Enumerable.Empty<object>(), "bar").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "shouldn't contain bar " + ExpectedExpression + NewLine
                + "                      <foo>" + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void CollectionContains_OnlyEnumeratesOnce()
        {
            TestEnumerable<int> actual = MakeEnumerable(2);

            sut.Contains(1, actual);

            Assert.AreEqual(1, actual.EnumerationCount);
        }

        [Test]
        public void CollectionContains_IncludesMessage()
        {
            string result = sut.Contains((object)null, Enumerable.Empty<object>(), message: "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void CollectionOnlyContains()
        {
            string result = sut.OnlyContains(new[] { new FakeObject("foo") }, Enumerable.Empty<object>()).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain more than just " + ExpectedExpression + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void CollectionOnlyContains_OnlyEnumeratesOnce()
        {
            TestEnumerable<int> expected = MakeEnumerable(1);
            TestEnumerable<int> actual = MakeEnumerable(2);

            sut.OnlyContains(expected, actual);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void CollectionOnlyContains_IncludesMessage()
        {
            string result = sut.OnlyContains(new object[0], new object[0], "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void CollectionContainsItems()
        {
            string result = sut.Contains(new[] { 1 }, new[] { 2, 1 }).Message;

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

            string result = sut.Contains(new[] { 1 }, new[] { 2, 1 }).Message;

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
            TestEnumerable<int> expectedToNotContain = MakeEnumerable(1);
            TestEnumerable<int> actual = MakeEnumerable(2);

            sut.Contains(expectedToNotContain, actual);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expectedToNotContain.EnumerationCount);
        }

        [Test]
        public void CollectionContainsItems_IncludesMessage()
        {
            string result = sut.Contains(new[] { 1 }, new[] { 1 }, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void DoesNotOnlyContain_MissingItem()
        {
            string result = sut.DoesNotOnlyContain(new[] { 1 }, new[] { 2 }).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain " + ExpectedExpression + NewLine
                + "but was missing item 0 <1>" + NewLine
                + "and was [<2>]", result);
        }

        [Test]
        public void DoesNotOnlyContain_ExpectedIsEmpty_NotEmptyMessage()
        {
            FakeObject[] enumerable = { new FakeObject("foo") };

            string result = sut.DoesNotOnlyContain(Enumerable.Empty<object>(), enumerable).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should be empty" + NewLine
                + "but had 1 element: <foo>", result);
        }

        [Test]
        public void DoesNotOnlyContain_ExtraItem_ContainsExtraItemMessage()
        {
            string result = sut.DoesNotOnlyContain(new[] { 1 }, new[] { 1, 2 }).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should only contain " + ExpectedExpression + NewLine
                + "but also contains [<2>]", result);
        }

        [Test]
        public void DoesNotOnlyContain_OnlyEnumeratesOnce()
        {
            TestEnumerable<int> actual = MakeEnumerable(1, 2);
            TestEnumerable<int> expected = MakeEnumerable(1);

            sut.DoesNotOnlyContain(expected, actual);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void ContainsExtraItem()
        {
            string result = sut.ContainsExtraItem(new[] { 1 }, new[] { 1, 2 }).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should only contain " + ExpectedExpression + NewLine
                + "but also contains [<2>]", result);
        }

        [Test]
        public void ContainsExtraItem_ExpectedIsNewCollection_ExpectedExpressionNotIncluded()
        {
            expressionProvider.GetExpectedExpression().Returns("new[] { 1 }");

            string result = sut.ContainsExtraItem(new[] { 1 }, new[] { 1, 2 }).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should only contain [<1>]" + NewLine
                + "but also contains [<2>]", result);
        }

        [Test]
        public void ContainsExtraItem_OnlyEnumeratesOnce()
        {
            TestEnumerable<int> expected = MakeEnumerable(1);
            TestEnumerable<int> actual = MakeEnumerable(2);

            sut.ContainsExtraItem(expected, actual);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void ContainsExtraItem_IncludesMessage()
        {
            string result = sut.DoesNotOnlyContain(new[] { 1 }, new[] { 1, 2 }, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void ContainsDuplicate()
        {
            string result = sut.ContainsDuplicate(new[] { 1, 2, 1, 1 }, "foo").Message;

            Assert.AreEqual($@"{ActualExpression}
should not contain duplicates
but <1>
was found at indices 0, 2 and 3.
foo", result);
        }

        [Test]
        public void ContainsDuplicate_OnlyEnumeratesOnce()
        {
            TestEnumerable<int> actual = MakeEnumerable(1, 1);

            sut.ContainsDuplicate(actual);

            Assert.AreEqual(1, actual.EnumerationCount);
        }

        [Test]
        public void DoNotMatch_NonMatchingCollections()
        {
            string result = sut.DoNotMatch(new[] { 1, 2, 3 }, new[] { 1, 3, 2 }, StandardTests.Instance.ObjectsAreEqual).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "doesn't match " + ExpectedExpression + ". Differs at index 1." + NewLine
                + "should match <2>" + NewLine
                + "but was      <3>", result);
        }

        [Test]
        public void DoNotMatch_ExpectedIsNewCollection_ExpectedExpressionNotIncluded()
        {
            expressionProvider.GetExpectedExpression().Returns("new List<int>() { 1, 2, 3 }");

            string result = sut.DoNotMatch(new[] { 1, 2, 3 }, new[] { 1, 3, 2 }, StandardTests.Instance.ObjectsAreEqual).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "differs at index 1." + NewLine
                + "should match <2>" + NewLine
                + "but was      <3>", result);
        }

        [Test]
        public void DoNotMatch_LengthMismatch()
        {
            string result = sut.DoNotMatch(new[] { 1, 2 }, Enumerable.Empty<int>(), StandardTests.Instance.ObjectsAreEqual).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "doesn't match " + ExpectedExpression + '.' + NewLine
                + "should have 2 elements" + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void DoNotMatch_Predicate()
        {
            string result = sut.DoNotMatch(new[] { 1, 2, 3 }, new[] { "1", "3", "2" }, (s, i) => s == i.ToString(CultureInfo.InvariantCulture)).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "doesn't match " + ExpectedExpression + ". "
                + "Differs at index 1." + NewLine
                + "should match <2>" + NewLine
                + "but was      \"3\"", result);
        }

        [Test]
        public void DoNotMatch_OnlyEnumeratesOnce()
        {
            TestEnumerable<int> expected = MakeEnumerable(1);
            TestEnumerable<int> actual = MakeEnumerable(2);

            sut.DoNotMatch(expected, actual, StandardTests.Instance.ObjectsAreEqual);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void DoNotMatch_IncludesMessage()
        {
            string result = sut.DoNotMatch(new[] { 1 }, new[] { 2 }, StandardTests.Instance.ObjectsAreEqual, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void ItemsNotSame_ToStringsItemsAtFirstDifference()
        {
            FakeObject same = new FakeObject("foo");

            string result = sut.ItemsNotSame(new[] { same, new FakeObject("bar") }, new[] { same, new FakeObject("baz") }).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "differs at index 1." + NewLine
                + "should be instance <bar>" + NewLine
                + "but was            <baz>", result);
        }

        [Test]
        public void ItemsNotSame_OnlyEnumeratesOnce()
        {
            TestEnumerable<int> expected = MakeEnumerable(1);
            TestEnumerable<int> actual = MakeEnumerable(2);

            sut.ItemsNotSame(expected, actual);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void ItemsNotSame_IncludesMessage()
        {
            string result = sut.ItemsNotSame(new[] { new object() }, new[] { new object() }, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void CollectionDoesNotStartWith_ActualIsShorterThanExpectedStart()
        {
            string result = sut.DoesNotStartWith(new[] { 1, 2 }, new[] { 1 }, Equals, "foo").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should have at least 2 elements" + NewLine
                + "but had 1 element: <1>" + NewLine
                + "foo", result);
        }

        [Test]
        public void CollectionDoesNotStartWith_ActualIsLongerThanExpectedStart()
        {
            string result = sut.DoesNotStartWith(new[] { 1, 2 }, new[] { 1, 3, 2 }, Equals, "foo").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "differs at index 1." + NewLine
                + "should be <2>" + NewLine
                + "but was   <3>" + NewLine
                + "foo", result);
        }

        [Test]
        public void CollectionDoesNotStartWith_OnlyEnumeratesOnce()
        {
            TestEnumerable<int> actual = MakeEnumerable(1);
            TestEnumerable<int> expected = MakeEnumerable(2);

            sut.DoesNotStartWith(expected, actual, StandardTests.Instance.ObjectsAreEqual);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void CollectionDoesNotEndWith_ActualIsShorterThanExpectedEnd()
        {
            string result = sut.DoesNotEndWith(new[] { 1, 2 }, new[] { 1 }, Equals, "foo").Message;

            Assert.AreEqual($@"{ActualExpression}
should have at least 2 elements
but had 1 element: <1>
foo", result);
        }

        [Test]
        public void CollectionDoesNotEndWith_ActualIsLongerThanExpectedEnd()
        {
            string result = sut.DoesNotEndWith(new[] { 1, 2 }, new[] { 1, 3, 2 }, Equals, "foo").Message;

            Assert.AreEqual($@"{ActualExpression}
differs at index 1.
should be <1>
but was   <3>
foo", result);
        }

        [Test]
        public void CollectionDoesNotEndWith_OnlyEnumeratesOnce()
        {
            TestEnumerable<int> actual = MakeEnumerable(1);
            TestEnumerable<int> expected = MakeEnumerable(2);

            sut.DoesNotEndWith(expected, actual, StandardTests.Instance.ObjectsAreEqual);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void TreesDoNotMatch_NonMatchingNodes()
        {
            string result = sut.TreesDoNotMatch(new[] { 1, 2.Node(21, 22) }, new[] { 1, 2.Node(21, 23) }, n => n, TestNodesMatch).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "doesn't match " + ExpectedExpression + "." + NewLine
                + "Differs at root -> 2, child index 1." + NewLine
                + "should be <22>" + NewLine
                + "but was   <23>", result);
        }

        [Test]
        public void TreesDoNotMatch_EmptyActual()
        {
            string result = sut.TreesDoNotMatch(new[] { 1.Node(11) }, new[] { 1.Node() }, n => n, TestNodesMatch).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "doesn't match " + ExpectedExpression + "." + NewLine
                + "root -> 1 node should have 1 child" + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void TreesDoNotMatch_SingleNodeActual()
        {
            string result = sut.TreesDoNotMatch(new TestNode<int>[] { 1, 2 }, new[] { 3 }, i => Enumerable.Empty<int>(), StandardTests.Instance.ObjectsAreEqual).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "doesn't match " + ExpectedExpression + "." + NewLine
                + "root node should have 2 children" + NewLine
                + "but had 1 child: <3>", result);
        }

        [Test]
        public void TreesDoNotMatch_MultipleNodeActual()
        {
            string result = sut.TreesDoNotMatch(new TestNode<int>[] { 1 }, new[] { 2, 3 }, i => Enumerable.Empty<int>(), StandardTests.Instance.ObjectsAreEqual).Message;

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
            string result = sut.TreesDoNotMatch(new TestNode<int>[] { 1 }, Enumerable.Empty<int>(), NoChildren, StandardTests.Instance.ObjectsAreEqual, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void TreesDoNotMatch_NonMatchingNode_IncludesMessage()
        {
            string result = sut.TreesDoNotMatch(new TestNode<int>[] { 1 }, new[] { 2 }, NoChildren, StandardTests.Instance.ObjectsAreEqual, "foo").Message;

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
            string result = sut.LengthMismatch(2, Enumerable.Empty<object>()).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should have 2 elements" + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void LengthMismatch_SingleElementExpected()
        {
            string result = sut.LengthMismatch(1, Enumerable.Empty<object>()).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should have 1 element" + NewLine
                + "but was empty.", result);
        }

        [Test]
        public void LengthMismatch_SingleElement()
        {
            string result = sut.LengthMismatch(2, new[] { new FakeObject("foo") }).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should have 2 elements" + NewLine
                + "but had 1 element: <foo>", result);
        }

        [Test]
        public void LengthMismatch_OnlyEnumeratesOnce()
        {
            TestEnumerable<int> actual = MakeEnumerable(1);

            sut.LengthMismatch(2, actual);

            Assert.AreEqual(1, actual.EnumerationCount);
        }

        [Test]
        public void LengthMismatch_IncludesMessage()
        {
            string result = sut.LengthMismatch(1, Enumerable.Empty<object>(), "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void StringDoesNotContain()
        {
            string result = sut.DoesNotContain("bar", "foo").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain " + ExpectedExpression + NewLine
                + "               \"bar\"" + NewLine
                + "but was        \"foo\"", result);
        }

        [Test]
        public void StringDoesNotContain_ActualIsLong_EndOfActualClipped()
        {
            string result = sut.DoesNotContain("foo",
                "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should contain " + ExpectedExpression + NewLine
                + "               \"foo\"" + NewLine
                + "but was        \"0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijk...\"", result);
        }

        [Test]
        public void StringDoesNotContain_IncludesMessage()
        {
            string result = sut.DoesNotContain(string.Empty, string.Empty, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void StringContains()
        {
            string result = sut.Contains("bar", "foobarbaz").Message;

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
            string result = sut.Contains("bar", "foobarbaz", "qux").Message;

            StringAssert.EndsWith(NewLine + "qux", result);
        }

        [Test]
        public void DoesNotStartWith()
        {
            string result = sut.DoesNotStartWith("foo", "bar").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should start with " + ExpectedExpression + NewLine
                + "                  \"foo\"" + NewLine
                + "but starts with   \"bar\"", result);
        }

        [Test]
        public void DoesNotStartWith_ActualIsLong_EndOfActualClipped()
        {
            string result = sut.DoesNotStartWith("foo",
                "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should start with " + ExpectedExpression + NewLine
                + "                  \"foo\"" + NewLine
                + "but starts with   \"0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijk...\"", result);
        }

        [Test]
        public void DoesNotStartWith_IncludesMessage()
        {
            string result = sut.DoesNotStartWith(string.Empty, string.Empty, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void DoesNotEndWith()
        {
            string result = sut.DoesNotEndWith("foo", "bar").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should end with " + ExpectedExpression + NewLine
                + "                \"foo\"" + NewLine
                + "but ends with   \"bar\"", result);
        }

        [Test]
        public void DoesNotEndWith_ActualIsLong_StartOfActualClipped()
        {
            string result = sut.DoesNotEndWith("foo",
                "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should end with " + ExpectedExpression + NewLine
                + "                \"foo\"" + NewLine
                + "but ends with   \"...fghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz\"", result);
        }

        [Test]
        public void DoesNotEndWith_IncludesMessage()
        {
            string result = sut.DoesNotEndWith(string.Empty, string.Empty, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void NoException_SimpleFunction()
        {
            Expression<Action> function = () => "".Trim();
            string result = sut.NoException(typeof(InvalidOperationException), function).Message;
            Assert.AreEqual("\"\".Trim()" + NewLine
                + "should throw <InvalidOperationException>" + NewLine
                + "but didn't throw at all.", result);
        }

        [Test]
        public void NoException_ClosureObjectMethod()
        {
            string str = "";
            Expression<Action> function = () => str.Trim();
            string result = sut.NoException(typeof(InvalidOperationException), function).Message;
            Assert.AreEqual("str.Trim()" + NewLine
               + "should throw <InvalidOperationException>" + NewLine
               + "but didn't throw at all.", result);
        }

        [Test]
        public void NoException_NoFunctionExpression()
        {
            string result = sut.NoException(typeof(InvalidOperationException)).Message;

            Assert.AreEqual($@"{ActualExpression}
should throw <InvalidOperationException>
but didn't throw at all.", result);
        }

        [Test]
        public void NoException_IncludesMessage()
        {
            Expression<Action> function = () => "".Trim();
            string result = sut.NoException(typeof(InvalidOperationException), function, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void WrongException_SimpleFunction()
        {
            Exception actualException = new Exception();
            Expression<Action> function = () => "".Trim();
            Exception result = sut.WrongException(typeof(InvalidOperationException), actualException, function);
            Assert.AreEqual("\"\".Trim()" + NewLine
                + "should throw <InvalidOperationException>" + NewLine
                + "but threw    <Exception>", result.Message);
            Assert.AreSame(actualException, result.InnerException);
        }

        [Test]
        public void WrongException_ClosureObjectMethod()
        {
            Exception actualException = new Exception();
            string str = "";
            Expression<Action> function = () => str.Trim();
            Exception result = sut.WrongException(typeof(InvalidOperationException), actualException, function);
            Assert.AreEqual("str.Trim()" + NewLine
                + "should throw <InvalidOperationException>" + NewLine
                + "but threw    <Exception>", result.Message);
        }

        [Test]
        public void WrongException_NoFunctionExpression()
        {
            Exception actualException = new Exception();
            Exception result = sut.WrongException(typeof(InvalidOperationException), actualException);

            Assert.AreEqual($@"{ActualExpression}
should throw <InvalidOperationException>
but threw    <Exception>", result.Message);
        }

        [Test]
        public void WrongException_IncludesMessage()
        {
            Exception actualException = new Exception();
            Expression<Action> function = () => "".Trim();
            Exception result = sut.WrongException(typeof(InvalidOperationException), actualException, function, "foo");

            StringAssert.EndsWith(NewLine + "foo", result.Message);
        }

        [Test]
        public void NotGreaterThan()
        {
            string result = sut.NotGreaterThan(2, 1).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should be greater than " + ExpectedExpression + NewLine
                + "                       <2>" + NewLine
                + "but was                <1>", result);
        }

        [Test]
        public void NotGreaterThan_IncludesMessage()
        {
            string result = sut.NotGreaterThan(2, 1, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void NotLessThan()
        {
            string result = sut.NotLessThan(1, 2).Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should be less than " + ExpectedExpression + NewLine
                + "                    <1>" + NewLine
                + "but was             <2>", result);
        }

        [Test]
        public void NotLessThan_IncludesMessage()
        {
            string result = sut.NotLessThan(1, 2, "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void DoesNotMatch()
        {
            string result = sut.DoesNotMatch(new Regex("foo"), "bar").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "should match " + ExpectedExpression + NewLine
                + "             /foo/" + NewLine
                + "but was \"bar\"", result);
        }

        [Test]
        public void DoesNotMatch_IncludesMessage()
        {
            string result = sut.DoesNotMatch(new Regex(""), "", "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void Matches()
        {
            string result = sut.Matches(new Regex("foo"), "bar").Message;

            Assert.AreEqual(ActualExpression + NewLine
                + "shouldn't match " + ExpectedExpression + NewLine
                + "                /foo/" + NewLine
                + "but was \"bar\"", result);
        }

        [Test]
        public void Matches_IncludesMessage()
        {
            string result = sut.Matches(new Regex(""), "", "foo").Message;

            StringAssert.EndsWith(NewLine + "foo", result);
        }

        [Test]
        public void TimedOut()
        {
            string result = sut.TaskTimedOut(TimeSpan.FromMilliseconds(1), "foo").Message;

            Assert.AreEqual($@"{ActualExpression}
timed out after 1ms.
foo", result);
        }

        private static TestEnumerable<T> MakeEnumerable<T>(params T[] items) => new TestEnumerable<T>(items);
    }
}
