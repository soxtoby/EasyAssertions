using NUnit.Framework;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class SourceExpressionProviderTests
    {
        private SourceExpressionProvider sut;

        [SetUp]
        public void SetUp()
        {
            sut = SourceExpressionProvider.ForCurrentThread;
        }

        [Test]
        public void ResultOnSameLine()
        {
            TestClass actualExpression = new TestClass(12);
            TestClass expectedExpression = new TestClass(2);
            Assert.Throws<EasyAssertionException>(() => actualExpression.ShouldBe(expectedExpression));

            Assert.AreEqual("actualExpression", sut.GetActualExpression());
        }

        [Test]
        public void ResultOnPreviousLine()
        {
            TestClass actualExpression = new TestClass(12);
            TestClass expectedExpression = new TestClass(2);
            Assert.Throws<EasyAssertionException>(() => actualExpression
                .ShouldBe(expectedExpression));

            Assert.AreEqual("actualExpression", sut.GetActualExpression());
        }

        [Test]
        public void MulltiLineActual_AlignsIndents()
        {
            var actual = new { one = new { two = new { three = 3 } } };

            Assert.Throws<EasyAssertionException>(() => actual
                .one
                    .two
            .three
                .ShouldBe(1));

            Assert.AreEqual(@"actual
        .one
            .two
    .three", sut.GetActualExpression());
        }

        [Test]
        public void MultiAssertionMessage_TakesExpressionFromFirstAssertion()
        {
            TestClass actualExpression = new TestClass(12);
            TestClass expectedExpression = new TestClass(2);
            Assert.Throws<EasyAssertionException>(() =>
                actualExpression.ShouldBeA<TestClass>()
                    .And.ShouldBe(expectedExpression));

            Assert.AreEqual("actualExpression", sut.GetActualExpression());
        }

        [Test]
        public void ContinuedAssertion_CombinesChainedExpressions()
        {
            TestClass actualExpression = new TestClass(12);
            int expectedExpression = 2;

            Assert.Throws<EasyAssertionException>(() =>
                actualExpression.ShouldBeA<TestClass>()
                    .And.Value.ShouldBe(expectedExpression));

            Assert.AreEqual("actualExpression.Value", sut.GetActualExpression());
        }

        [Test]
        public void ExpressionInsideNestedAssertion_CombinesOuterAndInnerExpressions()
        {
            TestClass actualExpression = new TestClass(12);
            int expectedExpression = 2;

            Assert.Throws<EasyAssertionException>(() =>
                actualExpression.Assert(a => a.Value.ShouldBe(expectedExpression)));

            Assert.AreEqual("actualExpression.Value", sut.GetActualExpression());
        }

        [Test]
        public void ExpressionSecondInsideNestedAssertion_CombinesOuterAndInnerExpressions()
        {
            TestClass actualExpression = new TestClass(12);
            int expectedExpression = 2;

            Assert.Throws<EasyAssertionException>(() =>
                actualExpression.Assert(tc =>
                    {
                        tc.Value.ShouldBe(12);
                        tc.Value.ShouldBe(expectedExpression);
                    }));

            Assert.AreEqual("actualExpression.Value", sut.GetActualExpression());
        }

        [Test]
        public void ExpressionTwoLevelsIn_CombinesOuterMiddleAndInnerExpressions()
        {
            TestClass actualExpression = new TestClass(12);
            Assert.Throws<EasyAssertionException>(() =>
                actualExpression
                    .Assert(a => a.Value.ShouldBe(12)
                        .And(b => b.ToString().ShouldBe("2"))));

            Assert.AreEqual("actualExpression.Value.ToString()", sut.GetActualExpression());
        }

        [Test]
        public void ExpressionAfterNestedAssertion_CombinesChainedExpressions()
        {
            TestClass actualExpression = new TestClass(12);
            int expectedExpression = 2;

            Assert.Throws<EasyAssertionException>(() =>
                actualExpression
                    .Assert(tc => tc.ShouldBeA<TestClass>())
                    .And.Value.ShouldBe(expectedExpression));

            Assert.AreEqual("actualExpression.Value", sut.GetActualExpression());
        }

        [Test]
        public void ExpressionInsideIndexedAssertion_IncludesIndex()
        {
            TestClass[] actualExpression = { null, new TestClass(12) };
            int expectedExpression = 2;

            Assert.Throws<EasyAssertionException>(() =>
                actualExpression.ItemsSatisfy(
                    tc => { },
                    tc => tc.Value.ShouldBe(expectedExpression)));

            Assert.AreEqual("actualExpression[1].Value", sut.GetActualExpression());
        }

        [Test]
        public void ExpressionAfterIndexedAssertions_CombinesChainedExpressions()
        {
            TestClass[] actualExpression = { new TestClass(12) };
            TestClass expectedExpression = new TestClass(2);

            Assert.Throws<EasyAssertionException>(() =>
                actualExpression
                    .ItemsSatisfy(tc => { })
                    .And.Single().ShouldBe(expectedExpression));

            Assert.AreEqual("actualExpression.Single()", sut.GetActualExpression());
        }

        [Test]
        public void SeparateExpressionAfterIndexedAssertion()
        {
            TestClass actualExpression = new TestClass(12);
            TestClass expectedExpression = new TestClass(2);

            Assert.Throws<EasyAssertionException>(() =>
                {
                    new[] { 1 }
                        .ItemsSatisfy(i => { });

                    actualExpression
                        .ShouldBe(expectedExpression);
                });

            Assert.AreEqual("actualExpression", sut.GetActualExpression());
        }

        [Test]
        public void AssertionNestsAnotherAssertionInAction_TakesExpressionFromOuterAssertion()
        {
            object actualExpression = null;

            Assert.Throws<EasyAssertionException>(() => actualExpression.TestActionAssert());

            Assert.AreEqual("actualExpression", sut.GetActualExpression());
        }

        [Test]
        public void AssertionNestsAnotherAssertionInFunc_TakesExpressionFromOuterAssertion()
        {
            object actualExpression = null;

            Assert.Throws<EasyAssertionException>(() => actualExpression.TestFuncAssert());

            Assert.AreEqual("actualExpression", sut.GetActualExpression());
        }

        [Test, Ignore("After upgrade to .NET 4.0, compiled expression now has a source file")]
        public void StackFrameHasNoSource_IsIgnored()
        {
            Expression<Action> assert = () => "foo".ShouldBe("bar", null);    // Compiled expression has no source file

            Assert.Throws<EasyAssertionException>(() => assert.Compile()());

            Assert.AreEqual(string.Empty, sut.GetActualExpression());
        }

        [Test]
        public void SameAssertionCalledTwice()
        {
            const string foo = "foo";
            for (int i = 0; i < 2; i++)
                Assert.Throws<EasyAssertionException>(() => foo.ShouldBe("bar"));

            Assert.AreEqual("foo", sut.GetActualExpression());
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
                return $"({Value})";
            }
        }
    }

    static class TestAssertions
    {
        public static Actual<object> TestActionAssert(this object actual)
        {
            return actual.RegisterAssert(c => { actual.ShouldNotBeNull(); });
        }

        public static Actual<object> TestFuncAssert(this object actual)
        {
            return actual.RegisterAssert(c => actual.ShouldNotBeNull());
        }
    }
}