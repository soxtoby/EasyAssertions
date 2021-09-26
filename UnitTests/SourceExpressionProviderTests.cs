using NUnit.Framework;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class SourceExpressionProviderTests
    {
        SourceExpressionProvider sut = null!;

        [SetUp]
        public void SetUp()
        {
            sut = SourceExpressionProvider.ForCurrentThread;
        }

        [Test]
        public void ResultOnSameLine()
        {
            var actualExpression = new TestClass(12);
            var expectedExpression = new TestClass(2);
            Assert.Throws<EasyAssertionException>(() => actualExpression.ShouldBe(expectedExpression));

            Assert.AreEqual(nameof(actualExpression), sut.GetActualExpression());
            Assert.AreEqual(nameof(expectedExpression), sut.GetExpectedExpression());
        }

        [Test]
        public void ResultOnPreviousLine()
        {
            var actualExpression = new TestClass(12);
            var expectedExpression = new TestClass(2);
            Assert.Throws<EasyAssertionException>(() => actualExpression
                .ShouldBe(expectedExpression));

            Assert.AreEqual(nameof(actualExpression), sut.GetActualExpression());
            Assert.AreEqual(nameof(expectedExpression), sut.GetExpectedExpression());
        }

        [Test]
        public void MultiLineActual_AlignsIndents()
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
        public void MultiAssertionMessage_TakesActualExpressionFromFirstAssertion()
        {
            var actualExpression = new TestClass(12);
            var expectedExpression = new TestClass(2);
            Assert.Throws<EasyAssertionException>(() =>
                actualExpression.ShouldBeA<TestClass>()
                    .And.ShouldBe(expectedExpression));

            Assert.AreEqual(nameof(actualExpression), sut.GetActualExpression());
            Assert.AreEqual(nameof(expectedExpression), sut.GetExpectedExpression());
        }

        [Test]
        public void ContinuedAssertion_CombinesChainedExpressions()
        {
            var actualExpression = new TestClass(12);
            var expectedExpression = 2;

            Assert.Throws<EasyAssertionException>(() =>
                actualExpression.ShouldBeA<TestClass>()
                    .And.Value.ShouldBe(expectedExpression));

            Assert.AreEqual($"{nameof(actualExpression)}.{nameof(actualExpression.Value)}", sut.GetActualExpression());
            Assert.AreEqual(nameof(expectedExpression), sut.GetExpectedExpression());
        }

        [Test]
        public void ExpressionInsideNestedAssertion_CombinesOuterAndInnerExpressions()
        {
            var actualExpression = new TestClass(12);
            var expectedExpression = 2;

            Assert.Throws<EasyAssertionException>(() =>
                actualExpression.Assert(a => a.Value.ShouldBe(expectedExpression)));

            Assert.AreEqual($"{nameof(actualExpression)}.{nameof(actualExpression.Value)}", sut.GetActualExpression());
            Assert.AreEqual(nameof(expectedExpression), sut.GetExpectedExpression());
        }

        [Test]
        public void ExpressionSecondInsideNestedAssertion_CombinesOuterAndInnerExpressions()
        {
            var actualExpression = new TestClass(12);
            var expectedExpression = 2;

            Assert.Throws<EasyAssertionException>(() =>
                actualExpression.Assert(tc =>
                    {
                        tc.Value.ShouldBe(12);
                        tc.Value.ShouldBe(expectedExpression);
                    }));

            Assert.AreEqual($"{nameof(actualExpression)}.{nameof(actualExpression.Value)}", sut.GetActualExpression());
            Assert.AreEqual(nameof(expectedExpression), sut.GetExpectedExpression());
        }

        [Test]
        public void ExpressionTwoLevelsIn_CombinesOuterMiddleAndInnerExpressions()
        {
            var actualExpression = new TestClass(12);
            var expectedExpression = "2";
            Assert.Throws<EasyAssertionException>(() =>
                actualExpression
                    .Assert(a => a.Value.ShouldBe(12)
                        .And(b => b.ToString().ShouldBe(expectedExpression))));

            Assert.AreEqual($"{nameof(actualExpression)}.{nameof(actualExpression.Value)}.ToString()", sut.GetActualExpression());
            Assert.AreEqual(nameof(expectedExpression), sut.GetExpectedExpression());
        }

        [Test]
        public void ExpressionAfterNestedAssertion_CombinesChainedExpressions()
        {
            var actualExpression = new TestClass(12);
            var expectedExpression = 2;

            Assert.Throws<EasyAssertionException>(() =>
                actualExpression
                    .Assert(tc => tc.ShouldBeA<TestClass>())
                    .And.Value.ShouldBe(expectedExpression));

            Assert.AreEqual($"{nameof(actualExpression)}.{nameof(actualExpression.Value)}", sut.GetActualExpression());
            Assert.AreEqual(nameof(expectedExpression), sut.GetExpectedExpression());
        }

        [Test]
        public void ExpressionInsideIndexedAssertion_IncludesIndex()
        {
            TestClass?[] actualExpression = { null, new TestClass(12) };
            var expectedExpression = 2;

            Assert.Throws<EasyAssertionException>(() =>
                actualExpression.ItemsSatisfy(
                    tc => { },
                    tc => tc.Value.ShouldBe(expectedExpression)));

            Assert.AreEqual($"{nameof(actualExpression)}[1].{nameof(TestClass.Value)}", sut.GetActualExpression());
            Assert.AreEqual(nameof(expectedExpression), sut.GetExpectedExpression());
        }

        [Test]
        public void ExpressionAfterIndexedAssertions_CombinesChainedExpressions()
        {
            TestClass[] actualExpression = { new TestClass(12) };
            var expectedExpression = new TestClass(2);

            Assert.Throws<EasyAssertionException>(() =>
                actualExpression
                    .ItemsSatisfy(tc => { })
                    .And.Single().ShouldBe(expectedExpression));

            Assert.AreEqual($"{nameof(actualExpression)}.Single()", sut.GetActualExpression());
            Assert.AreEqual(nameof(expectedExpression), sut.GetExpectedExpression());
        }

        [Test]
        public void SeparateExpressionAfterIndexedAssertion()
        {
            var actualExpression = new TestClass(12);
            var expectedExpression = new TestClass(2);

            Assert.Throws<EasyAssertionException>(() =>
                {
                    new[] { 1 }
                        .ItemsSatisfy(i => { });

                    actualExpression
                        .ShouldBe(expectedExpression);
                });

            Assert.AreEqual(nameof(actualExpression), sut.GetActualExpression());
            Assert.AreEqual(nameof(expectedExpression), sut.GetExpectedExpression());
        }

        [Test]
        public void AssertionNestsAnotherAssertionInAction_TakesExpressionFromOuterAssertion()
        {
            object? actualExpression = null;

            Assert.Throws<EasyAssertionException>(() => actualExpression.TestActionAssert());

            Assert.AreEqual(nameof(actualExpression), sut.GetActualExpression());
        }

        [Test]
        public void AssertionNestsAnotherAssertionInFunc_TakesExpressionFromOuterAssertion()
        {
            object? actualExpression = null;

            Assert.Throws<EasyAssertionException>(() => actualExpression.TestFuncAssert());

            Assert.AreEqual(nameof(actualExpression), sut.GetActualExpression());
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
            var actualExpression = "foo";
            var expectedExpression = "bar";
            for (var i = 0; i < 2; i++)
                Assert.Throws<EasyAssertionException>(() => actualExpression.ShouldBe(expectedExpression));

            Assert.AreEqual(nameof(actualExpression), sut.GetActualExpression());
            Assert.AreEqual(nameof(expectedExpression), sut.GetExpectedExpression());
        }

        [Test]
        public void ExpectedExpression_IsStringLiteral()
        {
            Assert.Throws<EasyAssertionException>(() => "foo".ShouldBe("bar"));

            Assert.AreEqual("\"bar\"", sut.GetExpectedExpression());
        }

        [Test]
        public void ExpectedExpression_IsNumberLiteral()
        {
            Assert.Throws<EasyAssertionException>(() => 1f.ShouldBe(2f, 0));

            Assert.AreEqual("2f", sut.GetExpectedExpression());
        }

        [Test]
        public void ExpectedExpression_ContainsActualExpression()
        {
            var actualExpression = new TestClass(1);
            Func<TestClass, int> valueOf = x => x.Value;

            Assert.Throws<EasyAssertionException>(() => actualExpression.Assert(a => a.Value.ShouldNotBe(valueOf(a))));

            Assert.AreEqual($"{nameof(valueOf)}({nameof(actualExpression)})", sut.GetExpectedExpression());
        }

        [Test]
        public void ExpectedExpression_PassedThroughIndexedUserAssertion()
        {
            var expectedExpression = new TestClass(1);
            var expectedIndex = 3;

            Assert.Throws<EasyAssertionException>(() => new TestClass(2).TestIndexedUserAssertion(expectedExpression, expectedIndex, (a, e) => a.ShouldBe(e)));

            Assert.AreEqual($"{nameof(expectedExpression)}[{expectedIndex}]", sut.GetExpectedExpression());
        }

        class TestClass
        {
            public readonly int Value;

            public TestClass(int value)
            {
                Value = value;
            }

            public override bool Equals(object obj)
            {
                return obj is TestClass other
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
        public static Actual<object> TestActionAssert(this object? actual)
        {
            return actual.RegisterAssertion(c => { actual.ShouldNotBeNull(); });
        }

        public static Actual<object> TestFuncAssert(this object? actual)
        {
            return actual.RegisterAssertion(c => actual.ShouldNotBeNull());
        }

        public static Actual<object> TestIndexedUserAssertion(this object actual, object expected, int index, Action<object, object> userAssertion)
        {
            return actual.RegisterNotNullAssertion(c => c.Call(() => userAssertion(actual, expected), $"[{index}]", $"[{index}]"));
        }
    }
}