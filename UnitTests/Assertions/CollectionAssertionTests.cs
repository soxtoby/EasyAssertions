using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
            int[] actual = { 1 };
            MockFormatter.NotEmpty(actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeEmpty("foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldBeEmpty_Null_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<string> actual = null;
            AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<string>), null, msg => actual.ShouldBeEmpty(msg));
        }

        [Test]
        public void ShouldNotBeEmpty_NotEmpty_ReturnsActualValue()
        {
            int[] actual = { 1 };

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
        public void ShouldNotBeEmpty_Null_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<string> actual = null;
            AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<string>), null, msg => actual.ShouldNotBeEmpty(msg));
        }

        [Test]
        public void ShouldBeSingular_IsSingular_ReturnsActualValue()
        {
            int[] actual = { 1 };

            Actual<int[]> result = actual.ShouldBeSingular();

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldBeSingular_IsNotSingular_FailsWithLengthMismatchMessage()
        {
            int[] actual = { };
            MockFormatter.LengthMismatch(1, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeSingular("foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldBeSingular_Null_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<string> actual = null;
            AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<string>), null, msg => actual.ShouldBeSingular(msg));
        }

        [Test]
        public void ShouldBeLength_IsExpectedLength_ReturnsActualValue()
        {
            int[] actual = { 1, 2, 3 };

            Actual<int[]> result = actual.ShouldBeLength(3);

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldBeLength_IsDifferentLength_FailsWithLengthMismatchMessage()
        {
            int[] actual = { 1, 2, 3 };
            MockFormatter.LengthMismatch(2, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeLength(2, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldBeLength_Null_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<string> actual = null;
            AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<string>), null, msg => actual.ShouldBeLength(1, msg));
        }

        [Test]
        public void ShouldMatch_MatchingEnumerable_ReturnsActualValue()
        {
            int[] actual = { 1, 2 };
            int[] expected = { 1, 2 };
            Actual<IEnumerable<int>> result = actual.ShouldMatch(expected.AsEnumerable());

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldMatch_NonMatchingEnumerables_FailsWithEnumerablesDoNotMatchMessage()
        {
            int[] expected = { 3, 4 };
            int[] actual = { 1, 2 };
            MockFormatter.DoNotMatch(expected, actual, Compare.ObjectsMatch, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldMatch_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<string> actual = null;
            AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<string>), null, msg => actual.ShouldMatch(new string[] { }, msg));
        }

        [Test]
        public void ShouldMatch_ExpectedIsNull_FailsWithArgumentNullException()
        {
            string[] actual = { };

            ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => actual.ShouldMatch((IEnumerable<string>)null, "foo"));

            Assert.AreEqual("expected", result.ParamName);
        }

        [Test]
        public void ShouldMatchParams_MatchingParams_ReturnsActualValue()
        {
            int[] actual = { 1, 2, 3 };

            Actual<IEnumerable<int>> result = actual.ShouldMatch(1, 2, 3);

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldMatchParams_EnumerableDoesNotMatchParams_FailsWIthEnumerablesDoNotMatchMessage()
        {
            int[] actual = { 1, 2 };
            MockFormatter.DoNotMatch(ArgMatches(new[] { 3, 4 }), actual, Compare.ObjectsMatch).Returns("foo");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(3, 4));

            Assert.AreEqual("foo", result.Message);
        }

        [Test]
        public void ShouldMatchParams_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<string> actual = null;
            MockFormatter.NotEqual(typeof(IEnumerable<string>), null).Returns("foo");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch("bar"));

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
            int[] actual = { 1, 2, 4 };
            int[] expected = { 2, 4, 6 };
            Func<int, int, bool> predicate = (a, e) => a == e / 2;
            MockFormatter.DoNotMatch(expected, actual, predicate, "foo").Returns("bar");
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, predicate, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldMatchCustomEquality_ActualIsNull_FailsWithTypeNotEqualMessage()
        {
            IEnumerable<string> actual = null;
            MockFormatter.NotEqual(typeof(IEnumerable<string>), null, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(Enumerable.Empty<string>(), (s, s1) => false, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldMatch_FloatsWithinDelta_ReturnsActualValue()
        {
            float[] actual = { 10f, 20f };

            Actual<IEnumerable<float>> result = actual.ShouldMatch(new[] { 11f, 21f }, 1f);

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldMatch_FloatsOutsideDelta_FailsWithEnumerablesDoNotMatchMessage()
        {
            float[] actual = { 10f, 20f };
            float[] expected = { 11f, 21f };
            Func<float, float, bool> predicate = null;
            MockFormatter.DoNotMatch(expected, actual, Arg.Do<Func<float, float, bool>>(p => predicate = p), "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, 0.9f, "foo"));

            Assert.AreEqual("bar", result.Message);
            Assert.IsTrue(predicate(1f, 1.9f));
            Assert.IsFalse(predicate(1f, 2f));
        }

        [Test]
        public void ShouldMatch_FloatsActualIsNull_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<float> actual = null;
            AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<float>), null, msg => actual.ShouldMatch(new[] { 1f }, 1f, msg));
        }

        [Test]
        public void ShouldMatch_DoublesWithinDelta_ReturnsActualValue()
        {
            double[] actual = { 10d, 20d };

            Actual<IEnumerable<double>> result = actual.ShouldMatch(new[] { 11d, 21d }, 1d);

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldMatch_DoublesOutsideDelta_FailsWithEnumerablesDoNotMatchMessage()
        {
            double[] actual = { 10d, 20d };
            double[] expected = { 11d, 21d };
            Func<double, double, bool> predicate = null;
            MockFormatter.DoNotMatch(expected, actual, Arg.Do<Func<double, double, bool>>(p => predicate = p), "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, 0.9d, "foo"));

            Assert.AreEqual("bar", result.Message);
            Assert.IsTrue(predicate(1d, 1.9d));
            Assert.IsFalse(predicate(1d, 2d));
        }

        [Test]
        public void ShouldMatch_DoublesActualIsNull_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<double> actual = null;
            AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<double>), null, msg => actual.ShouldMatch(new[] { 1d }, 1d, msg));
        }

        [Test]
        public void ShouldStartWith_StartsWithExpected_ReturnsActualValue()
        {
            int[] actual = { 1, 2, 3 };

            Actual<IEnumerable<int>> result = actual.ShouldStartWith(new[] { 1, 2 });

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldStartWith_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<int> actual = null;
            AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldStartWith(Enumerable.Empty<object>(), msg));
        }

        [Test]
        public void ShouldStartWith_ExpectedIsNull_FailsWithArgumentNullException()
        {
            AssertArgumentNullException("expectedStart", () => new[] { 1 }.ShouldStartWith((IEnumerable<int>)null));
        }

        [Test]
        public void ShouldStartWith_DoesNotStartWithExpected_FailsWithDoesNotStartWithMessage()
        {
            int[] actual = { 1 };
            int[] expectedStart = { 2 };
            MockFormatter.DoesNotStartWith(expectedStart, actual, Equals, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldStartWith(expectedStart, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldContain_CollectionContainsExpected_ReturnsActualValue()
        {
            int[] actual = { 1, 2 };

            Actual<IEnumerable<int>> result = actual.ShouldContain(2);

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldContain_CollectionDoesNotContainExpected_FailsWithEnumerableDoesNotContainMessage()
        {
            int[] actual = { 1, 2 };
            const int expected = 3;
            MockFormatter.DoesNotContain(expected, actual, message: "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldContain(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldContain_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<int> actual = null;
            AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldContain(1, msg));
        }

        [Test]
        public void ShouldNotContain_CollectionDoesNotContainExpected_ReturnsActualValue()
        {
            IEnumerable<int> actual = Enumerable.Empty<int>();

            Actual<IEnumerable<int>> result = actual.ShouldNotContain(1);

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldNotContain_CollectionContainsItem_FailsWithEnumerableContainsItemMessage()
        {
            int[] actual = { 1 };
            MockFormatter.Contains(1, actual, message: "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotContain(1, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldNotContain_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<int> actual = null;
            AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldNotContain(1, msg));
        }

        [Test]
        public void ShouldContainItems_CollectionContainsAllItems_ReturnsActualValue()
        {
            int[] actual = { 1, 2, 3 };

            Actual<IEnumerable<int>> result = actual.ShouldContainItems(new[] { 2, 3, 1 });

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldContainItems_CollectionDoesNotContainAllItems_FailsWithEnumerableDoesNotContainItemsMessage()
        {
            int[] actual = { 1, 2, 3 };
            int[] expected = { 1, 4 };
            MockFormatter.DoesNotContainItems(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldContainItems(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldContainItems_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<int> actual = null;
            AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldContainItems(new[] { 1 }, msg));
        }

        [Test]
        public void ItemsShouldBeIn_ExpectedContainsAllItems_ReturnsActualValue()
        {
            int[] actual = { 1, 2 };

            Actual<IEnumerable<int>> result = actual.ItemsShouldBeIn(new[] { 3, 2, 1 });

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ItemsShouldBeIn_ContainsExtraItem_FailsWithCollectionContainsExtraItemMessage()
        {
            int[] actual = { 1, 2 };
            int[] expected = { 1 };
            MockFormatter.ContainsExtraItem(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ItemsShouldBeIn(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ItemsShouldBeIn_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<int> actual = null;
            AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ItemsShouldBeIn(new[] { 1 }, msg));
        }

        [Test]
        public void ItemsShouldBeIn_ExpectedIsNull_ThrowsArgumentNullException()
        {
            AssertArgumentNullException("expectedSuperset", () => new int[0].ItemsShouldBeIn((IEnumerable<int>)null));
        }

        [Test]
        public void ShouldNotContainItems_CollectionDoesNotContainAnyOfTheItems_ReturnsActualValue()
        {
            int[] actual = { 1, 2 };

            Actual<IEnumerable<int>> result = actual.ShouldNotContainItems(new[] { 3, 4 });

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldNotContainItems_CollectionContainsOneOfTheItems_FailsWithSequenceContainsItemMessage()
        {
            int[] actual = { 1, 2 };
            int[] expectedToNotContain = { 3, 2 };
            MockFormatter.Contains(expectedToNotContain, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotContainItems(expectedToNotContain, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldNotContainItems_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<int> actual = null;
            AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldNotContainItems(new[] { 1 }, msg));
        }

        [Test]
        public void ShouldOnlyOnlyContain_CollectionsHaveSameItems_ReturnsActualValue()
        {
            int[] actual = { 1, 2 };

            Actual<IEnumerable<int>> result = actual.ShouldOnlyContain(new[] { 2, 1 });

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldOnlyContain_MissingItem_FailsWithCollectionDoesNotOnlyContainMessage()
        {
            int[] actual = { 1, 2 };
            int[] expected = { 1, 3 };
            MockFormatter.DoesNotOnlyContain(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldOnlyContain(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldOnlyContain_ExtraItem_FailsWithCollectionDoesNotOnlyContainMessage()
        {
            int[] actual = { 1, 2 };
            int[] expected = { 1 };
            MockFormatter.DoesNotOnlyContain(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldOnlyContain(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldOnlyContain_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<int> actual = null;
            AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldOnlyContain(new[] { 1 }, msg));
        }

        [Test]
        public void ShouldNotOnlyContain_ActualHasExpectedPlusMore_ReturnsActualValue()
        {
            int[] actual = { 1, 2 };

            Actual<IEnumerable<int>> result = actual.ShouldNotOnlyContain(new[] { 2 });

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldNotOnlyContain_ActualOnlyHasItemsInExpected_FailsWithCollectionOnlyContainsMessage()
        {
            int[] actual = { 1, 2 };
            int[] expected = { 2, 1, 3 };
            MockFormatter.OnlyContains(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotOnlyContain(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldNotOnlyContain_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<int> actual = null;
            AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldNotOnlyContain(new[] { 1 }, msg));
        }

        [Test]
        public void ShouldNotOnlyContain_ExpectedIsNull_ThrowsArgumentNullException()
        {
            AssertArgumentNullException("expected", () => new[] { 1 }.ShouldNotOnlyContain((int[])null));
        }

        [Test]
        public void ShouldMatchReferences_SameItems_ReturnsActualValue()
        {
            object a = new object();
            object b = new object();
            object[] actual = { a, b };

            Actual<IEnumerable<object>> result = actual.ShouldMatchReferences(new[] { a, b }.AsEnumerable());

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldMatchReferences_DifferentLength_FailsWithEnumerableLenthMismatchMessage()
        {
            object[] actual = { };
            object[] expected = { new object() };
            MockFormatter.LengthMismatch(1, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatchReferences(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldMatchReferences_DifferentItems_FailsWithEnumerablesNotSameMessage()
        {
            object a = new object();
            object[] actual = { a, new object() };
            object[] expected = { a, new object() };
            MockFormatter.ItemsNotSame(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatchReferences(expected, "foo"));

            Assert.AreSame("bar", result.Message);
        }

        [Test]
        public void ShouldMatchReferences_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<object> actual = null;
            AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<object>), null, msg => actual.ShouldMatchReferences(new[] { new object() }, msg));
        }

        [Test]
        public void ShouldMatchReferencesParams_SameItems_ReturnsActualValue()
        {
            object a = new object();
            object b = new object();
            object[] actual = { a, b };

            Actual<IEnumerable<object>> result = actual.ShouldMatchReferences(a, b);

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldMatchReferencesParams_DifferentLength_FailsWithEnumerableLenthMismatchMessage()
        {
            object[] actual = { };
            MockFormatter.LengthMismatch(1, actual).Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatchReferences(new object()));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldMatchReferencesParams_DifferentItems_FailsWithEnumerablesNotSameMessage()
        {
            object a = new object();
            object[] actual = { a, new object() };
            object[] expected = { a, new object() };
            MockFormatter.ItemsNotSame(expected, actual).Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatchReferences(expected));

            Assert.AreSame("bar", result.Message);
        }

        [Test]
        public void ShouldMatchReferencesParams_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<object> actual = null;
            MockFormatter.NotEqual(typeof(IEnumerable<object>), null).Returns("foo");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatchReferences(new object()));

            Assert.AreEqual("foo", result.Message);
        }

        [Test]
        public void KeyedCollectionShouldContainKey_ContainsKeys_ReturnsActualValue()
        {
            TestKeyedCollection actual = new TestKeyedCollection { "foo", "bar" };

            Actual<KeyedCollection<char, string>> result = actual.ShouldContainKey('b');

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void KeyedCollectionShouldContainKey_DoesNotContainKey_FailsWithDoesNotContainKeyMessage()
        {
            TestKeyedCollection actual = new TestKeyedCollection();
            MockFormatter.DoesNotContain('a', actual, "key", "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldContainKey('a', "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void KeyedCollectionShouldContainKey_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            TestKeyedCollection actual = null;
            AssertFailsWithTypesNotEqualMessage(typeof(KeyedCollection<char, string>), null, msg => actual.ShouldContainKey('a', msg));
        }

        [Test]
        public void KeyedCollectionShouldNotContainKey_DoesNotContainKey_ReturnsActualValue()
        {
            TestKeyedCollection actual = new TestKeyedCollection();

            Actual<KeyedCollection<char, string>> result = actual.ShouldNotContainKey('a');

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void KeyedCollectionShouldNotContainKey_ContainsKey_FailsWithContainsKeyMessage()
        {
            TestKeyedCollection actual = new TestKeyedCollection { "foo" };
            MockFormatter.Contains('f', actual, "key", "bar").Returns("baz");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotContainKey('f', "bar"));

            Assert.AreEqual("baz", result.Message);
        }

        [Test]
        public void KeyedCollectionShouldNotContainKey_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            TestKeyedCollection actual = null;
            AssertFailsWithTypesNotEqualMessage(typeof(KeyedCollection<char, string>), null, msg => actual.ShouldNotContainKey('a', msg));
        }

        [Test]
        public void ItemsSatisfy_ItemsSatisfyAssertions_ReturnsActualValue()
        {
            int[] actual = { 1 };

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
            int[] actual = { 1 };
            MockFormatter.LengthMismatch(2, actual).Returns("foo");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ItemsSatisfy(i => { }, i => { }));

            Assert.AreEqual("foo", result.Message);
        }

        [Test]
        public void ItemsSatisfy_ItemDoesNotSatisyItsAssertion_FailsWithThrownException()
        {
            int[] actual = { 1, 2 };
            Exception failure = new Exception("foo");

            Exception result = Assert.Throws<Exception>(() => actual.ItemsSatisfy(
                i => { },
                i => { throw failure; }));

            Assert.AreSame(failure, result);
        }

        [Test]
        public void ItemsSatisfy_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<int> actual = null;
            MockFormatter.NotEqual(typeof(IEnumerable<int>), null).Returns("foo");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ItemsSatisfy(i => { }));

            Assert.AreEqual("foo", result.Message);
        }

        [Test]
        public void AllItemsSatisfy_AllItemsSatisfyAssertion_ReturnsActualValue()
        {
            int[] actual = { 1 };

            Actual<IEnumerable<int>> result = actual.AllItemsSatisfy(i => { });

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void AllItemsSatify_ItemsDoNotSatisfyAssertion_FailsWithThrownException()
        {
            int[] actual = { 1, 2 };
            Exception failure = new Exception("foo");

            Exception result = Assert.Throws<Exception>(() => actual.AllItemsSatisfy(i =>
            {
                if (i > 1) throw failure;
            }));

            Assert.AreSame(failure, result);
        }

        [Test]
        public void AllItemsStatisfy_ActualIsNull_FailsWithTypesNotEqualMessage()
        {
            IEnumerable<int> actual = null;
            MockFormatter.NotEqual(typeof(IEnumerable<int>), null).Returns("foo");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.AllItemsSatisfy(i => { }));

            Assert.AreEqual("foo", result.Message);
        }

        [Test]
        public void AllItemsSatisfy_AssertionIsNull_FailsWithArgumentNullException()
        {
            ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => new int[0].AllItemsSatisfy(null));

            Assert.AreEqual("assertion", result.ParamName);
        }

        private static IEnumerable<TItem> ArgMatches<TItem>(IEnumerable<TItem> expected)
        {
            return Arg.Is<IEnumerable<TItem>>(arg => Compare.CollectionsMatch(arg, expected, Compare.ObjectsAreEqual));
        }

        private class TestKeyedCollection : KeyedCollection<char, string>
        {
            protected override char GetKeyForItem(string item)
            {
                return item[0];
            }
        }
    }
}
