using NSubstitute;
using NUnit.Framework;
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

        private IEnumerable<T> NoChildren<T>(T node)
        {
            return Enumerable.Empty<T>();
        }
    }
}