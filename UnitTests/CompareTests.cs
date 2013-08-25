using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

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
        public void ContainsAny_EmptySequence_DoesNotContainItems()
        {
            Assert.IsFalse(Compare.ContainsAny(Enumerable.Empty<int>(), new[] { 1 }));
        }

        [Test]
        public void ContainsAny_NothingToLookFor_DoesNotContainItems()
        {
            Assert.IsTrue(Compare.ContainsAny(new[] { 1 }, Enumerable.Empty<int>()));
        }

        [Test]
        public void ContainsAny_ContainsItem()
        {
            Assert.IsTrue(Compare.ContainsAny(new[] { 1, 2 }, new[] { 3, 2 }));
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

        [Test]
        public void TreesMatch_SingleMatchingLevel_ReturnsTrue()
        {
            Assert.IsTrue(Compare.TreesMatch(
                new[] { 1, 2, 3 },
                new TestNode<int> { 1, 2, 3 },
                n => Enumerable.Empty<int>(),
                Compare.ObjectsAreEqual));
        }

        [Test]
        public void TreesMatch_SingleNonMatchingLevel_ReturnsFalse()
        {
            Assert.IsFalse(Compare.TreesMatch(
                new[] { 1, 2, 3 },
                new TestNode<int> { 1, 3, 2 },
                n => Enumerable.Empty<int>(),
                Compare.ObjectsAreEqual));
        }

        [Test]
        public void TreesMatch_MatchingRootNodesWithNonMatchingChildren_ReturnsFalse()
        {
            Assert.IsFalse(Compare.TreesMatch(
                new[]{
                        new ActualNode(1)
                            {
                                Children =
                                    {
                                        new ActualNode(2),
                                        new ActualNode(3)
                                    }
                            }
                    },
                new TestNode<int>
                    {
                        new TestNode<int>(1)
                            {
                                3,
                                2
                            }
                    },
                n => n.Children,
                (a, e) => a.Value == e));
        }

        [Test]
        public void TreesMatch_MatchingRootNodesWithDifferentLengthChildren_ReturnsFalse()
        {
            Assert.IsFalse(Compare.TreesMatch(
                new[]
                    {
                        new ActualNode(1),
                        new ActualNode(2)
                            {
                                Children =
                                    {
                                        new ActualNode(3)
                                    }
                            }
                    },
                new TestNode<int>
                    {
                        new TestNode<int>(1),
                        new TestNode<int>(2)
                            {
                                3,
                                4
                            }
                    },
                n => n.Children,
                (a, e) => a.Value == e));
        }

        [Test]
        public void TreesMatch_MatchingRootNodesWithMatchingChildren_ReturnsTrue()
        {
            Assert.IsTrue(Compare.TreesMatch(
                new[]
                    {
                        new ActualNode(1)
                            {
                                Children =
                                    {
                                        new ActualNode(11),
                                        new ActualNode(12)
                                    }
                            },
                        new ActualNode(2)
                            {
                                Children =
                                    {
                                        new ActualNode(21),
                                        new ActualNode(22)
                                            {
                                                Children =
                                                    {
                                                        new ActualNode(221)
                                                    }
                                            }
                                    }
                            }
                    },
                new TestNode<int>
                    {
                        new TestNode<int>(1)
                            {
                                11,
                                12
                            },
                        new TestNode<int>(2)
                            {
                                21,
                                new TestNode<int>(22)
                                    {
                                        221
                                    }
                            }
                    },
                n => n.Children,
                (a, e) => a.Value == e));
        }

        private class ActualNode
        {
            public readonly int Value;
            public readonly List<ActualNode> Children = new List<ActualNode>();

            public ActualNode(int value)
            {
                Value = value;
            }
        }
    }
}