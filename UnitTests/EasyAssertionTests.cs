using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NSubstitute;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class EasyAssertionTests
    {
        private IFailureMessageFormatter mockFormatter;

        [SetUp]
        public void SetUp()
        {
            mockFormatter = Substitute.For<IFailureMessageFormatter>();
            FailureMessageFormatter.Override(mockFormatter);
        }

        [TearDown]
        public void TearDown()
        {
            FailureMessageFormatter.Default();
        }

        [Test]
        public void ShouldBe_SameValueReturnsActualValue()
        {
            Equatable actual = new Equatable(1);
            Equatable expected = new Equatable(1);
            Actual<Equatable> result = actual.ShouldBe(expected);

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldBe_DifferentObjects_FailsWithObjectsNotEqualMessage()
        {
            object obj1 = new object();
            object obj2 = new object();
            mockFormatter.NotEqual(obj2, obj1, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => obj1.ShouldBe(obj2, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldBe_DifferentStrings_FailsWithStringsNotEqualMessage()
        {
            mockFormatter.NotEqual("foo", "bar", "baz").Returns("qux");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => "bar".ShouldBe("foo", "baz"));

            Assert.AreEqual("qux", result.Message);
        }

        [Test]
        public void ShouldBe_FloatsWithinDelta_ReturnsActualValue()
        {
            const float actual = 1f;
            Actual<float> result = actual.ShouldBe(1f, 1f);

            Assert.AreEqual(actual, result.And);
        }

        [Test]
        public void ShouldBe_FloatsOutsideDelta_FailsWithObjectsNotEqualMessage()
        {
            const float expected = 10f;
            const float actual = 1f;
            mockFormatter.NotEqual(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBe(expected, 0, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldBe_DoublesWithinDelta_ReturnsActualValue()
        {
            const double actual = 1d;

            Actual<double> result = actual.ShouldBe(1d, 0);

            Assert.AreEqual(actual, result.And);
        }

        [Test]
        public void ShouldBe_DoublesOutsideDelta_FailsWithObjectsNotEqualMessage()
        {
            const double expected = 10d;
            const double actual = 1d;
            mockFormatter.NotEqual(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBe(expected, 1d, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldNotBe_DifferentValue_ReturnsActualValue()
        {
            Equatable actual = new Equatable(1);

            Actual<Equatable> result = actual.ShouldNotBe(new Equatable(2));

            Assert.AreSame(actual, result.Value);
        }

        [Test]
        public void ShouldNotBe_EqualValue_FailsWithObjectsEqualMessage()
        {
            Equatable actual = new Equatable(1);
            Equatable notExpected = new Equatable(1);
            mockFormatter.AreEqual(notExpected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotBe(notExpected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldNotBe_FloatsOutsideDelta_ReturnsActualValue()
        {
            const float actual = 1f;
            Actual<float> result = actual.ShouldNotBe(2f, 0);

            Assert.AreEqual(actual, result.And);
        }

        [Test]
        public void ShouldNotBe_FloatsWithinDelta_FailsWithObjectsEqualMessage()
        {
            const float actual = 1f;
            const float notExpected = 2f;
            mockFormatter.AreEqual(notExpected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotBe(notExpected, 1f, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldNotBe_DoublesOutsideDelta_ReturnsActualValue()
        {
            const double actual = 1d;

            Actual<double> result = actual.ShouldNotBe(2d, 0);

            Assert.AreEqual(actual, result.And);
        }

        [Test]
        public void ShouldNotBe_DoublesWithinDelta_FailsWithObjectsEqualMessage()
        {
            const double actual = 1d;
            const double notExpected = 2d;
            mockFormatter.AreEqual(notExpected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotBe(notExpected, 1d, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldBeNull_IsNull_Passes()
        {
            ((object)null).ShouldBeNull();
        }

        [Test]
        public void ShouldBeNull_NotNull_FailsWithNotEqualToNullMessage()
        {
            object actual = new object();
            mockFormatter.NotEqual(null, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeNull("foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldNotBeNull_NotNull_ReturnsActualValue()
        {
            object actual = new object();

            Actual<object> result = actual.ShouldNotBeNull();

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldNotBeNull_IsNull_FailsWithIsNullMessage()
        {
            mockFormatter.IsNull("foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => ((object)null).ShouldNotBeNull("foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldBeThis_SameObject_ReturnsActualValue()
        {
            object obj = new object();
            Actual<object> result = obj.ShouldBeThis(obj);

            Assert.AreSame(obj, result.And);
        }

        [Test]
        public void ShouldBeThis_DifferentObject_FailsWithObjectsNotSameMessage()
        {
            Equatable actual = new Equatable(1);
            Equatable expected = new Equatable(1);
            mockFormatter.NotSame(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeThis(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldNotBeThis_DifferentObject_ReturnsActualValue()
        {
            object actual = new object();

            Actual<object> result = actual.ShouldNotBeThis(new object());

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldNotBeThis_SameObject_FailsWithObjectsAreSameMessage()
        {
            object actual = new object();
            mockFormatter.AreSame(actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotBeThis(actual, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldBeEmpty_IsEmpty_ReturnsActualValue()
        {
            IEnumerable<object> actual = Enumerable.Empty<object>();

            var result = actual.ShouldBeEmpty();

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldBeEmpty_NonEmpty_FailsWithEnumerableNotEmptyMessage()
        {
            int[] actual = new[] { 1 };
            mockFormatter.NotEmpty(actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeEmpty("foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldNotBeEmpty_NotEmpty_ReturnsActualValue()
        {
            int[] actual = new[] { 1 };

            Actual<int[]> result = actual.ShouldNotBeEmpty();

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldNotBeEmpty_IsEmpty_FailsWithEnumerableEmptyMessage()
        {
            IEnumerable<object> actual = Enumerable.Empty<object>();
            mockFormatter.IsEmpty("foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotBeEmpty("foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldMatch_MatchingEnumerable_ReturnsActualValue()
        {
            int[] actual = new[] { 1, 2 };
            int[] expected = new[] { 1, 2 };
            Actual<IEnumerable<int>> result = actual.ShouldMatch(expected);

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldMatch_NonMatchingEnumerables_FailsWithEnumerablesDoNotMatchMessage()
        {
            int[] expected = new[] { 3, 4 };
            int[] actual = new[] { 1, 2 };
            mockFormatter.DoNotMatch(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldMatchCustomEquality_MatchingEnumerable_ReturnsActualValue()
        {
            IEnumerable<int> actual = new[] { 1, 2 };
            Actual<IEnumerable<int>> result = actual.ShouldMatch(new[] { 1, 2 }, (a, e) => a == e);

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldMatchCustomEquality_NonMatchingEnumerables_FailsWithEnumerablesDoNotMatch()
        {
            int[] actual = new[] { 1, 2, 4 };
            int[] expected = new[] { 2, 4, 6 };
            mockFormatter.DoNotMatch(expected, actual, "foo").Returns("bar");
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, (a, e) => a == e / 2, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldMatch_FloatsWithinDelta_ReturnsActualValue()
        {
            float[] actual = new[] { 10f, 20f };

            Actual<IEnumerable<float>> result = actual.ShouldMatch(new[] { 11f, 21f }, 1f);

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldMatch_FloatsOutsideDelta_FailsWithEnumerablesDoNotMatchMessage()
        {
            float[] actual = new[] { 10f, 20f };
            float[] expected = new[] { 11f, 21f };
            mockFormatter.DoNotMatch(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, 0.9f, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldMatch_DoublesWithinDelta_ReturnsActualValue()
        {
            double[] actual = new[] { 10d, 20d };

            Actual<IEnumerable<double>> result = actual.ShouldMatch(new[] { 11d, 21d }, 1d);

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldMatch_DoublesOutsideDelta_FailsWithEnumerablesDoNotMatchMessage()
        {
            double[] actual = new[] { 10d, 20d };
            double[] expected = new[] { 11d, 21d };
            mockFormatter.DoNotMatch(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, 0.9d, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldContain_CollectionContainsExpected_ReturnActualValue()
        {
            int[] actual = new[] { 1, 2 };

            Actual<IEnumerable<int>> result = actual.ShouldContain(2);

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldContain_CollectionDoesNotContainExpected_FailsWithEnumerableDoesNotContainMessage()
        {
            int[] actual = new[] { 1, 2 };
            int expected = 3;
            mockFormatter.DoesNotContain(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldContain(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldContainItems_CollectionContainsAllItems_ReturnsActualValue()
        {
            int[] actual = new[] { 1, 2, 3 };

            Actual<IEnumerable<int>> result = actual.ShouldContainItems(new[] { 2, 3, 1 });

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldContainItems_CollectionDoesNotContainAllItems_FailsWithEnumerableDoesNotContainItemsMessage()
        {
            int[] actual = new[] { 1, 2, 3 };
            int[] expected = new[] { 1, 4 };
            mockFormatter.DoesNotContainItems(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldContainItems(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldOnlyOnlyContain_CollectionsHaveSameItems_ReturnsActualValue()
        {
            int[] actual = new[] { 1, 2 };

            Actual<IEnumerable<int>> result = actual.ShouldOnlyContain(new[] { 2, 1 });

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldOnlyContain_MissingItem_FailsWithCollectionDoesNotOnlyContainMessage()
        {
            int[] actual = new[] { 1, 2 };
            int[] expected = new[] { 1, 3 };
            mockFormatter.DoesNotOnlyContain(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldOnlyContain(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldOnlyContain_ExtraItem_FailsWithCollectionDoesNotOnlyContainMessage()
        {
            int[] actual = new[] { 1, 2 };
            int[] expected = new[] { 1 };
            mockFormatter.DoesNotOnlyContain(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldOnlyContain(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldBeThese_SameItems_ReturnsActualValue()
        {
            object a = new object();
            object b = new object();
            object[] actual = new[] { a, b };

            Actual<IEnumerable<object>> result = actual.ShouldBeThese(new[] { a, b });

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldBeThese_DifferentLength_FailsWithEnumerableLenthMismatchMessage()
        {
            object[] actual = new object[] { };
            object[] expected = new[] { new object() };
            mockFormatter.LengthMismatch(1, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeThese(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldBeThese_DifferentItems_FailsWithEnumerablesNotSameMessage()
        {
            object a = new object();
            object[] actual = new[] { a, new object() };
            object[] expected = new[] { a, new object() };
            mockFormatter.ItemsNotSame(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeThese(expected, "foo"));

            Assert.AreSame("bar", result.Message);
        }

        [Test]
        public void ItemsSatisfy_ItemsSatisfyAssertions_ReturnsActualValue()
        {
            int[] actual = new[] { 1 };

            Actual<IEnumerable<int>> result = actual.ItemsSatisfy(i => { });

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ItemsSatisfy_CallsAssertionsWithMatchedItem()
        {
            bool gotFirstItem = false;
            bool gotSecondItem = false;

            new[] { 1, 2 }.ItemsSatisfy(
                i => gotFirstItem = i == 1,
                i => gotSecondItem = i == 2);

            Assert.IsTrue(gotFirstItem, "first item");
            Assert.IsTrue(gotSecondItem, "second item");
        }

        [Test]
        public void ItemsSatisfy_WrongNumberOfItems_FailsWithEnumerableLengthMismatchMessage()
        {
            int[] actual = new[] { 1 };
            mockFormatter.LengthMismatch(2, actual).Returns("foo");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ItemsSatisfy(i => { }, i => { }));

            Assert.AreEqual("foo", result.Message);
        }

        [Test]
        public void ItemsSatisfy_ItemDoesNotSatisyItsAssertion_FailsWithThrownException()
        {
            int[] actual = new[] { 1, 2 };
            Exception failure = new Exception("foo");

            Exception result = Assert.Throws<Exception>(() => actual.ItemsSatisfy(
                i => { },
                i => { throw failure; }));

            Assert.AreSame(failure, result);
        }

        [Test]
        public void AllItemsSatisfy_AllItemsSatisfyAssertion_ReturnsActualValue()
        {
            int[] actual = new[] { 1 };

            Actual<IEnumerable<int>> result = actual.AllItemsSatisfy(i => { });

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void AllItemsSatify_ItemsDoNotSatisfyAssertion_FailsWithThrownException()
        {
            int[] actual = new[] { 1, 2 };
            Exception failure = new Exception("foo");

            Exception result = Assert.Throws<Exception>(() => actual.AllItemsSatisfy(i =>
                {
                    if (i > 1) throw failure;
                }));

            Assert.AreSame(failure, result);
        }

        [Test]
        public void ShouldBeA_SubType_ReturnsTypedActual()
        {
            object actual = new SubEquatable(1);
            Actual<Equatable> result = actual.ShouldBeA<Equatable>();

            Assert.AreSame(actual, result.And);
            Assert.AreEqual(1, result.And.Value);
        }

        [Test]
        public void ShouldBeA_SuperType_FailsWithTypesNotEqualMessage()
        {
            object actual = new Equatable(1);
            mockFormatter.NotEqual(typeof(SubEquatable), typeof(Equatable), "foo").Returns("bar");
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeA<SubEquatable>("foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldContain_StringContainsSubstring_ReturnsActualValue()
        {
            Actual<string> result = "foo".ShouldContain("oo");
            Assert.AreEqual("foo", result.And);
        }

        [Test]
        public void ShouldContain_StringDoesNotContainSubstring_FailsWithStringDoesNotContainMessage()
        {
            mockFormatter.DoesNotContain("bar", "foo", "baz").Returns("qux");
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => "foo".ShouldContain("bar", "baz"));

            Assert.AreEqual("qux", result.Message);
        }

        [Test]
        public void ShouldStartWith_StringStartsWithExpected_ReturnsActualValue()
        {
            string actual = "foobar";

            Actual<string> result = actual.ShouldStartWith("foo");

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldStartWith_StringDoesNotStartWithExpected_FailsWithStringDoesNotStartWithMessage()
        {
            string actual = "foobar";
            string expectedStart = "bar";
            mockFormatter.DoesNotStartWith(expectedStart, actual, "foo").Returns("baz");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldStartWith(expectedStart, "foo"));

            Assert.AreEqual("baz", result.Message);
        }

        [Test]
        public void ShouldEndWith_StringEndsWithExpected_ReturnsActualValue()
        {
            string actual = "foobar";

            Actual<string> result = actual.ShouldEndWith("bar");

            Assert.AreSame(actual, result.Value);
        }

        [Test]
        public void ShouldEndWith_StringDoesNotEndWithExpected_FailsWithStringDoesNotEndWithMessage()
        {
            string actual = "foobar";
            string expectedEnd = "foo";
            mockFormatter.DoesNotEndWith(expectedEnd, actual, "bar").Returns("baz");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldEndWith(expectedEnd, "bar"));

            Assert.AreEqual("baz", result.Message);
        }

        [Test]
        public void ShouldThrow_Throws_ReturnsException()
        {
            Exception expectedException = new Exception();
            ExceptionThrower thrower = new ExceptionThrower(expectedException);

            Actual<Exception> result = Should.Throw<Exception>(() => thrower.Throw());

            result.And.ShouldBeThis(expectedException);
        }

        [Test]
        public void ShouldThrow_DoesNotThrow_FailsWithNoExceptionMessage()
        {
            Expression<Action> noThrow = () => "".Trim();
            mockFormatter.NoException(typeof(Exception), noThrow, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                Should.Throw<Exception>(noThrow, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldThrow_ThrowsWrongType_FailsWithWrongExceptionMessage()
        {
            ExceptionThrower thrower = new ExceptionThrower(new Exception());
            Expression<Action> throwsException = () => thrower.Throw();
            mockFormatter.WrongException(typeof(InvalidOperationException), typeof(Exception), throwsException, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                Should.Throw<InvalidOperationException>(throwsException, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldThrow_ThrowsWrongType_FailsWithActualExceptionAsInnerException()
        {
            Exception expectedException = new Exception();
            ExceptionThrower thrower = new ExceptionThrower(expectedException);

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                Should.Throw<InvalidOperationException>(() => thrower.Throw()));

            Assert.AreSame(expectedException, result.InnerException);
        }

        [Test]
        public void Assert_Passes_ReturnsInnerActual()
        {
            Actual<int> result = 1.Assert(i => new Actual<int>(2));

            Assert.AreEqual(2, result.And);
        }

        private class ExceptionThrower
        {
            private readonly Exception exception;

            public ExceptionThrower(Exception exception)
            {
                this.exception = exception;
            }

            public void Throw()
            {
                throw exception;
            }
        }

        private class Equatable
        {
            public readonly int Value;

            public Equatable(int value)
            {
                this.Value = value;
            }

            public override bool Equals(object obj)
            {
                Equatable otherEquatable = obj as Equatable;
                return otherEquatable != null
                    && otherEquatable.Value == Value;
            }
        }

        private class SubEquatable : Equatable
        {
            public SubEquatable(int value) : base(value) { }
        }
    }
}
