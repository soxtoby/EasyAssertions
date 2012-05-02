using System;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class DefaultFailureMessageFormatterTests
    {
        [Test]
        public void NotEqual_Objects_ToStringed()
        {
            string result = DefaultFailureMessageFormatter.Instance.NotEqual(new FakeObject("foo"), new FakeObject("bar"));
            Assert.AreEqual(string.Format(
                  "Expected: <foo>{0}"
                + "Actual:   <bar>", Environment.NewLine), result);
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
            Assert.AreEqual(string.Format("Strings differ at index 1.{0}"
                + "Expected: \"acd\"{0}"
                + "Actual:   \"abc\"{0}"
                 + "            ^", Environment.NewLine), result);
        }

        [Test]
        public void NotEqual_Strings_DifferenceOnLineTwo()
        {
            string result = DefaultFailureMessageFormatter.Instance.NotEqual("abc\ndfe", "abc\ndef");
            Assert.AreEqual(string.Format("Strings differ at index 5.{0}"
                + "Expected: \"abc\\ndfe\"{0}"
                + "Actual:   \"abc\\ndef\"{0}"
                  + "                 ^", Environment.NewLine), result);
        }

        [Test]
        public void NotEqual_Strings_DifferenceFarAwayFromBeginning()
        {
            string result = DefaultFailureMessageFormatter.Instance.NotEqual("0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz",
                                                                          "0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnpoqrstuvwxyz");
            Assert.AreEqual(string.Format("Strings differ at index 60.{0}"
                + "Expected: \"...789abcdefghijklmnopqrstuvwxyz\"{0}"
                + "Actual:   \"...789abcdefghijklmnpoqrstuvwxyz\"{0}"
                 + "                               ^", Environment.NewLine), result);
        }

        [Test]
        public void NotEqual_Strings_DifferenceFarAwayFromEnd()
        {
            string result = DefaultFailureMessageFormatter.Instance.NotEqual("0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz",
                                                                          "0123456789abdcefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz");
            Assert.AreEqual(string.Format("Strings differ at index 12.{0}"
                + "Expected: \"0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijk...\"{0}"
                + "Actual:   \"0123456789abdcefghijklmnopqrstuvwxyz0123456789abcdefghijk...\"{0}"
                 + "                       ^", Environment.NewLine), result);
        }

        [Test]
        public void NotEqual_Strings_DifferenceFarAwayFromBothEnds()
        {
            string result = DefaultFailureMessageFormatter.Instance.NotEqual("0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789",
                                                                          "0123456789abcdefghijkmlnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789");
            Assert.AreEqual(string.Format("Strings differ at index 21.{0}"
                + "Expected: \"...456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijkl...\"{0}"
                + "Actual:   \"...456789abcdefghijkmlnopqrstuvwxyz0123456789abcdefghijkl...\"{0}"
                 + "                               ^", Environment.NewLine), result);
        }

        [Test]
        public void NotEqual_Strings_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.NotEqual("acd", "abc", "foo");
            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void NotSame_ObjectsToStringed()
        {
            FakeObject expected = new FakeObject("foo");
            FakeObject actual = new FakeObject("bar");

            string result = DefaultFailureMessageFormatter.Instance.NotSame(expected, actual);

            Assert.AreEqual("Not the same object." + Environment.NewLine
                 + "Expected: <foo>" + Environment.NewLine
                 + "Actual:   <bar>", result);
        }

        [Test]
        public void NotSame_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.NotSame(new object(), new object(), "foo");
            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void DoNotMatch_NonMatchingCollections()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoNotMatch(new[] { 1, 2, 3 }, new[] { 1, 3, 2 });

            Assert.AreEqual("Enumerables differ at index 1." + Environment.NewLine
                + "Expected: <2>" + Environment.NewLine
                + "Actual:   <3>", result);
        }

        [Test]
        public void DoNotMatch_IncludesMessage()
        {
            string result = DefaultFailureMessageFormatter.Instance.DoNotMatch(new[] { 1 }, new[] { 2 }, "foo");
            StringAssert.EndsWith(Environment.NewLine + "foo", result);
        }

        [Test]
        public void NoException_SimpleFunction()
        {
            string result = DefaultFailureMessageFormatter.Instance.NoException(typeof(InvalidOperationException), () => "".Trim());
            Assert.AreEqual("\"\".Trim() didn't throw." + Environment.NewLine
               + "Expected: <InvalidOperationException>", result);
        }

        [Test]
        public void NoException_ClosureObjectMethod()
        {
            string str = "";
            string result = DefaultFailureMessageFormatter.Instance.NoException(typeof(InvalidOperationException), () => str.Trim());
            Assert.AreEqual("str.Trim() didn't throw." + Environment.NewLine
               + "Expected: <InvalidOperationException>", result);
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
            Assert.AreEqual("Wrong exception type thrown by \"\".Trim()" + Environment.NewLine
                + "Expected: <InvalidOperationException>" + Environment.NewLine
                + "Actual:   <Exception>", result);
        }

        [Test]
        public void WrongException_ClosureObjectMethod()
        {
            string str = "";
            string result = DefaultFailureMessageFormatter.Instance.WrongException(typeof(InvalidOperationException), typeof(Exception), () => str.Trim());
            Assert.AreEqual("Wrong exception type thrown by str.Trim()" + Environment.NewLine
                + "Expected: <InvalidOperationException>" + Environment.NewLine
                + "Actual:   <Exception>", result);
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