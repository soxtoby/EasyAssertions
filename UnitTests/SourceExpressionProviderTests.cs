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
            TestClass actualExpression = new TestClass(12);
            TestClass expectedExpression = new TestClass(2);
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actualExpression.ShouldBe(expectedExpression));
            Assert.AreEqual("actualExpression" + Environment.NewLine
                + "should be expectedExpression" + Environment.NewLine
                + "          <(2)>" + Environment.NewLine
                + "but was   <(12)>", result.Message);
        }

        [Test]
        public void ResultOnPreviousLine()
        {
            TestClass actualExpression = new TestClass(12);
            TestClass expectedExpression = new TestClass(2);
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actualExpression
                .ShouldBe(expectedExpression));
            Assert.AreEqual("actualExpression" + Environment.NewLine
                + "should be expectedExpression" + Environment.NewLine
                + "          <(2)>" + Environment.NewLine
                + "but was   <(12)>", result.Message);
        }

        [Test]
        public void MultiAssertionMessage_TakesExpressionFromFirstAssertion()
        {
            TestClass actualExpression = new TestClass(12);
            TestClass expectedExpression = new TestClass(2);
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                actualExpression.ShouldBeA<TestClass>()
                    .And.ShouldBe(expectedExpression));

            Assert.AreEqual("actualExpression" + Environment.NewLine
                + "should be expectedExpression" + Environment.NewLine
                + "          <(2)>" + Environment.NewLine
                + "but was   <(12)>", result.Message);
        }

        [Test]
        public void ContinuedAssertion_CombinesChainedExpressions()
        {
            TestClass actualExpression = new TestClass(12);
            int expectedExpression = 2;

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                actualExpression.ShouldBeA<TestClass>()
                    .And.Value.ShouldBe(expectedExpression));

            Assert.AreEqual("actualExpression.Value" + Environment.NewLine
                + "should be expectedExpression" + Environment.NewLine
                + "          <2>" + Environment.NewLine
                + "but was   <12>", result.Message);
        }

        [Test]
        public void ExpressionInsideNestedAssertion_CombinesOuterAndInnerExpressions()
        {
            TestClass actualExpression = new TestClass(12);
            int expectedExpression = 2;

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                actualExpression.Assert(a => a.Value.ShouldBe(expectedExpression)));

            Assert.AreEqual("actualExpression.Value" + Environment.NewLine
                + "should be expectedExpression" + Environment.NewLine
                + "          <2>" + Environment.NewLine
                + "but was   <12>", result.Message);
        }

        [Test]
        public void ExpressionSecondInsideNestedAssertion_CombinesOuterAndInnerExpressions()
        {
            TestClass actualExpression = new TestClass(12);
            int expectedExpression = 2;

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                actualExpression.Assert(tc =>
                    {
                        tc.Value.ShouldBe(12);
                        tc.Value.ShouldBe(expectedExpression);
                    }));

            Assert.AreEqual("actualExpression.Value" + Environment.NewLine
                + "should be expectedExpression" + Environment.NewLine
                + "          <2>" + Environment.NewLine
                + "but was   <12>", result.Message);
        }

        [Test]
        public void ExpressionTwoLevelsIn_CombinesOuterMiddleAndInnerExpressions()
        {
            TestClass actualExpression = new TestClass(12);
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                actualExpression
                    .Assert(a => a.Value.ShouldBe(12)
                        .And(b => b.ToString().ShouldBe("2"))));

            StringAssert.StartsWith("actualExpression.Value.ToString()" + Environment.NewLine, result.Message);
        }

        [Test]
        public void ExpressionAfterNestedAssertion_CombinesChainedExpressions()
        {
            TestClass actualExpression = new TestClass(12);
            int expectedExpression = 2;

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                actualExpression
                    .Assert(tc => tc.ShouldBeA<TestClass>())
                    .And.Value.ShouldBe(expectedExpression));

            Assert.AreEqual("actualExpression.Value" + Environment.NewLine
                + "should be expectedExpression" + Environment.NewLine
                + "          <2>" + Environment.NewLine
                + "but was   <12>", result.Message);
        }

        [Test]
        public void ExpressionInsideIndexedAssertion_IncludesIndex()
        {
            TestClass[] actualExpression = new[] { null, new TestClass(12) };
            int expectedExpression = 2;

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                actualExpression.ItemsSatisfy(
                    tc => { },
                    tc => tc.Value.ShouldBe(expectedExpression)));

            Assert.AreEqual("actualExpression[1].Value" + Environment.NewLine
                + "should be expectedExpression" + Environment.NewLine
                + "          <2>" + Environment.NewLine
                + "but was   <12>", result.Message);
        }

        [Test]
        public void ExpressionAfterIndexedAssertions_CombinesChainedExpressions()
        {
            TestClass[] actualExpression = new[] { new TestClass(12) };
            TestClass expectedExpression = new TestClass(2);

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                actualExpression
                    .ItemsSatisfy(tc => { })
                    .And.Single().ShouldBe(expectedExpression));

            Assert.AreEqual("actualExpression.Single()" + Environment.NewLine
                + "should be expectedExpression" + Environment.NewLine
                + "          <(2)>" + Environment.NewLine
                + "but was   <(12)>", result.Message);
        }

        [Test]
        public void SeparateExpressionAfterIndexedAssertion()
        {
            TestClass actualExpression = new TestClass(12);
            TestClass expectedExpression = new TestClass(2);

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                {
                    new[] { 1 }
                        .ItemsSatisfy(i => { });

                    actualExpression
                        .ShouldBe(expectedExpression);
                });

            Assert.AreEqual("actualExpression" + Environment.NewLine
                + "should be expectedExpression" + Environment.NewLine
                + "          <(2)>" + Environment.NewLine
                + "but was   <(12)>", result.Message);
        }

        [Test]
        public void AssertionNestsAnotherAssertionInAction_TakesExpressionFromOuterAssertion()
        {
            object actualExpression = null;

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actualExpression.TestActionAssert());

            StringAssert.StartsWith("actualExpression" + Environment.NewLine, result.Message);
        }

        [Test]
        public void AssertionNestsAnotherAssertionInFunc_TakesExpressionFromOuterAssertion()
        {
            object actualExpression = null;

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actualExpression.TestFuncAssert());

            StringAssert.StartsWith("actualExpression" + Environment.NewLine, result.Message);
        }

        [Test]
        public void SameAssertionCalledTwice()
        {
            AssertMethod("foo");
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => AssertMethod("bar"));

            StringAssert.StartsWith("foo", result.Message);
        }

        private static void AssertMethod(string foo)
        {
            foo.ShouldBe("foo");
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
        public static Actual<object> TestActionAssert(this object actual)
        {
            return actual.RegisterAssert(a => { a.ShouldNotBeNull(); });
        }

        public static Actual<object> TestFuncAssert(this object actual)
        {
            return actual.RegisterAssert(a => a.ShouldNotBeNull());
        }
    }
}