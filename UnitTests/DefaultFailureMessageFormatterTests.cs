using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class DefaultFailureMessageFormatterTests
    {
        private TestExpressionProvider expressionProvider;
        private readonly DefaultFailureMessageFormatter sut = DefaultFailureMessageFormatter.Instance;
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
            string result = sut.NotEqual(new FakeObject("foo"), new FakeObject("bar"));
            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be " + ExpectedExpression + Environment.NewLine
                + "          <foo>" + Environment.NewLine
                + "but was   <bar>", result);
        }

        [Test]
        public void NotEqual_ExpectedIsLiteral_ExpectedExpressionNotIncluded()
        {
            expressionProvider.GetExpectedExpression().Returns("1");

            string result = sut.NotEqual(1, 2);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be <1>" + Environment.NewLine
                + "but was   <2>", result);
        }

        [Test]
        public void NotEqual_Objects_IncludesMessage()
        {
            string result = sut.NotEqual(new FakeObject("foo"), new FakeObject("bar"), "baz");
            StringAssert.EndsWith(Environment.NewLine + "baz", result);
        }

        [Test]
        public void NotEqual_SingleLineStrings()
        {
            string result = sut.NotEqual("acd", "abc");
            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be " + ExpectedExpression + Environment.NewLine
                + "          \"acd\"" + Environment.NewLine
                + "but was   \"abc\"" + Environment.NewLine
                 + "            ^" + Environment.NewLine
                + "Difference at index 1.", result);
        }

        [Test]
        public void StringsNotEqual_DifferenceOnLineTwo()
        {
            string result = sut.NotEqual("abc\ndfe", "abc\ndef");
            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be " + ExpectedExpression + Environment.NewLine
                + "          \"abc\\ndfe\"" + Environment.NewLine
                + "but was   \"abc\\ndef\"" + Environment.NewLine
                  + "                 ^" + Environment.NewLine
                + "Difference at index 5.", result);
        }

        [Test]
        public void StringsNotEqual_DifferenceFarAwayFromBeginning()
        {
            string result = sut.NotEqual("0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz",
                                                                             "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnpoqrstuvwxyz");
            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be " + ExpectedExpression + Environment.NewLine
                + "          \"...fghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz\"" + Environment.NewLine
                + "but was   \"...fghijklmnopqrstuvwxyz0123456789abcdefghijklmnpoqrstuvwxyz\"" + Environment.NewLine
                 + "                                                           ^" + Environment.NewLine
                + "Difference at index 60.", result);
        }

        [Test]
        public void StringsNotEqual_DifferenceFarAwayFromEnd()
        {
            string result = sut.NotEqual("0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz",
                                                                             "0123456789abdcefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz");
            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be " + ExpectedExpression + Environment.NewLine
                + "          \"0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijk...\"" + Environment.NewLine
                + "but was   \"0123456789abdcefghijklmnopqrstuvwxyz0123456789abcdefghijk...\"" + Environment.NewLine
                 + "                       ^" + Environment.NewLine
                + "Difference at index 12.", result);
        }

        [Test]
        public void StringsNotEqual_DifferenceFarAwayFromBothEnds()
        {
            string result = sut.NotEqual("0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789",
                                                                             "0123456789abcdefghijkmlnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789");
            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be " + ExpectedExpression + Environment.NewLine
                + "          \"...456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijkl...\"" + Environment.NewLine
                + "but was   \"...456789abcdefghijkmlnopqrstuvwxyz0123456789abcdefghijkl...\"" + Environment.NewLine
                 + "                               ^" + Environment.NewLine
                + "Difference at index 21.", result);
        }

        [Test]
        public void StringsNotEqual_ExpectedIsLiteral_ExpectedExpressionNotIncluded()
        {
            expressionProvider.GetExpectedExpression().Returns("@\"acd\"");

            string result = sut.NotEqual("acd", "abc");
            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be \"acd\"" + Environment.NewLine
                + "but was   \"abc\"" + Environment.NewLine
                 + "            ^" + Environment.NewLine
                + "Difference at index 1.", result);
        }

        [Test]
        public void StringsNotEqual_ActualShorterThanExpected()
        {
            string result = sut.NotEqual("ab", "a");
            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be " + ExpectedExpression + Environment.NewLine
                + "          \"ab\"" + Environment.NewLine
                + "but was   \"a\"" + Environment.NewLine
                 + "            ^" + Environment.NewLine
                + "Difference at index 1.", result);
        }

        [Test]
        public void StringsNotEqual_ActualLongerThanExpected()
        {
            string result = sut.NotEqual("a", "ab");
            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be " + ExpectedExpression + Environment.NewLine
                + "          \"a\"" + Environment.NewLine
                + "but was   \"ab\"" + Environment.NewLine
                 + "            ^" + Environment.NewLine
                + "Difference at index 1.", result);
        }

        [Test]
        public void StringsNotEqual_CaseInsensitive_DifferenceIgnoresCase()
        {
            string result = sut.NotEqual("abc", "Acb", Case.Insensitive);
            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be " + ExpectedExpression + Environment.NewLine
                + "          \"abc\"" + Environment.NewLine
                + "but was   \"Acb\"" + Environment.NewLine
                 + "            ^" + Environment.NewLine
                + "Difference at index 1.", result);
        }

        [Test]
        public void StringsNotEqual_IncludesMessage()
        {
            string result = sut.NotEqual("acd", "abc", message: "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void AreEqual_ObjectsToStringed()
        {
            string result = sut.AreEqual(new FakeObject("foo"), new FakeObject("bar"));

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should not be " + ExpectedExpression + Environment.NewLine
                + "              <foo>" + Environment.NewLine
                + "but was       <bar>", result);
        }

        [Test]
        public void AreEqual_IncludesMessage()
        {
            string result = sut.AreEqual(null, null, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void IsNull()
        {
            string result = sut.IsNull();

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should not be null, but was.", result);
        }

        [Test]
        public void IsNull_IncludesMessage()
        {
            string result = sut.IsNull("foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void NotSame_ObjectsToStringed()
        {
            FakeObject expected = new FakeObject("foo");
            FakeObject actual = new FakeObject("bar");

            string result = sut.NotSame(expected, actual);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                 + "should be instance " + ExpectedExpression + Environment.NewLine
                 + "                   <foo>" + Environment.NewLine
                 + "but was            <bar>", result);
        }

        [Test]
        public void NotSame_IncludesMessage()
        {
            string result = sut.NotSame(new object(), new object(), "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void AreSame_ObjectToStringed()
        {
            string result = sut.AreSame(new FakeObject("foo"));

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "shouldn't be instance " + ExpectedExpression + Environment.NewLine
                + "                      <foo>", result);
        }

        [Test]
        public void AreSame_IncludesMessage()
        {
            string result = sut.AreSame(null, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void NotEmpty_SingleElement()
        {
            FakeObject[] enumerable = { new FakeObject("foo") };

            string result = sut.NotEmpty(enumerable);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be empty" + Environment.NewLine
                + "but had 1 element: <foo>", result);
        }

        [Test]
        public void NotEmpty_TwoElements()
        {
            FakeObject[] enumerable = { new FakeObject("foo"), new FakeObject("bar") };

            string result = sut.NotEmpty(enumerable);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be empty" + Environment.NewLine
                + "but had 2 elements: [" + Environment.NewLine
                + "    <foo>," + Environment.NewLine
                + "    <bar>" + Environment.NewLine
                + "]", result);
        }

        [Test]
        public void NotEmpty_ManyItems_OnlyFirstTenDisplayed()
        {
            IEnumerable<FakeObject> enumerable = Enumerable.Range(1, 11).Select(i => new FakeObject(i.ToString()));

            string result = sut.NotEmpty(enumerable);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be empty" + Environment.NewLine
                + "but had 11 elements: [" + Environment.NewLine
                + "    <1>," + Environment.NewLine
                + "    <2>," + Environment.NewLine
                + "    <3>," + Environment.NewLine
                + "    <4>," + Environment.NewLine
                + "    <5>," + Environment.NewLine
                + "    <6>," + Environment.NewLine
                + "    <7>," + Environment.NewLine
                + "    <8>," + Environment.NewLine
                + "    <9>," + Environment.NewLine
                + "    <10>," + Environment.NewLine
                + "    ..." + Environment.NewLine
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

            string result = sut.NotEmpty(enumerable, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void StringNotEmpty()
        {
            string result = sut.NotEmpty("foo");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be empty" + Environment.NewLine
                + "but was \"foo\"", result);
        }

        [Test]
        public void StringNotEmpty_IncludesMessage()
        {
            string result = sut.NotEmpty("foo", "bar");

            StringAssert.EndsWith(Environment.NewLine + "bar", result);
        }

        [Test]
        public void IsEmpty()
        {
            string result = sut.IsEmpty();

            Assert.AreEqual(ActualExpression + Environment.NewLine + "should not be empty, but was.", result);
        }

        [Test]
        public void IsEmpty_IncludesMessage()
        {
            string result = sut.IsEmpty("foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void CollectionDoesNotContain_EmptyCollection()
        {
            string result = sut.DoesNotContain(1, Enumerable.Empty<int>());

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should contain " + ExpectedExpression + Environment.NewLine
                + "               <1>" + Environment.NewLine
                + "but was empty.", result);
        }

        [Test]
        public void CollectionDoesNotContain_SingleItem()
        {
            string result = sut.DoesNotContain(1, new[] { 2 });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should contain " + ExpectedExpression + Environment.NewLine
                + "               <1>" + Environment.NewLine
                + "but was       [<2>]", result);
        }

        [Test]
        public void CollectionDoesNotContain_MultipleItems()
        {
            string result = sut.DoesNotContain(1, new[] { 2, 3 });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should contain " + ExpectedExpression + Environment.NewLine
                + "               <1>" + Environment.NewLine
                + "but was [" + Environment.NewLine
                + "    <2>," + Environment.NewLine
                + "    <3>" + Environment.NewLine
                + "]", result);
        }

        [Test]
        public void CollectionDoesNotContain_LongActual_OnlyFirst10Displayed()
        {
            string result = sut.DoesNotContain(0, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should contain " + ExpectedExpression + Environment.NewLine
                + "               <0>" + Environment.NewLine
                + "but was [" + Environment.NewLine
                + "    <1>," + Environment.NewLine
                + "    <2>," + Environment.NewLine
                + "    <3>," + Environment.NewLine
                + "    <4>," + Environment.NewLine
                + "    <5>," + Environment.NewLine
                + "    <6>," + Environment.NewLine
                + "    <7>," + Environment.NewLine
                + "    <8>," + Environment.NewLine
                + "    <9>," + Environment.NewLine
                + "    <10>," + Environment.NewLine
                + "    ..." + Environment.NewLine
                + "]", result);
        }

        [Test]
        public void CollectionDoesNotContain_IncludesItemType()
        {
            string result = sut.DoesNotContain(1, Enumerable.Empty<int>(), "foo");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should contain foo " + ExpectedExpression + Environment.NewLine
                + "                   <1>" + Environment.NewLine
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
            string result = sut.DoesNotContain(0, Enumerable.Empty<int>(), message: "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void CollectionDoesNotContainItems()
        {
            string result = sut.DoesNotContainItems(new[] { 1, 2 }, new[] { 1 });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should contain " + ExpectedExpression + Environment.NewLine
                + "but was missing item 1 <2>" + Environment.NewLine
                + "and was [<1>]", result);
        }

        [Test]
        public void CollectionDoesNotContainItems_ExpectedIsNewCollection_ExpectedExpressionNotIncluded()
        {
            expressionProvider.GetExpectedExpression().Returns("new[] { 1 }");

            string result = sut.DoesNotContainItems(new[] { 1 }, Enumerable.Empty<int>());

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should contain expected item 0 <1>" + Environment.NewLine
                + "but was empty.", result);
        }

        [Test]
        public void CollectionDoesNotContainItems_IncludesMessage()
        {
            string result = sut.DoesNotContainItems(new[] { 1 }, Enumerable.Empty<int>(), "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
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
            string result = sut.Contains(new FakeObject("foo"), Enumerable.Empty<object>());

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "shouldn't contain " + ExpectedExpression + Environment.NewLine
                + "                  <foo>" + Environment.NewLine
                + "but was empty.", result);
        }

        [Test]
        public void CollectionContains_IncludesItemType()
        {
            string result = sut.Contains(new FakeObject("foo"), Enumerable.Empty<object>(), "bar");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "shouldn't contain bar " + ExpectedExpression + Environment.NewLine
                + "                      <foo>" + Environment.NewLine
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
            string result = sut.Contains((object)null, Enumerable.Empty<object>(), message: "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void CollectionOnlyContains()
        {
            string result = sut.OnlyContains(new[] { new FakeObject("foo") }, Enumerable.Empty<object>());

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should contain more than just " + ExpectedExpression + Environment.NewLine
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
            string result = sut.OnlyContains(new object[0], new object[0], "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void CollectionContainsItems()
        {
            string result = sut.Contains(new[] { 1 }, new[] { 2, 1 });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "shouldn't contain " + ExpectedExpression + Environment.NewLine
                + "but contained <1>" + Environment.NewLine
                + "and was [" + Environment.NewLine
                + "    <2>," + Environment.NewLine
                + "    <1>" + Environment.NewLine
                + "]" + Environment.NewLine
                + "Match at index 1.", result);
        }

        [Test]
        public void CollectionContainsItems_ExpectedIsNewCollection()
        {
            expressionProvider.GetExpectedExpression().Returns("new[] { 1 }");

            string result = sut.Contains(new[] { 1 }, new[] { 2, 1 });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "shouldn't contain <1>" + Environment.NewLine
                + "but was [" + Environment.NewLine
                + "    <2>," + Environment.NewLine
                + "    <1>" + Environment.NewLine
                + "]" + Environment.NewLine
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
            string result = sut.Contains(new[] { 1 }, new[] { 1 }, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void DoesNotOnlyContain_MissingItem()
        {
            string result = sut.DoesNotOnlyContain(new[] { 1 }, new[] { 2 });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should contain " + ExpectedExpression + Environment.NewLine
                + "but was missing item 0 <1>" + Environment.NewLine
                + "and was [<2>]", result);
        }

        [Test]
        public void DoesNotOnlyContain_ExpectedIsEmpty_NotEmptyMessage()
        {
            FakeObject[] enumerable = { new FakeObject("foo") };

            string result = sut.DoesNotOnlyContain(Enumerable.Empty<object>(), enumerable);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be empty" + Environment.NewLine
                + "but had 1 element: <foo>", result);
        }

        [Test]
        public void DoesNotOnlyContain_ExtraItem_ContainsExtraItemMessage()
        {
            string result = sut.DoesNotOnlyContain(new[] { 1 }, new[] { 1, 2 });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should only contain " + ExpectedExpression + Environment.NewLine
                + "but also contains [<2>]", result);
        }

        [Test]
        public void ContainsExtraItem()
        {
            string result = sut.ContainsExtraItem(new[] { 1 }, new[] { 1, 2 });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should only contain " + ExpectedExpression + Environment.NewLine
                + "but also contains [<2>]", result);
        }

        [Test]
        public void ContainsExtraItem_ExpectedIsNewCollection_ExpectedExpressionNotIncluded()
        {
            expressionProvider.GetExpectedExpression().Returns("new[] { 1 }");

            string result = sut.ContainsExtraItem(new[] { 1 }, new[] { 1, 2 });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should only contain [<1>]" + Environment.NewLine
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
            string result = sut.DoesNotOnlyContain(new[] { 1 }, new[] { 1, 2 }, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void ContainsDuplicate()
        {
            string result = sut.ContainsDuplicate(new[] { 1, 2, 1, 1 }, "foo");

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
            string result = sut.DoNotMatch(new[] { 1, 2, 3 }, new[] { 1, 3, 2 }, Compare.ObjectsAreEqual);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "doesn't match " + ExpectedExpression + ". Differs at index 1." + Environment.NewLine
                + "should match <2>" + Environment.NewLine
                + "but was      <3>", result);
        }

        [Test]
        public void DoNotMatch_ExpectedIsNewCollection_ExpectedExpressionNotIncluded()
        {
            expressionProvider.GetExpectedExpression().Returns("new List<int>() { 1, 2, 3 }");

            string result = sut.DoNotMatch(new[] { 1, 2, 3 }, new[] { 1, 3, 2 }, Compare.ObjectsAreEqual);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "differs at index 1." + Environment.NewLine
                + "should match <2>" + Environment.NewLine
                + "but was      <3>", result);
        }

        [Test]
        public void DoNotMatch_LengthMismatch()
        {
            string result = sut.DoNotMatch(new[] { 1, 2 }, Enumerable.Empty<int>(), Compare.ObjectsAreEqual);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "doesn't match " + ExpectedExpression + '.' + Environment.NewLine
                + "should have 2 elements" + Environment.NewLine
                + "but was empty.", result);
        }

        [Test]
        public void DoNotMatch_Predicate()
        {
            string result = sut.DoNotMatch(new[] { 1, 2, 3 }, new[] { "1", "3", "2" }, (s, i) => s == i.ToString(CultureInfo.InvariantCulture));

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "doesn't match " + ExpectedExpression + ". "
                + "Differs at index 1." + Environment.NewLine
                + "should match <2>" + Environment.NewLine
                + "but was      \"3\"", result);
        }

        [Test]
        public void DoNotMatch_OnlyEnumeratesOnce()
        {
            TestEnumerable<int> expected = MakeEnumerable(1);
            TestEnumerable<int> actual = MakeEnumerable(2);

            sut.DoNotMatch(expected, actual, Compare.ObjectsAreEqual);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void DoNotMatch_IncludesMessage()
        {
            string result = sut.DoNotMatch(new[] { 1 }, new[] { 2 }, Compare.ObjectsAreEqual, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void ItemsNotSame_ToStringsItemsAtFirstDifference()
        {
            FakeObject same = new FakeObject("foo");

            string result = sut.ItemsNotSame(new[] { same, new FakeObject("bar") }, new[] { same, new FakeObject("baz") });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "differs at index 1." + Environment.NewLine
                + "should be instance <bar>" + Environment.NewLine
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
            string result = sut.ItemsNotSame(new[] { new object() }, new[] { new object() }, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void CollectionDoesNotStartWith_ActualIsShorterThanExpectedStart()
        {
            string result = sut.DoesNotStartWith(new[] { 1, 2 }, new[] { 1 }, Equals, "foo");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should have at least 2 elements" + Environment.NewLine
                + "but had 1 element: <1>" + Environment.NewLine
                + "foo", result);
        }

        [Test]
        public void CollectionDoesNotStartWith_ActualIsLongerThanExpectedStart()
        {
            string result = sut.DoesNotStartWith(new[] { 1, 2 }, new[] { 1, 3, 2 }, Equals, "foo");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "differs at index 1." + Environment.NewLine
                + "should be <2>" + Environment.NewLine
                + "but was   <3>" + Environment.NewLine
                + "foo", result);
        }

        [Test]
        public void CollectionDoesNotStartWith_OnlyEnumeratesOnce()
        {
            TestEnumerable<int> actual = MakeEnumerable(1);
            TestEnumerable<int> expected = MakeEnumerable(2);

            sut.DoesNotStartWith(expected, actual, Compare.ObjectsAreEqual);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void CollectionDoesNotEndWith_ActualIsShorterThanExpectedEnd()
        {
            string result = sut.DoesNotEndWith(new[] { 1, 2 }, new[] { 1 }, Equals, "foo");

            Assert.AreEqual($@"{ActualExpression}
should have at least 2 elements
but had 1 element: <1>
foo", result);
        }

        [Test]
        public void CollectionDoesNotEndWith_ActualIsLongerThanExpectedEnd()
        {
            string result = sut.DoesNotEndWith(new[] { 1, 2 }, new[] { 1, 3, 2 }, Equals, "foo");

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

            sut.DoesNotEndWith(expected, actual, Compare.ObjectsAreEqual);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void TreesDoNotMatch_NonMatchingNodes()
        {
            string result = sut.TreesDoNotMatch(new[] { 1, 2.Node(21, 22) }, new[] { 1, 2.Node(21, 23) }, n => n, TestNodesMatch);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "doesn't match " + ExpectedExpression + "." + Environment.NewLine
                + "Differs at root -> 2, child index 1." + Environment.NewLine
                + "should be <22>" + Environment.NewLine
                + "but was   <23>", result);
        }

        [Test]
        public void TreesDoNotMatch_EmptyActual()
        {
            string result = sut.TreesDoNotMatch(new[] { 1.Node(11) }, new[] { 1.Node() }, n => n, TestNodesMatch);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "doesn't match " + ExpectedExpression + "." + Environment.NewLine
                + "root -> 1 node should have 1 child" + Environment.NewLine
                + "but was empty.", result);
        }

        [Test]
        public void TreesDoNotMatch_SingleNodeActual()
        {
            string result = sut.TreesDoNotMatch(new TestNode<int>[] { 1, 2 }, new[] { 3 }, i => Enumerable.Empty<int>(), Compare.ObjectsAreEqual);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "doesn't match " + ExpectedExpression + "." + Environment.NewLine
                + "root node should have 2 children" + Environment.NewLine
                + "but had 1 child: <3>", result);
        }

        [Test]
        public void TreesDoNotMatch_MultipleNodeActual()
        {
            string result = sut.TreesDoNotMatch(new TestNode<int>[] { 1 }, new[] { 2, 3 }, i => Enumerable.Empty<int>(), Compare.ObjectsAreEqual);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "doesn't match " + ExpectedExpression + "." + Environment.NewLine
                + "root node should have 1 child" + Environment.NewLine
                + "but had 2 children: [" + Environment.NewLine
                + "    <2>," + Environment.NewLine
                + "    <3>" + Environment.NewLine
                + "]", result);
        }

        [Test]
        public void TreesDoNotMatch_ChildLengthMismatch_IncludesMessage()
        {
            string result = sut.TreesDoNotMatch(new TestNode<int>[] { 1 }, Enumerable.Empty<int>(), NoChildren, Compare.ObjectsAreEqual, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void TreesDoNotMatch_NonMatchingNode_IncludesMessage()
        {
            string result = sut.TreesDoNotMatch(new TestNode<int>[] { 1 }, new[] { 2 }, NoChildren, Compare.ObjectsAreEqual, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
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
            string result = sut.LengthMismatch(2, Enumerable.Empty<object>());

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should have 2 elements" + Environment.NewLine
                + "but was empty.", result);
        }

        [Test]
        public void LengthMismatch_SingleElementExpected()
        {
            string result = sut.LengthMismatch(1, Enumerable.Empty<object>());

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should have 1 element" + Environment.NewLine
                + "but was empty.", result);
        }

        [Test]
        public void LengthMismatch_SingleElement()
        {
            string result = sut.LengthMismatch(2, new[] { new FakeObject("foo") });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should have 2 elements" + Environment.NewLine
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
            string result = sut.LengthMismatch(1, Enumerable.Empty<object>(), "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void StringDoesNotContain()
        {
            string result = sut.DoesNotContain("bar", "foo");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should contain " + ExpectedExpression + Environment.NewLine
                + "               \"bar\"" + Environment.NewLine
                + "but was        \"foo\"", result);
        }

        [Test]
        public void StringDoesNotContain_ActualIsLong_EndOfActualClipped()
        {
            string result = sut.DoesNotContain("foo",
                "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should contain " + ExpectedExpression + Environment.NewLine
                + "               \"foo\"" + Environment.NewLine
                + "but was        \"0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijk...\"", result);
        }

        [Test]
        public void StringDoesNotContain_IncludesMessage()
        {
            string result = sut.DoesNotContain(string.Empty, string.Empty, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void StringContains()
        {
            string result = sut.Contains("bar", "foobarbaz");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "shouldn't contain " + ExpectedExpression + Environment.NewLine
                + "                  \"bar\"" + Environment.NewLine
                + "but was           \"foobarbaz\"" + Environment.NewLine
                 + "                      ^" + Environment.NewLine
                + "Match at index 3.", result);
        }

        [Test]
        public void StringContains_IncludesMessage()
        {
            string result = sut.Contains("bar", "foobarbaz", "qux");

            StringAssert.EndsWith(Environment.NewLine + "qux", result);
        }

        [Test]
        public void DoesNotStartWith()
        {
            string result = sut.DoesNotStartWith("foo", "bar");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should start with " + ExpectedExpression + Environment.NewLine
                + "                  \"foo\"" + Environment.NewLine
                + "but starts with   \"bar\"", result);
        }

        [Test]
        public void DoesNotStartWith_ActualIsLong_EndOfActualClipped()
        {
            string result = sut.DoesNotStartWith("foo",
                "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should start with " + ExpectedExpression + Environment.NewLine
                + "                  \"foo\"" + Environment.NewLine
                + "but starts with   \"0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijk...\"", result);
        }

        [Test]
        public void DoesNotStartWith_IncludesMessage()
        {
            string result = sut.DoesNotStartWith(string.Empty, string.Empty, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void DoesNotEndWith()
        {
            string result = sut.DoesNotEndWith("foo", "bar");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should end with " + ExpectedExpression + Environment.NewLine
                + "                \"foo\"" + Environment.NewLine
                + "but ends with   \"bar\"", result);
        }

        [Test]
        public void DoesNotEndWith_ActualIsLong_StartOfActualClipped()
        {
            string result = sut.DoesNotEndWith("foo",
                "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should end with " + ExpectedExpression + Environment.NewLine
                + "                \"foo\"" + Environment.NewLine
                + "but ends with   \"...fghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz\"", result);
        }

        [Test]
        public void DoesNotEndWith_IncludesMessage()
        {
            string result = sut.DoesNotEndWith(string.Empty, string.Empty, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void NoException_SimpleFunction()
        {
            Expression<Action> function = () => "".Trim();
            string result = sut.NoException(typeof(InvalidOperationException), function);
            Assert.AreEqual("\"\".Trim()" + Environment.NewLine
                + "should throw <InvalidOperationException>" + Environment.NewLine
                + "but didn't throw at all.", result);
        }

        [Test]
        public void NoException_ClosureObjectMethod()
        {
            string str = "";
            Expression<Action> function = () => str.Trim();
            string result = sut.NoException(typeof(InvalidOperationException), function);
            Assert.AreEqual("str.Trim()" + Environment.NewLine
               + "should throw <InvalidOperationException>" + Environment.NewLine
               + "but didn't throw at all.", result);
        }

        [Test]
        public void NoException_NoFunctionExpression()
        {
            string result = sut.NoException(typeof(InvalidOperationException));

            Assert.AreEqual($@"{ActualExpression}
should throw <InvalidOperationException>
but didn't throw at all.", result);
        }

        [Test]
        public void NoException_IncludesMessage()
        {
            Expression<Action> function = () => "".Trim();
            string result = sut.NoException(typeof(InvalidOperationException), function, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void WrongException_SimpleFunction()
        {
            Expression<Action> function = () => "".Trim();
            string result = sut.WrongException(typeof(InvalidOperationException), typeof(Exception), function);
            Assert.AreEqual("\"\".Trim()" + Environment.NewLine
                + "should throw <InvalidOperationException>" + Environment.NewLine
                + "but threw    <Exception>", result);
        }

        [Test]
        public void WrongException_ClosureObjectMethod()
        {
            string str = "";
            Expression<Action> function = () => str.Trim();
            string result = sut.WrongException(typeof(InvalidOperationException), typeof(Exception), function);
            Assert.AreEqual("str.Trim()" + Environment.NewLine
                + "should throw <InvalidOperationException>" + Environment.NewLine
                + "but threw    <Exception>", result);
        }

        [Test]
        public void WrongException_NoFunctionExpression()
        {
            string result = sut.WrongException(typeof(InvalidOperationException), typeof(Exception));

            Assert.AreEqual($@"{ActualExpression}
should throw <InvalidOperationException>
but threw    <Exception>", result);
        }

        [Test]
        public void WrongException_IncludesMessage()
        {
            Expression<Action> function = () => "".Trim();
            string result = sut.WrongException(typeof(InvalidOperationException), typeof(Exception), function, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void NotGreaterThan()
        {
            string result = sut.NotGreaterThan(2, 1);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be greater than " + ExpectedExpression + Environment.NewLine
                + "                       <2>" + Environment.NewLine
                + "but was                <1>", result);
        }

        [Test]
        public void NotGreaterThan_IncludesMessage()
        {
            string result = sut.NotGreaterThan(2, 1, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void NotLessThan()
        {
            string result = sut.NotLessThan(1, 2);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be less than " + ExpectedExpression + Environment.NewLine
                + "                    <1>" + Environment.NewLine
                + "but was             <2>", result);
        }

        [Test]
        public void NotLessThan_IncludesMessage()
        {
            string result = sut.NotLessThan(1, 2, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void DoesNotMatch()
        {
            string result = sut.DoesNotMatch(new Regex("foo"), "bar");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should match " + ExpectedExpression + Environment.NewLine
                + "             /foo/" + Environment.NewLine
                + "but was \"bar\"", result);
        }

        [Test]
        public void DoesNotMatch_IncludesMessage()
        {
            string result = sut.DoesNotMatch(new Regex(""), "", "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void Matches()
        {
            string result = sut.Matches(new Regex("foo"), "bar");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "shouldn't match " + ExpectedExpression + Environment.NewLine
                + "                /foo/" + Environment.NewLine
                + "but was \"bar\"", result);
        }

        [Test]
        public void Matches_IncludesMessage()
        {
            string result = sut.Matches(new Regex(""), "", "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void TimedOut()
        {
            string result = sut.TaskTimedOut(TimeSpan.FromMilliseconds(1), "foo");

            Assert.AreEqual($@"{ActualExpression}
timed out after 1ms.
foo", result);
        }

        private static TestEnumerable<T> MakeEnumerable<T>(params T[] items) => new TestEnumerable<T>(items);
    }
}
