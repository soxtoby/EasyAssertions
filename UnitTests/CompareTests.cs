using System.Collections.Generic;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class CompareTests
    {
        [Test]
        public void ObjectsAreEqual_EqualValues_AreEqual()
        {
            Assert.IsTrue(Compare.ObjectsAreEqual(1, 1));
        }

        [Test]
        public void ObjectsAreEqual_DifferentValues_AreNotEqual()
        {
            Assert.IsFalse(Compare.ObjectsAreEqual(1, 2));
        }

        [Test]
        public void CollectionsMatch_MatchingIEnumerables_AreEqual()
        {
            Assert.IsTrue(Compare.CollectionsMatch(new[] { 1, 2, 3 }, new[] { 1, 2, 3 }));
        }

        [Test]
        public void CollectionsMatch_NonMatchingIEnumerables_AreNotEqual()
        {
            Assert.IsFalse(Compare.CollectionsMatch(new[] { 1, 2, 3 }, new[] { 1, 3, 2 }));
        }

        [Test]
        public void CollectionsMatch_ActualIEnumerableLongerThanExpectedIEnumerable_AreNotEqual()
        {
            Assert.IsFalse(Compare.CollectionsMatch(new[] { 1, 2, 3 }, new[] { 1, 2 }));
        }

        [Test]
        public void CollectionsMatch_ActualIEnumerableShorterThanExpectedIEnumerable_AreNotEqual()
        {
            Assert.IsFalse(Compare.CollectionsMatch(new[] { 1, 2 }, new[] { 1, 2, 3 }));
        }

        [Test]
        public void CollectionsMatch_MatchingNestedEnumerables_AreEqual()
        {
            var actual = new List<List<int>>
                {
                    new List<int> { 1, 2, 3 },
                    new List<int> { 4, 5, 6 }
                };
            var expected = new[]
                {
                    new[] { 1, 2, 3 },
                    new[] { 4, 5, 6 }
                };
            Assert.IsTrue(Compare.CollectionsMatch(actual, expected));
        }
    }
}