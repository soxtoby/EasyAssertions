using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class CollectionAssertionTests : AssertionTests
    {
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
            MockFormatter.NotEmpty(actual, "foo").Returns("bar");

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
            MockFormatter.IsEmpty("foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotBeEmpty("foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldMatch_MatchingEnumerable_ReturnsActualValue()
        {
            int[] actual = new[] { 1, 2 };
            int[] expected = new[] { 1, 2 };
            Actual<IEnumerable<int>> result = actual.ShouldMatch(expected.AsEnumerable());

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldMatch_NonMatchingEnumerables_FailsWithEnumerablesDoNotMatchMessage()
        {
            int[] expected = new[] { 3, 4 };
            int[] actual = new[] { 1, 2 };
            MockFormatter.DoNotMatch(expected, actual, Compare.ObjectsMatch, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldMatchParams_MatchingParams_ReturnsActualValue()
        {
            int[] actual = new[] { 1, 2, 3 };

            Actual<IEnumerable<int>> result = actual.ShouldMatch(1, 2, 3);

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldMatchParams_EnumerableDoesNotMatchParams_FailsWIthEnumerablesDoNotMatchMessage()
        {
            int[] actual = new[] { 1, 2 };
            MockFormatter.DoNotMatch(ArgMatches(new[] { 3, 4 }), actual, Compare.ObjectsMatch).Returns("foo");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(3, 4));

            Assert.AreEqual("foo", result.Message);
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
            Func<int, int, bool> predicate = (a, e) => a == e / 2;
            MockFormatter.DoNotMatch(expected, actual, predicate, "foo").Returns("bar");
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, predicate, "foo"));

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
            Func<float, float, bool> predicate = null;
            MockFormatter.DoNotMatch(expected, actual, Arg.Do<Func<float, float, bool>>(p => predicate = p), "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, 0.9f, "foo"));

            Assert.AreEqual("bar", result.Message);
            Assert.IsTrue(predicate(1f, 1.9f));
            Assert.IsFalse(predicate(1f, 2f));
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
            Func<double, double, bool> predicate = null;
            MockFormatter.DoNotMatch(expected, actual, Arg.Do<Func<double, double, bool>>(p => predicate = p), "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, 0.9d, "foo"));

            Assert.AreEqual("bar", result.Message);
            Assert.IsTrue(predicate(1d, 1.9d));
            Assert.IsFalse(predicate(1d, 2d));
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
            const int expected = 3;
            MockFormatter.DoesNotContain(expected, actual, "foo").Returns("bar");

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
            MockFormatter.DoesNotContainItems(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldContainItems(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldNotContainItems_CollectionDoesNotContainAnyOfTheItems_ReturnsActualValue()
        {
            int[] actual = new[] { 1, 2 };

            Actual<IEnumerable<int>> result = actual.ShouldNotContainItems(new[] { 3, 4 });

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldNotContainItems_CollectionContainsOneOfTheItems_FailsWithSequenceContainsItemMessage()
        {
            int[] actual = new[] { 1, 2 };
            int[] expectedToNotContain = new[] { 3, 2 };
            MockFormatter.Contains(expectedToNotContain, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotContainItems(expectedToNotContain, "foo"));

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
            MockFormatter.DoesNotOnlyContain(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldOnlyContain(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldOnlyContain_ExtraItem_FailsWithCollectionDoesNotOnlyContainMessage()
        {
            int[] actual = new[] { 1, 2 };
            int[] expected = new[] { 1 };
            MockFormatter.DoesNotOnlyContain(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldOnlyContain(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldMatchReferences_SameItems_ReturnsActualValue()
        {
            object a = new object();
            object b = new object();
            object[] actual = new[] { a, b };

            Actual<IEnumerable<object>> result = actual.ShouldMatchReferences(new[] { a, b });

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldMatchReferences_DifferentLength_FailsWithEnumerableLenthMismatchMessage()
        {
            object[] actual = new object[] { };
            object[] expected = new[] { new object() };
            MockFormatter.LengthMismatch(1, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatchReferences(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldMatchReferences_DifferentItems_FailsWithEnumerablesNotSameMessage()
        {
            object a = new object();
            object[] actual = new[] { a, new object() };
            object[] expected = new[] { a, new object() };
            MockFormatter.ItemsNotSame(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatchReferences(expected, "foo"));

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
            MockFormatter.LengthMismatch(2, actual).Returns("foo");

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

        private static IEnumerable<TItem> ArgMatches<TItem>(IEnumerable<TItem> expected)
        {
            return Arg.Is<IEnumerable<TItem>>(arg => Compare.CollectionsMatch(arg, expected, Compare.ObjectsAreEqual));
        }
    }
}