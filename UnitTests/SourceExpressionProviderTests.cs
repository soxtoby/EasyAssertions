using System;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class SourceExpressionProviderTests
    {
        [Test]
        public void ResultOnSameLine()
        {
            TestClass expectedExpression = new TestClass(12);
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => expectedExpression.ShouldEqual(new TestClass(2)));
            Assert.AreEqual("expectedExpression" + Environment.NewLine
                + "should be <(2)>" + Environment.NewLine
                + "but was   <(12)>", result.Message);
        }

        [Test]
        public void ResultOnPreviousLine()
        {
            TestClass expectedExpression = new TestClass(12);
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => expectedExpression
                .ShouldEqual(new TestClass(2)));
            Assert.AreEqual("expectedExpression" + Environment.NewLine
                + "should be <(2)>" + Environment.NewLine
                + "but was   <(12)>", result.Message);
        }

        [Test]
        public void MultiAssertionMessage_TakesExpressionFromFirstAssertion()
        {
            TestClass expectedExpression = new TestClass(12);
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                expectedExpression.ShouldBeA<TestClass>()
                    .And.ShouldEqual(new TestClass(2)));

            Assert.AreEqual("expectedExpression" + Environment.NewLine
                + "should be <(2)>" + Environment.NewLine
                + "but was   <(12)>", result.Message);
        }

        [Test]
        public void ContinuedAssertion_CombinesExpressions()
        {
            TestClass expectedExpression = new TestClass(12);
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                expectedExpression.ShouldBeA<TestClass>()
             .And.Value.ShouldEqual(2));

            Assert.AreEqual("expectedExpression.Value" + Environment.NewLine
                + "should be <2>" + Environment.NewLine
                + "but was   <12>", result.Message);
        }

        private class TestClass
        {
            public readonly int Value;

            public TestClass(int value)
            {
                Value = value;
            }

            public override bool Equals(object obj)
            {
                TestClass other = obj as TestClass;
                return other != null
             && other.Value == Value;
            }

            public override int GetHashCode()
            {
                return Value;
            }

            public override string ToString()
            {
                return string.Format("({0})", Value);
            }
        }
    }
}