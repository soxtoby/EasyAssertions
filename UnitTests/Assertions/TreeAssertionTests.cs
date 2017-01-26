using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class TreeAssertionTests : AssertionTests
    {
        [Test]
        public void ShouldMatchTree_TreeMatches_ReturnsActual()
        {
            object actualItem = new object();
            IEnumerable<object> actual = new[] { actualItem };

            Actual<IEnumerable<object>> result = actual.ShouldMatch(new[] { actualItem.Node() }, NoChildren);

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldMatchTree_TreesAreDifferent_FailsWithTreesDoNotMatchMessage()
        {
            int[] actual = { 1 };
            TestNode<int>[] expected = { 2 };
            Error.TreesDoNotMatch(expected, actual, NoChildren, StandardTests.Instance.ObjectsAreEqual, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() => actual.ShouldMatch(expected, NoChildren, "foo"));
        }

        [Test]
        public void ShouldMatchTree_ActualIsNull_FailsWithTypedNotEqualMessage()
        {
            IEnumerable<int> actual = null;
            Error.NotEqual(typeof(IEnumerable<int>), null, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() => actual.ShouldMatch(1.Node(), NoChildren, "foo"));
        }

        [Test]
        public void ShouldMatchTree_ExpectedIsNull_ThrowsArgumentNullException()
        {
            AssertArgumentNullException("expectedRootNodes", () => new int[0].ShouldMatch((IEnumerable<TestNode<int>>)null, NoChildren));
        }

        [Test]
        public void ShouldMatchTree_GetChildrenIsNull_ThrowsArgumentNullException()
        {
            AssertArgumentNullException("getChildren", () => new int[0].ShouldMatch(1.Node(), (Func<int, IEnumerable<int>>)null));
        }

        [Test]
        public void ShouldMatchTree_CorrectlyRegistersAssertion()
        {
            int[] actual = { 1 };
            TestNode<int>[] expected = { 2 };
            Error.TreesDoNotMatch(Arg.Any<IEnumerable<TestNode<int>>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<Func<int, IEnumerable<int>>>(), Arg.Any<Func<object, object, bool>>()).Returns(ExpectedException);

            AssertThrowsExpectedError(() => actual.ShouldMatch(expected, NoChildren));

            Assert.AreEqual(nameof(actual), TestExpression.GetActual());
            Assert.AreEqual(nameof(expected), TestExpression.GetExpected());
        }

        [Test]
        public void ShouldMatchTree_CustomEquality_TreeMatches_ReturnsActual()
        {
            object actualItem = new object();
            IEnumerable<object> actual = new[] { actualItem };

            Actual<IEnumerable<object>> result = actual.ShouldMatch(new[] { new object().Node() }, NoChildren, (a, e) => a == actualItem);

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldMatchTree_CustomEquality_TreesAreDifferent_FailsWithTreesDoNotMatchMessage()
        {
            int[] actual = { 1 };
            TestNode<int>[] expected = { 1 };
            Func<int, int, bool> equality = Substitute.For<Func<int, int, bool>>();
            Func<object, object, bool> formatterEquality = null;
            Error.TreesDoNotMatch(expected, actual, NoChildren, Arg.Do<Func<object, object, bool>>(func => formatterEquality = func), "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() => actual.ShouldMatch(expected, NoChildren, equality, "foo"));

            formatterEquality(2, 3);
            equality.Received()(2, 3);
        }

        [Test]
        public void ShouldMatchTree_CustomEquality_ActualIsNull_FailsWithTypedNotEqualMessage()
        {
            IEnumerable<int> actual = null;
            Error.NotEqual(typeof(IEnumerable<int>), null, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() => actual.ShouldMatch(1.Node(), NoChildren, (a, e) => false, "foo"));
        }

        [Test]
        public void ShouldMatchTree_CustomEquality_ExpectedIsNull_ThrowsArgumentNullException()
        {
            AssertArgumentNullException("expectedRootNodes", () => new int[0].ShouldMatch((IEnumerable<TestNode<int>>)null, NoChildren, (a, e) => false));
        }

        [Test]
        public void ShouldMatchTree_CustomEquality_GetChildrenIsNull_ThrowsArgumentNullException()
        {
            AssertArgumentNullException("getChildren", () => new int[0].ShouldMatch(1.Node(), null, (a, e) => false));
        }

        [Test]
        public void ShouldMatchTree_CustomEquality_PredicateIsNull_ThrowsArgumentNullException()
        {
            AssertArgumentNullException("predicate", () => new int[0].ShouldMatch(1.Node(), NoChildren, (Func<int, int, bool>)null));
        }

        [Test]
        public void ShouldMatchTree_CustomEquality_CorrectlyRegistersAssertion()
        {
            int[] actual = { 1 };
            TestNode<int>[] expected = { 1 };
            Func<int, int, bool> equality = Substitute.For<Func<int, int, bool>>();
            Error.TreesDoNotMatch(Arg.Any<IEnumerable<TestNode<int>>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<Func<int, IEnumerable<int>>>(), Arg.Any<Func<object, object, bool>>()).Returns(ExpectedException);

            AssertThrowsExpectedError(() => actual.ShouldMatch(expected, NoChildren, equality));

            Assert.AreEqual(nameof(actual), TestExpression.GetActual());
            Assert.AreEqual(nameof(expected), TestExpression.GetExpected());
        }

        private static IEnumerable<T> NoChildren<T>(T node)
        {
            return Enumerable.Empty<T>();
        }
    }
}