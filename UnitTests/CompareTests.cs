using System.Collections.Generic;
using System.Linq;
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
        public void FloatsAreWithinDelta_WithinDelta()
        {
            Assert.IsTrue(Compare.AreWithinTolerance(1f, 2f, 1f));
        }

        [Test]
        public void FloatsAreWithinDelta_OutsideDelta()
        {
            Assert.IsFalse(Compare.AreWithinTolerance(1f, 2f, 0.9f));
        }

        [Test]
        public void DoublesAreWithinDelta_WithinDelta()
        {
            Assert.IsTrue(Compare.AreWithinTolerance(1d, 2d, 1d));
        }

        [Test]
        public void DoublesAreWithinDelta_OutsideDelta()
        {
            Assert.IsFalse(Compare.AreWithinTolerance(1d, 2d, 0.9d));
        }

        [Test]
        public void CollectionsMatch_MatchingIEnumerables_AreEqual()
        {
            Assert.IsTrue(Compare.CollectionsMatch(new[] { 1, 2, 3 }, new[] { 1, 2, 3 }, Compare.ObjectsMatch));
        }

        [Test]
        public void CollectionsMatch_NonMatchingIEnumerables_AreNotEqual()
        {
            Assert.IsFalse(Compare.CollectionsMatch(new[] { 1, 2, 3 }, new[] { 1, 3, 2 }, Compare.ObjectsMatch));
        }

        [Test]
        public void CollectionsMatch_ActualIEnumerableLongerThanExpectedIEnumerable_AreNotEqual()
        {
            Assert.IsFalse(Compare.CollectionsMatch(new[] { 1, 2, 3 }, new[] { 1, 2 }, Compare.ObjectsMatch));
        }

        [Test]
        public void CollectionsMatch_ActualIEnumerableShorterThanExpectedIEnumerable_AreNotEqual()
        {
            Assert.IsFalse(Compare.CollectionsMatch(new[] { 1, 2 }, new[] { 1, 2, 3 }, Compare.ObjectsMatch));
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
            Assert.IsTrue(Compare.CollectionsMatch(actual, expected, Compare.ObjectsMatch));
        }

        [Test]
        public void IsEmpty_Empty()
        {
            Assert.IsTrue(Compare.IsEmpty(Enumerable.Empty<int>()));
        }

        [Test]
        public void IsEmpty_NotEmpty()
        {
            Assert.IsFalse(Compare.IsEmpty(new[] { 1 }));
        }
    }
}