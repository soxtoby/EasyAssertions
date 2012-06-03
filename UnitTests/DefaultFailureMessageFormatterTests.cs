﻿using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class DefaultFailureMessageFormatterTests
    {
        private const string TestExpression = "test expression";

        [SetUp]
        public void SetUp()
        {
            TestExpressionProvider expressionProvider = Substitute.For<TestExpressionProvider>();
            expressionProvider.GetExpression().Returns(TestExpression);
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
            Assert.AreEqual(TestExpression + Environment.NewLine
                + "should be <foo>" + Environment.NewLine
                + "but was   <bar>", result);
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
            Assert.AreEqual(TestExpression + Environment.NewLine
                + "should be \"acd\"" + Environment.NewLine
                + "but was   \"abc\"" + Environment.NewLine
                 + "            ^" + Environment.NewLine
                + "Difference at index 1.", result);
        }

        [Test]
        public void NotEqual_Strings_DifferenceOnLineTwo()
        {
            string result = DefaultFailureMessageFormatter.Instance.NotEqual("abc\ndfe", "abc\ndef");
            Assert.AreEqual(TestExpression + Environment.NewLine
                + "should be \"abc\\ndfe\"" + Environment.NewLine
                + "but was   \"abc\\ndef\"" + Environment.NewLine
                  + "                 ^" + Environment.NewLine
                + "Difference at index 5.", result);
        }

        [Test]
        public void NotEqual_Strings_DifferenceFarAwayFromBeginning()
        {
            string result = DefaultFailureMessageFormatter.Instance.NotEqual("0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz",
                                                                             "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnpoqrstuvwxyz");
            Assert.AreEqual(TestExpression + Environment.NewLine
                + "should be \"...789abcdefghijklmnopqrstuvwxyz\"" + Environment.NewLine
                + "but was   \"...789abcdefghijklmnpoqrstuvwxyz\"" + Environment.NewLine
                 + "                               ^" + Environment.NewLine
                + "Difference at index 60.", result);
        }

        [Test]
        public void NotEqual_Strings_DifferenceFarAwayFromEnd()
        {
            string result = DefaultFailureMessageFormatter.Instance.NotEqual("0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz",
                                                                             "0123456789abdcefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz");
            Assert.AreEqual(TestExpression + Environment.NewLine
                + "should be \"0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijk...\"" + Environment.NewLine
                + "but was   \"0123456789abdcefghijklmnopqrstuvwxyz0123456789abcdefghijk...\"" + Environment.NewLine
                 + "                       ^" + Environment.NewLine
                + "Difference at index 12.", result);
        }

        [Test]
        public void NotEqual_Strings_DifferenceFarAwayFromBothEnds()
        {
            string result = DefaultFailureMessageFormatter.Instance.NotEqual("0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789",
                                                                             "0123456789abcdefghijkmlnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789");
            Assert.AreEqual(TestExpression + Environment.NewLine
                + "should be \"...456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijkl...\"" + Environment.NewLine
                + "but was   \"...456789abcdefghijkmlnopqrstuvwxyz0123456789abcdefghijkl...\"" + Environment.NewLine
                 + "                               ^" + Environment.NewLine
                + "Difference at index 21.", result);
        }

        [Test]
        public void NotEqual_Strings_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.NotEqual("acd", "abc", "foo");
            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void AreEqual_ObjectsToStringed()
        {
            string result = DefaultFailureMessageFormatter.Instance.AreEqual(new FakeObject("foo"), new FakeObject("bar"));

            Assert.AreEqual(TestExpression + Environment.NewLine
                + "should not be <foo>" + Environment.NewLine
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

            Assert.AreEqual(TestExpression + Environment.NewLine
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

            Assert.AreEqual(TestExpression + Environment.NewLine
                 + "should be instance <foo>" + Environment.NewLine
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

            Assert.AreEqual(TestExpression + Environment.NewLine
                + "shouldn't be instance <foo>", result);
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

            Assert.AreEqual(TestExpression + Environment.NewLine
                + "should be empty" + Environment.NewLine
                + "but had 1 element: <foo>", result);
        }

        [Test]
        public void NotEmpty_TwoElements()
        {
            FakeObject[] enumerable = new[] { new FakeObject("foo"), new FakeObject("bar") };

            string result = DefaultFailureMessageFormatter.Instance.NotEmpty(enumerable);

            Assert.AreEqual(TestExpression + Environment.NewLine
                + "should be empty" + Environment.NewLine
                + "but had 2 elements: [" + Environment.NewLine
                + "    <foo>," + Environment.NewLine
                + "    <bar>" + Environment.NewLine
                + "]", result);
        }

        [Test]
        public void NotEmpty_ManyItems_OnlyFirstThreeDisplayed()
        {
            FakeObject[] enumerable = new[] { new FakeObject("one"), new FakeObject("two"), new FakeObject("three"), new FakeObject("four") };

            string result = DefaultFailureMessageFormatter.Instance.NotEmpty(enumerable);

            Assert.AreEqual(TestExpression + Environment.NewLine
                + "should be empty" + Environment.NewLine
                + "but had 4 elements: [" + Environment.NewLine
                + "    <one>," + Environment.NewLine
                + "    <two>," + Environment.NewLine
                + "    <three>," + Environment.NewLine
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

            Assert.AreEqual(TestExpression + Environment.NewLine + "should not be empty, but was.", result);
        }

        [Test]
        public void IsEmpty_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.IsEmpty("foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void DoNotMatch_NonMatchingCollections()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoNotMatch(new[] { 1, 2, 3 }, new[] { 1, 3, 2 });

            Assert.AreEqual(TestExpression + " differs at index 1." + Environment.NewLine
                + "should be <2>" + Environment.NewLine
                + "but was   <3>", result);
        }

        [Test]
        public void DoNotMatch_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoNotMatch(new[] { 1 }, new[] { 2 }, "foo");
            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void LengthMismatch_EmptyEnumerable()
        {
            string result = DefaultFailureMessageFormatter.Instance.LengthMismatch(Enumerable.Empty<object>(), 2);

            Assert.AreEqual(TestExpression + Environment.NewLine
                + "should have 2 elements" + Environment.NewLine
                + "but was empty.", result);
        }

        [Test]
        public void LengthMismatch_SingleElementExpected()
        {
            string result = DefaultFailureMessageFormatter.Instance.LengthMismatch(Enumerable.Empty<object>(), 1);

            Assert.AreEqual(TestExpression + Environment.NewLine
                + "should have 1 element" + Environment.NewLine
                + "but was empty.", result);
        }

        [Test]
        public void LengthMismatch_SingleElement()
        {
            string result = DefaultFailureMessageFormatter.Instance.LengthMismatch(new[] { new FakeObject("foo") }, 2);

            Assert.AreEqual(TestExpression + Environment.NewLine
                + "should have 2 elements" + Environment.NewLine
                + "but had 1 element: <foo>", result);
        }

        [Test]
        public void LengthMismatch_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.LengthMismatch(Enumerable.Empty<object>(), 1, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void DoesNotContain()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotContain("bar", "foo");

            Assert.AreEqual(TestExpression + Environment.NewLine
                + "should contain \"bar\"" + Environment.NewLine
                + "but was        \"foo\"", result);
        }

        [Test]
        public void DoesNotContains_ActualIsLong_EndOfActualClipped()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotContain("foo",
                "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz");

            Assert.AreEqual(TestExpression + Environment.NewLine
                + "should contain \"foo\"" + Environment.NewLine
                + "but was        \"0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijk...\"", result);
        }

        [Test]
        public void DoesNotContain_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotContain(string.Empty, string.Empty, "foo");

            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void DoesNotEndWith()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotEndWith("foo", "bar");

            Assert.AreEqual(TestExpression + Environment.NewLine
                + "should end with \"foo\"" + Environment.NewLine
                + "but ends with   \"bar\"", result);
        }

        [Test]
        public void DoesNotEndWith_ActualIsLong_StartOfActualClipped()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoesNotEndWith("foo",
                "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz");

            Assert.AreEqual(TestExpression + Environment.NewLine
                + "should end with \"foo\"" + Environment.NewLine
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
            string result = DefaultFailureMessageFormatter.Instance.NoException(typeof(InvalidOperationException), () => "".Trim());
            Assert.AreEqual("\"\".Trim()" + Environment.NewLine
                + "should throw <InvalidOperationException>" + Environment.NewLine
                + "but didn't throw at all.", result);
        }

        [Test]
        public void NoException_ClosureObjectMethod()
        {
            string str = "";
            string result = DefaultFailureMessageFormatter.Instance.NoException(typeof(InvalidOperationException), () => str.Trim());
            Assert.AreEqual("str.Trim()" + Environment.NewLine
               + "should throw <InvalidOperationException>" + Environment.NewLine
               + "but didn't throw at all.", result);
        }

        [Test]
        public void NoException_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.NoException(typeof(InvalidOperationException), () => "".Trim(), "foo");
            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void WrongException_SimpleFunction()
        {
            string result = DefaultFailureMessageFormatter.Instance.WrongException(typeof(InvalidOperationException), typeof(Exception), () => "".Trim());
            Assert.AreEqual("\"\".Trim()" + Environment.NewLine
                + "should throw <InvalidOperationException>" + Environment.NewLine
                + "but threw    <Exception>", result);
        }

        [Test]
        public void WrongException_ClosureObjectMethod()
        {
            string str = "";
            string result = DefaultFailureMessageFormatter.Instance.WrongException(typeof(InvalidOperationException), typeof(Exception), () => str.Trim());
            Assert.AreEqual("str.Trim()" + Environment.NewLine
                + "should throw <InvalidOperationException>" + Environment.NewLine
                + "but threw    <Exception>", result);
        }

        [Test]
        public void WrongException_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.WrongException(typeof(InvalidOperationException), typeof(Exception), () => "".Trim(), "foo");
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