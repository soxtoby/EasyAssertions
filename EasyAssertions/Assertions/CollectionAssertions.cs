using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasyAssertions
{
    /// <summary>
    /// Collection-related assertions.
    /// </summary>
    public static class CollectionAssertions
    {
        /// <summary>
        /// Asserts that a sequence has no elements in it.
        /// </summary>
        public static Actual<TActual> ShouldBeEmpty<TActual>(this TActual actual, string message = null) where TActual : IEnumerable
        {
            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<TActual>(actual, message);
                    if (!Compare.IsEmpty(actual))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.NotEmpty(actual, message));
                });
        }

        /// <summary>
        /// Asserts that a sequence contains at least one element.
        /// </summary>
        public static Actual<TActual> ShouldNotBeEmpty<TActual>(this TActual actual, string message = null) where TActual : IEnumerable
        {
            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<TActual>(actual, message);
                    if (Compare.IsEmpty(actual))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.IsEmpty(message));
                });
        }

        /// <summary>
        /// Asserts that a sequnce contains exactly one element.
        /// </summary>
        public static Actual<TActual> ShouldBeSingular<TActual>(this TActual actual, string message = null) where TActual : IEnumerable
        {
            return actual.RegisterAssert(() => AssertLength(actual, 1, message));
        }

        /// <summary>
        /// Asserts that a sequence contains a specific number of elements.
        /// </summary>
        public static Actual<TActual> ShouldBeLength<TActual>(this TActual actual, int expectedLength, string message = null) where TActual : IEnumerable
        {
            return actual.RegisterAssert(() => AssertLength(actual, expectedLength, message));
        }

        private static void AssertLength<TActual>(TActual actual, int expectedLength, string message) where TActual : IEnumerable
        {
            ObjectAssertions.AssertType<TActual>(actual, message);
            if (actual.Cast<object>().Count() != expectedLength)
                throw EasyAssertion.Failure(FailureMessageFormatter.Current.LengthMismatch(expectedLength, actual, message));
        }

        /// <summary>
        /// Asserts that two sequences contain the same items in the same order.
        /// <see cref="IEnumerable"/> items are compared recursively.
        /// Non-<c>IEnumerable</c> items are compared using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldMatch<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) where TExpected : TActual
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));

            return actual.RegisterAssert(() => AssertMatch(actual, expected, Compare.ObjectsMatch, message));
        }

        /// <summary>
        /// Asserts that two sequences contain the same <see cref="float"/> values (within a specified tolerance), in the same order.
        /// </summary>
        public static Actual<IEnumerable<float>> ShouldMatch(this IEnumerable<float> actual, IEnumerable<float> expected, float delta, string message = null)
        {
            return actual.RegisterAssert(() => AssertMatch(actual, expected, (a, e) => Compare.AreWithinTolerance(a, e, delta), message));
        }

        /// <summary>
        /// Asserts that two sequences contain the same <see cref="double"/> values (within a specified tolerance), in the same order.
        /// </summary>
        public static Actual<IEnumerable<double>> ShouldMatch(this IEnumerable<double> actual, IEnumerable<double> expected, double delta, string message = null)
        {
            return actual.RegisterAssert(() => AssertMatch(actual, expected, (a, e) => Compare.AreWithinTolerance(a, e, delta), message));
        }

        /// <summary>
        /// Asserts that two sequences contain the same items in the same order, using a custom equality function.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldMatch<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, Func<TActual, TExpected, bool> predicate, string message = null)
        {
            return actual.RegisterAssert(() => AssertMatch(actual, expected, predicate, message));
        }

        private static void AssertMatch<TActual, TExpected>(IEnumerable<TActual> actual, IEnumerable<TExpected> expected, Func<TActual, TExpected, bool> predicate, string message = null)
        {
            ObjectAssertions.AssertType<IEnumerable<TActual>>(actual, message);

            List<TActual> actualList = actual.ToList();
            List<TExpected> expectedList = expected.ToList();

            if (!Compare.CollectionsMatch(actualList, expectedList, (a, e) => predicate((TActual)a, (TExpected)e)))
                throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoNotMatch(expected, actual, predicate, message));
        }

        /// <summary>
        /// Asserts that a sequence starts with another sub-sequence, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldStartWith<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expectedStart, string message = null)
        {
            if (expectedStart == null) throw new ArgumentNullException(nameof(expectedStart));

            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<IEnumerable<TActual>>(actual, message);

                    List<object> actualList = actual.Cast<object>().ToList();
                    List<object> expectedList = expectedStart.Cast<object>().ToList();

                    if (!Compare.CollectionStartsWith(actualList, expectedList, Compare.ObjectsAreEqual))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoesNotStartWith(expectedStart, actual, Compare.ObjectsAreEqual, message));
                });
        }

        public static Actual<IEnumerable<TActual>> ShouldEndWith<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expectedEnd, string message = null)
        {
            if (expectedEnd == null) throw new ArgumentNullException(nameof(expectedEnd));

            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<IEnumerable<TActual>>(actual, message);

                    List<object> actualList = actual.Cast<object>().ToList();
                    List<object> expectedList = expectedEnd.Cast<object>().ToList();

                    if (!Compare.CollectionEndsWith(actualList, expectedList, Compare.ObjectsAreEqual))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoesNotEndWith(expectedEnd, actual, Compare.ObjectsAreEqual, message));
                });
        }

        /// <summary>
        /// Asserts that a sequence contains a specified element, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldContain<TActual, TExpected>(this IEnumerable<TActual> actual, TExpected expected, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<IEnumerable<TActual>>(actual, message);
                    if (!actual.Contains(expected))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoesNotContain(expected, actual, message: message));
                });
        }

        /// <summary>
        /// Asserts that a sequence contains all specified elements, in any order, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldContainItems<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<IEnumerable<TActual>>(actual, message);
                    if (!Compare.ContainsAllItems(actual, expected))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoesNotContainItems(expected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that all elements in a sequence are contained within another sequence, in any order, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ItemsShouldBeIn<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expectedSuperset, string message = null) where TExpected : TActual
        {
            if (expectedSuperset == null) throw new ArgumentNullException(nameof(expectedSuperset));

            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<IEnumerable<TActual>>(actual, message);

                    if (!Compare.ContainsAllItems(expectedSuperset, actual))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.ContainsExtraItem(expectedSuperset, actual, message));
                });
        }

        /// <summary>
        /// Asserts that a sequence does not contain a specified element, using the default equality comparer.
        /// </summary>
        public static Actual<TActual> ShouldNotContain<TActual, TItem>(this TActual actual, TItem expectedToNotContain, string message = null) where TActual : IEnumerable<TItem>
        {
            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<TActual>(actual, message);
                    if (actual.Contains(expectedToNotContain))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.Contains(expectedToNotContain, actual, message: message));
                });
        }

        /// <summary>
        /// Asserts that a sequence does not contain any of the specified elements, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldNotContainItems<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expectedToNotContain, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<IEnumerable<TActual>>(actual, message);
                    if (Compare.ContainsAny(actual, expectedToNotContain))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.Contains(expectedToNotContain, actual, message));
                });
        }

        /// <summary>
        /// Asserts thats a sequence only contains the specified elements, and nothing else, in any order, with no duplicates, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldOnlyContain<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<IEnumerable<TActual>>(actual, message);
                    if (!Compare.ContainsOnlyExpectedItems(actual, expected))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoesNotOnlyContain(expected, actual, message));
                    AssertDistinct(actual, message);
                });
        }

        /// <summary>
        /// Asserts that a sequence contains at least one item that does appear in another specified sequence, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldNotOnlyContain<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) where TExpected : TActual
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));

            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<IEnumerable<TActual>>(actual, message);

                    if (Compare.ContainsAllItems(expected, actual))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.OnlyContains(expected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that a sequence contains no duplicated elements, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldBeDistinct<TActual>(this IEnumerable<TActual> actual, string message = null)
        {
            return actual.RegisterAssert(() => AssertDistinct(actual, message));
        }

        private static void AssertDistinct<TActual>(IEnumerable<TActual> actual, string message)
        {
            ObjectAssertions.AssertType<IEnumerable<TActual>>(actual, message);

            if (actual.GroupBy(i => i).Any(group => @group.Count() > 1))
                throw EasyAssertion.Failure(FailureMessageFormatter.Current.ContainsDuplicate(actual, message));
        }

        /// <summary>
        /// Asserts that two sequences contain the same object instances in the same order.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldMatchReferences<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() => {
                ObjectAssertions.AssertType<IEnumerable<TActual>>(actual, message);

                List<TActual> actualList = actual.ToList();
                List<TExpected> expectedList = expected.ToList();

                if (actualList.Count != expectedList.Count)
                    throw EasyAssertion.Failure(FailureMessageFormatter.Current.LengthMismatch(expectedList.Count, actual, message));

                if (!Compare.CollectionsMatch(actual, expected, ReferenceEquals))
                    throw EasyAssertion.Failure(FailureMessageFormatter.Current.ItemsNotSame(expected, actual, message));
            });
        }

        /// <summary>
        /// Asserts that a <see cref="KeyedCollection{TKey,TItem}"/> contains an item for the specified key.
        /// </summary>
        public static Actual<KeyedCollection<TKey, TItem>> ShouldContainKey<TKey, TItem>(this KeyedCollection<TKey, TItem> actual, TKey expectedKey, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<KeyedCollection<TKey, TItem>>(actual, message);
                    if (!actual.Contains(expectedKey))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoesNotContain(expectedKey, actual, "key", message));
                });
        }

        /// <summary>
        /// Asserts that a <see cref="KeyedCollection{TKey,TItem}"/> does not contain an item for the specified key.
        /// </summary>
        public static Actual<KeyedCollection<TKey, TItem>> ShouldNotContainKey<TKey, TItem>(this KeyedCollection<TKey, TItem> actual, TKey notExpectedKey, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<KeyedCollection<TKey, TItem>>(actual, message);
                    if (actual.Contains(notExpectedKey))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.Contains(notExpectedKey, actual, "key", message));
                });
        }

        /// <summary>
        /// Asserts that a sequence of items satisfies a matched sequence of assertions.
        /// </summary>
        public static Actual<IEnumerable<TItem>> ItemsSatisfy<TItem>(this IEnumerable<TItem> actual, params Action<TItem>[] assertions)
        {
            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<IEnumerable<TItem>>(actual);

                    List<TItem> actualList = actual.ToList();
                    if (actualList.Count != assertions.Length)
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.LengthMismatch(assertions.Length, actual));

                    for (int i = 0; i < assertions.Length; i++)
                        EasyAssertion.RegisterIndexedAssert(i, assertions[i].Method, () => assertions[i](actualList[i]));
                });
        }

        /// <summary>
        /// Asserts that all items in a sequence satisfy a specified assertion.
        /// </summary>
        public static Actual<IEnumerable<TItem>> AllItemsSatisfy<TItem>(this IEnumerable<TItem> actual, Action<TItem> assertion)
        {
            if (assertion == null) throw new ArgumentNullException(nameof(assertion));

            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<IEnumerable<TItem>>(actual);

                    int i = 0;
                    foreach (TItem item in actual)
                        EasyAssertion.RegisterIndexedAssert(i++, assertion.Method, () => assertion(item));
                });
        }
    }
}
