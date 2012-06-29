using System;
using System.Linq;
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
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => expectedExpression.ShouldBe(new TestClass(2)));
            Assert.AreEqual("expectedExpression" + Environment.NewLine
                + "should be <(2)>" + Environment.NewLine
                + "but was   <(12)>", result.Message);
        }

        [Test]
        public void ResultOnPreviousLine()
        {
            TestClass expectedExpression = new TestClass(12);
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => expectedExpression
                .ShouldBe(new TestClass(2)));
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
                    .And.ShouldBe(new TestClass(2)));

            Assert.AreEqual("expectedExpression" + Environment.NewLine
                + "should be <(2)>" + Environment.NewLine
                + "but was   <(12)>", result.Message);
        }

        [Test]
        public void ContinuedAssertion_CombinesChainedExpressions()
        {
            TestClass expectedExpression = new TestClass(12);
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                expectedExpression.ShouldBeA<TestClass>()
                    .And.Value.ShouldBe(2));

            Assert.AreEqual("expectedExpression.Value" + Environment.NewLine
                + "should be <2>" + Environment.NewLine
                + "but was   <12>", result.Message);
        }

        [Test]
        public void ExpressionInsideNestedAssertion_CombinesOuterAndInnerExpressions()
        {
            TestClass expectedExpression = new TestClass(12);
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                expectedExpression.ShouldBe(new TestClass(12))
                    .And(tc => tc.Value.ShouldBe(2)));

            Assert.AreEqual("expectedExpression.Value" + Environment.NewLine
                + "should be <2>" + Environment.NewLine
                + "but was   <12>", result.Message);
        }

        [Test]
        public void ExpressionAfterNestedAssertion_CombinesChainedExpressions()
        {
            TestClass expectedExpression = new TestClass(12);
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                expectedExpression.ShouldBeA<TestClass>()
                    .And(tc => tc.ShouldBeA<TestClass>())
                    .And.Value.ShouldBe(2));

            Assert.AreEqual("expectedExpression.Value" + Environment.NewLine
                + "should be <2>" + Environment.NewLine
                + "but was   <12>", result.Message);
        }

        [Test]
        public void ExpressionInsideIndexedAssertion_IncludesIndex()
        {
            TestClass[] expectedExpression = new[] { null, new TestClass(12) };
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                expectedExpression.ItemsSatisfy(
                    tc => { },
                    tc => tc.Value.ShouldBe(2)));

            Assert.AreEqual("expectedExpression[1].Value" + Environment.NewLine
                + "should be <2>" + Environment.NewLine
                + "but was   <12>", result.Message);
        }

        [Test]
        public void ExpressionAfterIndexedAssertions_CombinesChainedExpressions()
        {
            TestClass[] expectedExpression = new[] { new TestClass(12) };
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                expectedExpression
                    .ItemsSatisfy(tc => { })
                    .And.Single().ShouldBe(new TestClass(2)));

            Assert.AreEqual("expectedExpression.Single()" + Environment.NewLine
                + "should be <(2)>" + Environment.NewLine
                + "but was   <(12)>", result.Message);
        }

        [Test]
        public void SeparateExpressionAfterIndexedAssertion()
        {
            TestClass expectedExpression = new TestClass(12);
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                {
                    new[] { 1 }
                        .ItemsSatisfy(i => { });

                    expectedExpression
                        .ShouldBe(new TestClass(2));
                });

            Assert.AreEqual("expectedExpression" + Environment.NewLine
                + "should be <(2)>" + Environment.NewLine
                + "but was   <(12)>", result.Message);
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