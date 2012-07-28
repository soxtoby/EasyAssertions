using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using NSubstitute;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class DefaultFailureMessageFormatterTests
    {
        private TestExpressionProvider expressionProvider;
        private const string ActualExpression = "test expression";
        private const string ExpectedExpression = "expected expression";

        [SetUp]
        public void SetUp()
        {
            expressionProvider = Substitute.For<TestExpressionProvider>();
            expressionProvider.GetActualExpression().Returns(ActualExpression);
            expressionProvider.GetExpectedExpression().Returns(ExpectedExpression);
            EasyAssertions.TestExpression.OverrideProvider(expressionProvider);
        }

        [TearDown]
        public void TearDown()
        {
            EasyAssertions.TestExpression.DefaultProvider();
        }

        [Test]
        public void NotEqual_Objects_ToStringed()
        {
            string result = DefaultFailureMessageFormatter.Instance.NotEqual(new FakeObject("foo"), new FakeObject("bar"));
            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be " + ExpectedExpression + Environment.NewLine
                + "          <foo>" + Environment.NewLine
                + "but was   <bar>", result);
        }

        [Test]
        public void NotEqual_ExpectedIsLiteral_ExpectedExpressionNotIncluded()
        {
            expressionProvider.GetExpectedExpression().Returns("1");

            string result = DefaultFailureMessageFormatter.Instance.NotEqual(1, 2);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be <1>" + Environment.NewLine
                + "but was   <2>", result);
        }

        [Test]
        public void NotEqual_Objects_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.NotEqual(new FakeObject("foo"), new FakeObject("bar"), "baz");
            StringAssert.EndsWith(Environment.NewLine + "baz", result);
        }

        [Test]
        public void NotEqual_SingleLineStrings()
        {
            string result = DefaultFailureMessageFormatter.Instance.NotEqual("acd", "abc");
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
            string result = DefaultFailureMessageFormatter.Instance.NotEqual("abc\ndfe", "abc\ndef");
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
            string result = DefaultFailureMessageFormatter.Instance.NotEqual("0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz",
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
            string result = DefaultFailureMessageFormatter.Instance.NotEqual("0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz",
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
            string result = DefaultFailureMessageFormatter.Instance.NotEqual("0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789",
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

            string result = DefaultFailureMessageFormatter.Instance.NotEqual("acd", "abc");
            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be \"acd\"" + Environment.NewLine
                + "but was   \"abc\"" + Environment.NewLine
                 + "            ^" + Environment.NewLine
                + "Difference at index 1.", result);
        }

        [Test]
        public void StringsNotEqual_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.NotEqual("acd", "abc", "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void AreEqual_ObjectsToStringed()
        {
            string result = DefaultFailureMessageFormatter.Instance.AreEqual(new FakeObject("foo"), new FakeObject("bar"));

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should not be " + ExpectedExpression + Environment.NewLine
                + "              <foo>" + Environment.NewLine
                + "but was       <bar>", result);
        }

        [Test]
        public void AreEqual_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.AreEqual(null, null, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void IsNull()
        {
            string result = DefaultFailureMessageFormatter.Instance.IsNull();

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should not be null, but was.", result);
        }

        [Test]
        public void IsNull_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.IsNull("foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void NotSame_ObjectsToStringed()
        {
            FakeObject expected = new FakeObject("foo");
            FakeObject actual = new FakeObject("bar");

            string result = DefaultFailureMessageFormatter.Instance.NotSame(expected, actual);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                 + "should be instance " + ExpectedExpression + Environment.NewLine
                 + "                   <foo>" + Environment.NewLine
                 + "but was            <bar>", result);
        }

        [Test]
        public void NotSame_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.NotSame(new object(), new object(), "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void AreSame_ObjectToStringed()
        {
            string result = DefaultFailureMessageFormatter.Instance.AreSame(new FakeObject("foo"));

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "shouldn't be instance " + ExpectedExpression + Environment.NewLine
                + "                      <foo>", result);
        }

        [Test]
        public void AreSame_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.AreSame(null, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void NotEmpty_SingleElement()
        {
            FakeObject[] enumerable = new[] { new FakeObject("foo") };

            string result = DefaultFailureMessageFormatter.Instance.NotEmpty(enumerable);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be empty" + Environment.NewLine
                + "but had 1 element: <foo>", result);
        }

        [Test]
        public void NotEmpty_TwoElements()
        {
            FakeObject[] enumerable = new[] { new FakeObject("foo"), new FakeObject("bar") };

            string result = DefaultFailureMessageFormatter.Instance.NotEmpty(enumerable);

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

            string result = DefaultFailureMessageFormatter.Instance.NotEmpty(enumerable);

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
        public void NotEmpty_IncludesMessage()
        {
            FakeObject[] enumerable = new[] { new FakeObject(String.Empty) };

            string result = DefaultFailureMessageFormatter.Instance.NotEmpty(enumerable, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void IsEmpty()
        {
            string result = DefaultFailureMessageFormatter.Instance.IsEmpty();

            Assert.AreEqual(ActualExpression + Environment.NewLine + "should not be empty, but was.", result);
        }

        [Test]
        public void IsEmpty_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.IsEmpty("foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void CollectionDoesNotContain_EmptyCollection()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotContain(1, Enumerable.Empty<int>());

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should contain " + ExpectedExpression + Environment.NewLine
                + "               <1>" + Environment.NewLine
                + "but was empty.", result);
        }

        [Test]
        public void CollectionDoesNotContain_SingleItem()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotContain(1, new[] { 2 });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should contain " + ExpectedExpression + Environment.NewLine
                + "               <1>" + Environment.NewLine
                + "but was       [<2>]", result);
        }

        [Test]
        public void CollectionDoesNotContain_MultipleItems()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotContain(1, new[] { 2, 3 });

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
            string result = DefaultFailureMessageFormatter.Instance.DoesNotContain(0, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 });

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
        public void CollectionDoesNotContain_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotContain(0, Enumerable.Empty<int>(), "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void CollectionDoesNotContainItems()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotContainItems(new[] { 1, 2 }, new[] { 1 });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should contain " + ExpectedExpression + Environment.NewLine
                + "but was missing item 1 <2>" + Environment.NewLine
                + "and was [<1>]", result);
        }

        [Test]
        public void CollectionDoesNotContainItems_ExpectedIsNewCollection_ExpectedExpressionNotIncluded()
        {
            expressionProvider.GetExpectedExpression().Returns("new[] { 1 }");

            string result = DefaultFailureMessageFormatter.Instance.DoesNotContainItems(new[] { 1 }, Enumerable.Empty<int>());

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should contain expected item 0 <1>" + Environment.NewLine
                + "but was empty.", result);
        }

        [Test]
        public void CollectionDoesNotContainItems_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotContainItems(new[] { 1 }, Enumerable.Empty<int>(), "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void CollectionContains()
        {
            string result = DefaultFailureMessageFormatter.Instance.Contains(new[] { 1 }, new[] { 2, 1 });

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
        public void CollectionContains_ExpectedIsNewCollection()
        {
            expressionProvider.GetExpectedExpression().Returns("new[] { 1 }");

            string result = DefaultFailureMessageFormatter.Instance.Contains(new[] { 1 }, new[] { 2, 1 });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "shouldn't contain <1>" + Environment.NewLine
                + "but was [" + Environment.NewLine
                + "    <2>," + Environment.NewLine
                + "    <1>" + Environment.NewLine
                + "]" + Environment.NewLine
                + "Match at index 1.", result);
        }

        [Test]
        public void CollectionContains_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.Contains(new[] { 1 }, new[] { 1 }, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void DoesNotOnlyContain_MissingItem()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotOnlyContain(new[] { 1 }, new[] { 2 });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should contain " + ExpectedExpression + Environment.NewLine
                + "but was missing item 0 <1>" + Environment.NewLine
                + "and was [<2>]", result);
        }

        [Test]
        public void DoesNotOnlyContain_ExtraItem()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotOnlyContain(new[] { 1 }, new[] { 1, 2 });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should only contain " + ExpectedExpression + Environment.NewLine
                + "but also contains [<2>]", result);
        }

        [Test]
        public void DoesNotOnlyContain_ExtraItems_ExpectedIsNewCollection_ExpectedExpressionNotIncluded()
        {
            expressionProvider.GetExpectedExpression().Returns("new[] { 1 }");

            string result = DefaultFailureMessageFormatter.Instance.DoesNotOnlyContain(new[] { 1 }, new[] { 1, 2 });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should only contain [<1>]" + Environment.NewLine
                + "but also contains [<2>]", result);
        }

        [Test]
        public void DoesNotOnlyContain_ExpectedIsEmpty_NotEmptyMessage()
        {
            FakeObject[] enumerable = new[] { new FakeObject("foo") };

            string result = DefaultFailureMessageFormatter.Instance.DoesNotOnlyContain(Enumerable.Empty<object>(), enumerable);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should be empty" + Environment.NewLine
                + "but had 1 element: <foo>", result);
        }

        [Test]
        public void DoesNotOnlyContain_ExtraItems_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotOnlyContain(new[] { 1 }, new[] { 1, 2 }, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void DoNotMatch_NonMatchingCollections()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoNotMatch(new[] { 1, 2, 3 }, new[] { 1, 3, 2 }, Compare.ObjectsAreEqual);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "doesn't match " + ExpectedExpression + ". Differs at index 1." + Environment.NewLine
                + "should match <2>" + Environment.NewLine
                + "but was      <3>", result);
        }

        [Test]
        public void DoNotMatch_ExpectedIsNewCollection_ExpectedExpressionNotIncluded()
        {
            expressionProvider.GetExpectedExpression().Returns("new List<int>() { 1, 2, 3 }");

            string result = DefaultFailureMessageFormatter.Instance.DoNotMatch(new[] { 1, 2, 3 }, new[] { 1, 3, 2 }, Compare.ObjectsAreEqual);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "differs at index 1." + Environment.NewLine
                + "should match <2>" + Environment.NewLine
                + "but was      <3>", result);
        }

        [Test]
        public void DoNotMatch_LengthMismatch()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoNotMatch(new[] { 1, 2 }, Enumerable.Empty<int>(), Compare.ObjectsAreEqual);

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "doesn't match " + ExpectedExpression + '.' + Environment.NewLine
                + "should have 2 elements" + Environment.NewLine
                + "but was empty.", result);
        }

        [Test]
        public void DoNotMatch_Predicate()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoNotMatch(new[] { 1, 2, 3 }, new[] { "1", "3", "2" }, (s, i) => s == i.ToString(CultureInfo.InvariantCulture));

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "doesn't match " + ExpectedExpression + ". "
                + "Differs at index 1." + Environment.NewLine
                + "should match <2>" + Environment.NewLine
                + "but was      \"3\"", result);
        }

        [Test]
        public void DoNotMatch_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoNotMatch(new[] { 1 }, new[] { 2 }, Compare.ObjectsAreEqual, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void ItemsNotSame_ToStringsItemsAtFirstDifference()
        {
            FakeObject same = new FakeObject("foo");

            string result = DefaultFailureMessageFormatter.Instance.ItemsNotSame(new[] { same, new FakeObject("bar") }, new[] { same, new FakeObject("baz") });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "differs at index 1." + Environment.NewLine
                + "should be instance <bar>" + Environment.NewLine
                + "but was            <baz>", result);
        }

        [Test]
        public void ItemsNotSame_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.ItemsNotSame(new[] { new object() }, new[] { new object() }, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void LengthMismatch_EmptyEnumerable()
        {
            string result = DefaultFailureMessageFormatter.Instance.LengthMismatch(2, Enumerable.Empty<object>());

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should have 2 elements" + Environment.NewLine
                + "but was empty.", result);
        }

        [Test]
        public void LengthMismatch_SingleElementExpected()
        {
            string result = DefaultFailureMessageFormatter.Instance.LengthMismatch(1, Enumerable.Empty<object>());

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should have 1 element" + Environment.NewLine
                + "but was empty.", result);
        }

        [Test]
        public void LengthMismatch_SingleElement()
        {
            string result = DefaultFailureMessageFormatter.Instance.LengthMismatch(2, new[] { new FakeObject("foo") });

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should have 2 elements" + Environment.NewLine
                + "but had 1 element: <foo>", result);
        }

        [Test]
        public void LengthMismatch_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.LengthMismatch(1, Enumerable.Empty<object>(), "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void StringDoesNotContain()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotContain("bar", "foo");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should contain " + ExpectedExpression + Environment.NewLine
                + "               \"bar\"" + Environment.NewLine
                + "but was        \"foo\"", result);
        }

        [Test]
        public void StringDoesNotContain_ActualIsLong_EndOfActualClipped()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotContain("foo",
                "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should contain " + ExpectedExpression + Environment.NewLine
                + "               \"foo\"" + Environment.NewLine
                + "but was        \"0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijk...\"", result);
        }

        [Test]
        public void StringDoesNotContain_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotContain(string.Empty, string.Empty, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void StringContains()
        {
            string result = DefaultFailureMessageFormatter.Instance.Contains("bar", "foobarbaz");

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
            string result = DefaultFailureMessageFormatter.Instance.Contains("bar", "foobarbaz", "qux");

            StringAssert.EndsWith(Environment.NewLine + "qux", result);
        }

        [Test]
        public void DoesNotStartWith()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotStartWith("foo", "bar");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should start with " + ExpectedExpression + Environment.NewLine
                + "                  \"foo\"" + Environment.NewLine
                + "but starts with   \"bar\"", result);
        }

        [Test]
        public void DoesNotStartWith_ActualIsLong_EndOfActualClipped()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotStartWith("foo",
                "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should start with " + ExpectedExpression + Environment.NewLine
                + "                  \"foo\"" + Environment.NewLine
                + "but starts with   \"0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijk...\"", result);
        }

        [Test]
        public void DoesNotStartWith_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotStartWith(string.Empty, string.Empty, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void DoesNotEndWith()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotEndWith("foo", "bar");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should end with " + ExpectedExpression + Environment.NewLine
                + "                \"foo\"" + Environment.NewLine
                + "but ends with   \"bar\"", result);
        }

        [Test]
        public void DoesNotEndWith_ActualIsLong_StartOfActualClipped()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotEndWith("foo",
                "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz");

            Assert.AreEqual(ActualExpression + Environment.NewLine
                + "should end with " + ExpectedExpression + Environment.NewLine
                + "                \"foo\"" + Environment.NewLine
                + "but ends with   \"...fghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz\"", result);
        }

        [Test]
        public void DoesNotEndWith_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotEndWith(string.Empty, string.Empty, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void NoException_SimpleFunction()
        {
            Expression<Action> function = () => "".Trim();
            string result = DefaultFailureMessageFormatter.Instance.NoException(typeof(InvalidOperationException), function);
            Assert.AreEqual("\"\".Trim()" + Environment.NewLine
                + "should throw <InvalidOperationException>" + Environment.NewLine
                + "but didn't throw at all.", result);
        }

        [Test]
        public void NoException_ClosureObjectMethod()
        {
            string str = "";
            Expression<Action> function = () => str.Trim();
            string result = DefaultFailureMessageFormatter.Instance.NoException(typeof(InvalidOperationException), function);
            Assert.AreEqual("str.Trim()" + Environment.NewLine
               + "should throw <InvalidOperationException>" + Environment.NewLine
               + "but didn't throw at all.", result);
        }

        [Test]
        public void NoException_IncludesMessage()
        {
            Expression<Action> function = () => "".Trim();
            string result = DefaultFailureMessageFormatter.Instance.NoException(typeof(InvalidOperationException), function, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void WrongException_SimpleFunction()
        {
            Expression<Action> function = () => "".Trim();
            string result = DefaultFailureMessageFormatter.Instance.WrongException(typeof(InvalidOperationException), typeof(Exception), function);
            Assert.AreEqual("\"\".Trim()" + Environment.NewLine
                + "should throw <InvalidOperationException>" + Environment.NewLine
                + "but threw    <Exception>", result);
        }

        [Test]
        public void WrongException_ClosureObjectMethod()
        {
            string str = "";
            Expression<Action> function = () => str.Trim();
            string result = DefaultFailureMessageFormatter.Instance.WrongException(typeof(InvalidOperationException), typeof(Exception), function);
            Assert.AreEqual("str.Trim()" + Environment.NewLine
                + "should throw <InvalidOperationException>" + Environment.NewLine
                + "but threw    <Exception>", result);
        }

        [Test]
        public void WrongException_IncludesMessage()
        {
            Expression<Action> function = () => "".Trim();
            string result = DefaultFailureMessageFormatter.Instance.WrongException(typeof(InvalidOperationException), typeof(Exception), function, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        private class FakeObject
        {
            private readonly string toString;

            public FakeObject(string toString)
            {
                this.toString = toString;
            }

            public override string ToString()
            {
                return toString;
            }
        }
    }
}
