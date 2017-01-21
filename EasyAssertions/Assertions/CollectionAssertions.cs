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

                    using (IBuffer<object> bufferedActual = actual.Buffer())
                    {
                        if (!Compare.IsEmpty(bufferedActual))
                            throw EasyAssertion.Failure(FailureMessageFormatter.Current.NotEmpty(bufferedActual, message));
                    }
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

            using (IBuffer<object> bufferedActual = actual.Buffer())
            {
                if (bufferedActual.Count() != expectedLength)
                    throw EasyAssertion.Failure(FailureMessageFormatter.Current.LengthMismatch(expectedLength, bufferedActual, message));
            }
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

            using (IBuffer<TActual> bufferedActual = actual.Buffer())
            using (IBuffer<TExpected> bufferedExpected = expected.Buffer())
            {
                if (!Compare.CollectionsMatch(bufferedActual, bufferedExpected, (a, e) => predicate((TActual)a, (TExpected)e)))
                    throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoNotMatch(bufferedExpected, bufferedActual, predicate, message));
            }
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

                    using (IBuffer<TActual> bufferedActual = actual.Buffer())
                    using (IBuffer<TExpected> bufferedExpected = expectedStart.Buffer())
                    {
                        if (!Compare.CollectionStartsWith(bufferedActual, bufferedExpected, Compare.ObjectsAreEqual))
                            throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoesNotStartWith(bufferedExpected, bufferedActual, Compare.ObjectsAreEqual, message));
                    }
                });
        }

        public static Actual<IEnumerable<TActual>> ShouldEndWith<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expectedEnd, string message = null)
        {
            if (expectedEnd == null) throw new ArgumentNullException(nameof(expectedEnd));

            return actual.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<IEnumerable<TActual>>(actual, message);

                    using (IBuffer<TActual> bufferedActual = actual.Buffer())
                    using (IBuffer<TExpected> bufferedExpected = expectedEnd.Buffer())
                    {
                        if (!Compare.CollectionEndsWith(bufferedActual, bufferedExpected, Compare.ObjectsAreEqual))
                            throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoesNotEndWith(bufferedExpected, bufferedActual, Compare.ObjectsAreEqual, message));
                    }
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

                    using (IBuffer<TActual> bufferedActual = actual.Buffer())
                    {
                        if (!bufferedActual.Contains(expected))
                            throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoesNotContain(expected, bufferedActual, message: message));
                    }
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

                    using (IBuffer<TActual> bufferedActual = actual.Buffer())
                    using (IBuffer<TExpected> bufferedExpected = expected.Buffer())
                    {
                        if (!Compare.ContainsAllItems(bufferedActual, bufferedExpected))
                            throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoesNotContainItems(bufferedExpected, bufferedActual, message));
                    }
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

                    using (IBuffer<TActual> bufferedActual = actual.Buffer())
                    using (IBuffer<TExpected> bufferedExpected = expectedSuperset.Buffer())
                    {
                        if (!Compare.ContainsAllItems(bufferedExpected, bufferedActual))
                            throw EasyAssertion.Failure(FailureMessageFormatter.Current.ContainsExtraItem(bufferedExpected, bufferedActual, message));
                    }
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

                    using (IBuffer<TItem> bufferedActual = actual.Buffer())
                    {
                        if (bufferedActual.Contains(expectedToNotContain))
                            throw EasyAssertion.Failure(FailureMessageFormatter.Current.Contains(expectedToNotContain, bufferedActual, message: message));
                    }
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

                    using (IBuffer<TActual> bufferedActual = actual.Buffer())
                    using (IBuffer<TExpected> bufferedExpected = expectedToNotContain.Buffer())
                    {
                        if (Compare.ContainsAny(bufferedActual, bufferedExpected))
                            throw EasyAssertion.Failure(FailureMessageFormatter.Current.Contains(bufferedExpected, bufferedActual, message));
                    }
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

                    using (IBuffer<TActual> bufferedActual = actual.Buffer())
                    using (IBuffer<TExpected> bufferedExpected = expected.Buffer())
                    {
                        if (!Compare.ContainsOnlyExpectedItems(bufferedActual, bufferedExpected))
                            throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoesNotOnlyContain(bufferedExpected, bufferedActual, message));
                        AssertDistinct(bufferedActual, message);
                    }
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

                    using (IBuffer<TActual> bufferedActual = actual.Buffer())
                    using (IBuffer<TExpected> bufferedExpected = expected.Buffer())
                    {
                        if (Compare.ContainsAllItems(bufferedExpected, bufferedActual))
                            throw EasyAssertion.Failure(FailureMessageFormatter.Current.OnlyContains(bufferedExpected, bufferedActual, message));
                    }
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

            using (IBuffer<TActual> bufferedActual = actual.Buffer())
            {
                if (bufferedActual.GroupBy(i => i).Any(group => @group.Count() > 1))
                    throw EasyAssertion.Failure(FailureMessageFormatter.Current.ContainsDuplicate(bufferedActual, message));
            }
        }

        /// <summary>
        /// Asserts that two sequences contain the same object instances in the same order.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldMatchReferences<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() =>
            {
                ObjectAssertions.AssertType<IEnumerable<TActual>>(actual, message);

                using (IBuffer<TActual> bufferedActual = actual.Buffer())
                using (IBuffer<TExpected> bufferedExpected = expected.Buffer())
                {
                    if (bufferedActual.Count() != bufferedExpected.Count())
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.LengthMismatch(bufferedExpected.Count(), bufferedActual, message));

                    if (!Compare.CollectionsMatch(bufferedActual, bufferedExpected, ReferenceEquals))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.ItemsNotSame(bufferedExpected, bufferedActual, message));
                }
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

                    using (IBuffer<TItem> bufferedActual = actual.Buffer())
                    {
                        if (bufferedActual.Count() != assertions.Length)
                            throw EasyAssertion.Failure(FailureMessageFormatter.Current.LengthMismatch(assertions.Length, bufferedActual));

                        for (int i = 0; i < assertions.Length; i++)
                            EasyAssertion.RegisterIndexedAssert(i, assertions[i].Method, () => assertions[i](bufferedActual[i]));
                    }
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
