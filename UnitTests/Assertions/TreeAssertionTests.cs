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
            MockFormatter.TreesDoNotMatch(expected, actual, NoChildren, Compare.ObjectsAreEqual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, NoChildren, "foo"));

            Assert.AreEqual("bar", result.Message);
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
            MockFormatter.TreesDoNotMatch(expected, actual, NoChildren, Arg.Do<Func<object, object, bool>>(func => formatterEquality = func), "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, NoChildren, equality, "foo"));

            Assert.AreEqual("bar", result.Message);
            formatterEquality(2, 3);
            equality.Received()(2, 3);
        }

        private static IEnumerable<T> NoChildren<T>(T node)
        {
            return Enumerable.Empty<T>();
        }
    }
}