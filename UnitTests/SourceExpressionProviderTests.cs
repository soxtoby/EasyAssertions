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
                    .And(a => a.Value.ShouldBe(2)));

            Assert.AreEqual("expectedExpression.Value" + Environment.NewLine
                + "should be <2>" + Environment.NewLine
                + "but was   <12>", result.Message);
        }

        [Test]
        public void ExpressionSecondInsideNestedAssertion_CombinesOuterAndInnerExpressions()
        {
            TestClass expectedExpression = new TestClass(12);

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                expectedExpression.ShouldBe(new TestClass(12))
                    .And(tc =>
                        {
                            tc.Value.ShouldBe(12);
                            tc.Value.ShouldBe(2);
                        }));

            Assert.AreEqual("expectedExpression.Value" + Environment.NewLine
                + "should be <2>" + Environment.NewLine
                + "but was   <12>", result.Message);
        }

        [Test]
        public void ExpressionTwoLevelsIn_CombinesOuterMiddleAndInnerExpressions()
        {
            TestClass expectedExpression = new TestClass(12);
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                expectedExpression.ShouldBe(new TestClass(12))
                    .And(a => a.Value.ShouldBe(12)
                    .And(b => b.ToString().ShouldBe("2"))));

            StringAssert.StartsWith("expectedExpression.Value.ToString()" + Environment.NewLine, result.Message);
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

        [Test]
        public void AssertionNestsAnotherAssertion_TakesExpressionFromOuterAssertion()
        {
            object expectedExpression = null;

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => expectedExpression.TestAssert());

            StringAssert.StartsWith("expectedExpression" + Environment.NewLine, result.Message);
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

            public override string ToString()
            {
                return string.Format("({0})", Value);
            }
        }
    }

    static class TestAssertions
    {
        public static Actual<object> TestAssert(this object actual)
        {
            return actual.Assert(a => a.ShouldNotBeNull());
        }
    }
}