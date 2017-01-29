using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class StandardTestsTests
    {
        private StandardTests sut;

        [SetUp]
        public void SetUp()
        {
            sut = StandardTests.Instance;
        }

        [Test]
        public void ObjectsAreEqual_EqualValues_AreEqual()
        {
            Assert.IsTrue(sut.ObjectsAreEqual(1, 1));
        }

        [Test]
        public void ObjectsAreEqual_DifferentValues_AreNotEqual()
        {
            Assert.IsFalse(sut.ObjectsAreEqual(1, 2));
        }

        [Test]
        public void FloatsAreWithinDelta_WithinDelta()
        {
            Assert.IsTrue(sut.AreWithinTolerance(1f, 2f, 1d));
        }

        [Test]
        public void FloatsAreWithinDelta_OutsideDelta()
        {
            Assert.IsFalse(sut.AreWithinTolerance(1f, 2f, 0.9d));
        }

        [Test]
        public void DoublesAreWithinDelta_WithinDelta()
        {
            Assert.IsTrue(sut.AreWithinTolerance(1d, 2d, 1d));
        }

        [Test]
        public void DoublesAreWithinDelta_OutsideDelta()
        {
            Assert.IsFalse(sut.AreWithinTolerance(1d, 2d, 0.9d));
        }

        [Test]
        public void CollectionsMatch_MatchingIEnumerables_AreEqual()
        {
            Assert.IsTrue(sut.CollectionsMatch(new[] { 1, 2, 3 }, new[] { 1, 2, 3 }, sut.ObjectsMatch));
        }

        [Test]
        public void CollectionsMatch_NonMatchingIEnumerables_AreNotEqual()
        {
            Assert.IsFalse(sut.CollectionsMatch(new[] { 1, 2, 3 }, new[] { 1, 3, 2 }, sut.ObjectsMatch));
        }

        [Test]
        public void CollectionsMatch_ActualIEnumerableLongerThanExpectedIEnumerable_AreNotEqual()
        {
            Assert.IsFalse(sut.CollectionsMatch(new[] { 1, 2, 3 }, new[] { 1, 2 }, sut.ObjectsMatch));
        }

        [Test]
        public void CollectionsMatch_ActualIEnumerableShorterThanExpectedIEnumerable_AreNotEqual()
        {
            Assert.IsFalse(sut.CollectionsMatch(new[] { 1, 2 }, new[] { 1, 2, 3 }, sut.ObjectsMatch));
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
            Assert.IsTrue(sut.CollectionsMatch(actual, expected, sut.ObjectsMatch));
        }

        [Test]
        public void CollectionsMatch_OnlyEnumeratesOnce()
        {
            TestEnumerable<int> actual = MakeEnumerable(1);
            TestEnumerable<int> expected = MakeEnumerable(1);

            sut.CollectionsMatch(actual, expected, sut.ObjectsMatch);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void CollectionStartsWith_MatchingCollections_StartsWith()
        {
            Assert.IsTrue(sut.CollectionStartsWith(new object[] { 1, 2, 3 }, new object[] { 1, 2, 3 }, Equals));
        }

        [Test]
        public void CollectionStartsWith_Subset_StartsWith()
        {
            Assert.IsTrue(sut.CollectionStartsWith(new object[] { 1, 2, 3 }, new object[] { 1, 2 }, Equals));
        }

        [Test]
        public void CollectionStartsWith_LongerCollection_DoesNotStartWith()
        {
            Assert.IsFalse(sut.CollectionStartsWith(new object[] { 1 }, new object[] { 1, 2 }, Equals));
        }

        [Test]
        public void CollectionStartsWith_DifferentItems_DoesNotStartWith()
        {
            Assert.IsFalse(sut.CollectionStartsWith(new object[] { 1, 2 }, new object[] { 1, 3 }, Equals));
        }

        [Test]
        public void CollectionStartsWith_OnlyEnumeratesOnce()
        {
            TestEnumerable<int> actual = MakeEnumerable(1);
            TestEnumerable<int> expected = MakeEnumerable(1);

            sut.CollectionStartsWith(actual, expected, sut.ObjectsMatch);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void CollectionEndsWith_MatchingCollections_StartsWith()
        {
            Assert.IsTrue(sut.CollectionEndsWith(new object[] { 1, 2, 3 }, new object[] { 1, 2, 3 }, Equals));
        }

        [Test]
        public void CollectionEndsWith_Subset_StartsWith()
        {
            Assert.IsTrue(sut.CollectionEndsWith(new object[] { 1, 2, 3 }, new object[] { 2, 3 }, Equals));
        }

        [Test]
        public void CollectionEndsWith_LongerCollection_DoesNotEndWith()
        {
            Assert.IsFalse(sut.CollectionEndsWith(new object[] { 1 }, new object[] { 1, 2 }, Equals));
        }

        [Test]
        public void CollectionEndsWith_DifferentItems_DoesNotEndWith()
        {
            Assert.IsFalse(sut.CollectionEndsWith(new object[] { 1, 2 }, new object[] { 3, 2 }, Equals));
        }

        [Test]
        public void CollectionEndsWith_OnlyEnumeratesOnce()
        {
            TestEnumerable<int> actual = MakeEnumerable(1);
            TestEnumerable<int> expected = MakeEnumerable(1);

            sut.CollectionEndsWith(actual, expected, sut.ObjectsMatch);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void ContainsAny_EmptySequence_DoesNotContainItems()
        {
            Assert.IsFalse(sut.ContainsAny(Enumerable.Empty<int>(), new[] { 1 }));
        }

        [Test]
        public void ContainsAny_NothingToLookFor_DoesNotContainItems()
        {
            Assert.IsTrue(sut.ContainsAny(new[] { 1 }, Enumerable.Empty<int>()));
        }

        [Test]
        public void ContainsAny_ContainsItem()
        {
            Assert.IsTrue(sut.ContainsAny(new[] { 1, 2 }, new[] { 3, 2 }));
        }

        [Test]
        public void ContainsAny_OnlyEnumeratesOnce()
        {
            TestEnumerable<int> actual = MakeEnumerable(1);
            TestEnumerable<int> expected = MakeEnumerable(1);

            sut.ContainsAny(actual, expected);

            Assert.AreEqual(1, actual.EnumerationCount);
            Assert.AreEqual(1, expected.EnumerationCount);
        }

        [Test]
        public void IsEmpty_Empty()
        {
            Assert.IsTrue(sut.IsEmpty(Enumerable.Empty<int>()));
        }

        [Test]
        public void IsEmpty_NotEmpty()
        {
            Assert.IsFalse(sut.IsEmpty(new[] { 1 }));
        }

        [Test]
        public void TreesMatch_SingleMatchingLevel_ReturnsTrue()
        {
            Assert.IsTrue(sut.TreesMatch(
                new[] { 1, 2, 3 },
                new TestNode<int> { 1, 2, 3 },
                n => Enumerable.Empty<int>(),
                sut.ObjectsAreEqual));
        }

        [Test]
        public void TreesMatch_SingleNonMatchingLevel_ReturnsFalse()
        {
            Assert.IsFalse(sut.TreesMatch(
                new[] { 1, 2, 3 },
                new TestNode<int> { 1, 3, 2 },
                n => Enumerable.Empty<int>(),
                sut.ObjectsAreEqual));
        }

        [Test]
        public void TreesMatch_MatchingRootNodesWithNonMatchingChildren_ReturnsFalse()
        {
            Assert.IsFalse(sut.TreesMatch(
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
            Assert.IsFalse(sut.TreesMatch(
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
            Assert.IsTrue(sut.TreesMatch(
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

        private static TestEnumerable<T> MakeEnumerable<T>(params T[] items) => new TestEnumerable<T>(items);

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