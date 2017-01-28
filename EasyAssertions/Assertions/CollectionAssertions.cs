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
        private const string UnorderedCollectionComparisonError = "Don't compare unordered collections with ordered sequences. Use an assertion that doesn't test ordering instead.";

        [Obsolete(UnorderedCollectionComparisonError, true)]
        public static void ShouldMatch<TActualKey, TActualValue, TExpectedKey, TExpectedValue>(this IDictionary<TActualKey, TActualValue> actual, IEnumerable<KeyValuePair<TExpectedKey, TExpectedValue>> expected, string message = null) { }
        [Obsolete(UnorderedCollectionComparisonError, true)]
        public static void ShouldMatch<TActualKey, TActualValue, TExpectedKey, TExpectedValue>(this IEnumerable<KeyValuePair<TActualKey, TActualValue>> actual, IDictionary<TExpectedKey, TExpectedValue> expected, string message = null) { }
        [Obsolete(UnorderedCollectionComparisonError, true)]
        public static void ShouldMatch<TActual, TExpected>(this ISet<TActual> actual, IEnumerable<TExpected> expected, string message = null) { }
        [Obsolete(UnorderedCollectionComparisonError, true)]
        public static void ShouldMatch<TActual, TExpected>(this IEnumerable<TActual> actual, ISet<TExpected> expected, string message = null) { }
        [Obsolete(UnorderedCollectionComparisonError, true)]
        public static void ShouldMatchReferences<TActualKey, TActualValue, TExpectedKey, TExpectedValue>(this IDictionary<TActualKey, TActualValue> actual, IEnumerable<KeyValuePair<TExpectedKey, TExpectedValue>> expected, string message = null) { }
        [Obsolete(UnorderedCollectionComparisonError, true)]
        public static void ShouldMatchReferences<TActualKey, TActualValue, TExpectedKey, TExpectedValue>(this IEnumerable<KeyValuePair<TActualKey, TActualValue>> actual, IDictionary<TExpectedKey, TExpectedValue> expected, string message = null) { }
        [Obsolete(UnorderedCollectionComparisonError, true)]
        public static void ShouldMatchReferences<TActual, TExpected>(this ISet<TActual> actual, IEnumerable<TExpected> expected, string message = null) { }
        [Obsolete(UnorderedCollectionComparisonError, true)]
        public static void ShouldMatchReferences<TActual, TExpected>(this IEnumerable<TActual> actual, ISet<TExpected> expected, string message = null) { }

        /// <summary>
        /// Asserts that a sequence has no elements in it.
        /// </summary>
        public static Actual<TActual> ShouldBeEmpty<TActual>(this TActual actual, string message = null) 
            where TActual : IEnumerable
        {
            return actual.RegisterAssertion(c =>
                {
                    actual.ShouldBeA<TActual>(message);

                    using (IBuffer<object> bufferedActual = actual.Buffer())
                    {
                        if (!c.Test.IsEmpty(bufferedActual))
                            throw c.StandardError.NotEmpty(bufferedActual, message);
                    }
                });
        }

        /// <summary>
        /// Asserts that a sequence contains at least one element.
        /// </summary>
        public static Actual<TActual> ShouldNotBeEmpty<TActual>(this TActual actual, string message = null) 
            where TActual : IEnumerable
        {
            return actual.RegisterAssertion(c =>
                {
                    actual.ShouldBeA<TActual>(message);
                    if (c.Test.IsEmpty(actual))
                        throw c.StandardError.IsEmpty(message);
                });
        }

        /// <summary>
        /// Asserts that a sequence contains exactly one element.
        /// </summary>
        public static Actual<TActual> ShouldBeSingular<TActual>(this TActual actual, string message = null) 
            where TActual : IEnumerable
        {
            return actual.RegisterAssertion(c => actual.ShouldBeLength(1, message));
        }

        /// <summary>
        /// Asserts that a sequence contains a specific number of elements.
        /// </summary>
        public static Actual<TActual> ShouldBeLength<TActual>(this TActual actual, int expectedLength, string message = null) 
            where TActual : IEnumerable
        {
            return actual.RegisterAssertion(c =>
                {
                    actual.ShouldBeA<TActual>(message);

                    using (IBuffer<object> bufferedActual = actual.Buffer())
                    {
                        if (bufferedActual.Count != expectedLength)
                            throw c.StandardError.LengthMismatch(expectedLength, bufferedActual, message);
                    }
                });
        }

        /// <summary>
        /// Asserts that two sequences contain the same items in the same order.
        /// <see cref="IEnumerable"/> items are compared recursively.
        /// Non-<c>IEnumerable</c> items are compared using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldMatch<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) 
            where TExpected : TActual
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));

            return actual.RegisterAssertion(c => actual.ShouldMatch(expected, c.Test.ObjectsMatch, message));
        }

        /// <summary>
        /// Asserts that two sequences contain the same <see cref="float"/> values (within a specified tolerance), in the same order.
        /// </summary>
        public static Actual<IEnumerable<float>> ShouldMatch(this IEnumerable<float> actual, IEnumerable<float> expected, float delta, string message = null)
        {
            return actual.RegisterAssertion(c => actual.ShouldMatch(expected, (a, e) => c.Test.AreWithinTolerance(a, e, delta), message));
        }

        /// <summary>
        /// Asserts that two sequences contain the same <see cref="double"/> values (within a specified tolerance), in the same order.
        /// </summary>
        public static Actual<IEnumerable<double>> ShouldMatch(this IEnumerable<double> actual, IEnumerable<double> expected, double delta, string message = null)
        {
            return actual.RegisterAssertion(c => actual.ShouldMatch(expected, (a, e) => c.Test.AreWithinTolerance(a, e, delta), message));
        }

        /// <summary>
        /// Asserts that two sequences contain the same items in the same order, using a custom equality function.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldMatch<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, Func<TActual, TExpected, bool> predicate, string message = null)
        {
            return actual.RegisterAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TActual>>(message);

                    using (IBuffer<TActual> bufferedActual = actual.Buffer())
                    using (IBuffer<TExpected> bufferedExpected = expected.Buffer())
                    {
                        if (!c.Test.CollectionsMatch(bufferedActual, bufferedExpected, (a, e) => predicate((TActual)a, (TExpected)e)))
                            throw c.StandardError.DoNotMatch(bufferedExpected, bufferedActual, predicate, message);
                    }
                });
        }

        /// <summary>
        /// Asserts that a sequence starts with another sub-sequence, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldStartWith<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expectedStart, string message = null) 
            where TExpected : TActual
        {
            if (expectedStart == null) throw new ArgumentNullException(nameof(expectedStart));

            return actual.RegisterAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TActual>>(message);

                    using (IBuffer<TActual> bufferedActual = actual.Buffer())
                    using (IBuffer<TExpected> bufferedExpected = expectedStart.Buffer())
                    {
                        if (!c.Test.CollectionStartsWith(bufferedActual, bufferedExpected, c.Test.ObjectsAreEqual))
                            throw c.StandardError.DoesNotStartWith(bufferedExpected, bufferedActual, c.Test.ObjectsAreEqual, message);
                    }
                });
        }

        public static Actual<IEnumerable<TActual>> ShouldEndWith<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expectedEnd, string message = null) 
            where TExpected : TActual
        {
            if (expectedEnd == null) throw new ArgumentNullException(nameof(expectedEnd));

            return actual.RegisterAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TActual>>(message);

                    using (IBuffer<TActual> bufferedActual = actual.Buffer())
                    using (IBuffer<TExpected> bufferedExpected = expectedEnd.Buffer())
                    {
                        if (!c.Test.CollectionEndsWith(bufferedActual, bufferedExpected, c.Test.ObjectsAreEqual))
                            throw c.StandardError.DoesNotEndWith(bufferedExpected, bufferedActual, c.Test.ObjectsAreEqual, message);
                    }
                });
        }

        /// <summary>
        /// Asserts that a sequence contains a specified element, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldContain<TActual, TExpected>(this IEnumerable<TActual> actual, TExpected expected, string message = null) 
            where TExpected : TActual
        {
            return actual.RegisterAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TActual>>(message);

                    using (IBuffer<TActual> bufferedActual = actual.Buffer())
                    {
                        if (!bufferedActual.Contains(expected))
                            throw c.StandardError.DoesNotContain(expected, bufferedActual, message: message);
                    }
                });
        }

        /// <summary>
        /// Asserts that a sequence contains all specified elements, in any order, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldContainItems<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) 
            where TExpected : TActual
        {
            return actual.RegisterAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TActual>>(message);

                    using (IBuffer<TActual> bufferedActual = actual.Buffer())
                    using (IBuffer<TExpected> bufferedExpected = expected.Buffer())
                    {
                        if (!c.Test.ContainsAllItems(bufferedActual, bufferedExpected))
                            throw c.StandardError.DoesNotContainItems(bufferedExpected, bufferedActual, message);
                    }
                });
        }

        /// <summary>
        /// Asserts that all elements in a sequence are contained within another sequence, in any order, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ItemsShouldBeIn<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expectedSuperset, string message = null) 
            where TExpected : TActual
        {
            if (expectedSuperset == null) throw new ArgumentNullException(nameof(expectedSuperset));

            return actual.RegisterAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TActual>>(message);

                    using (IBuffer<TActual> bufferedActual = actual.Buffer())
                    using (IBuffer<TExpected> bufferedExpected = expectedSuperset.Buffer())
                    {
                        if (!c.Test.ContainsAllItems(bufferedExpected, bufferedActual))
                            throw c.StandardError.ContainsExtraItem(bufferedExpected, bufferedActual, message);
                    }
                });
        }

        /// <summary>
        /// Asserts that a sequence does not contain a specified element, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldNotContain<TActual, TExpected>(this IEnumerable<TActual> actual, TExpected expectedToNotContain, string message = null) 
            where TExpected : TActual
        {
            return actual.RegisterAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TActual>>(message);

                    using (IBuffer<TActual> bufferedActual = actual.Buffer())
                    {
                        if (bufferedActual.Contains(expectedToNotContain))
                            throw c.StandardError.Contains(expectedToNotContain, bufferedActual, message: message);
                    }
                });
        }

        /// <summary>
        /// Asserts that a sequence does not contain any of the specified elements, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldNotContainItems<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expectedToNotContain, string message = null) 
            where TExpected : TActual
        {
            return actual.RegisterAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TActual>>(message);

                    using (IBuffer<TActual> bufferedActual = actual.Buffer())
                    using (IBuffer<TExpected> bufferedExpected = expectedToNotContain.Buffer())
                    {
                        if (c.Test.ContainsAny(bufferedActual, bufferedExpected))
                            throw c.StandardError.Contains(bufferedExpected, bufferedActual, message);
                    }
                });
        }

        /// <summary>
        /// Asserts thats a sequence only contains the specified elements, and nothing else, in any order, with no duplicates, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldOnlyContain<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) 
            where TExpected : TActual
        {
            return actual.RegisterAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TActual>>(message);

                    using (IBuffer<TActual> bufferedActual = actual.Buffer())
                    using (IBuffer<TExpected> bufferedExpected = expected.Buffer())
                    {
                        if (!c.Test.ContainsOnlyExpectedItems(bufferedActual, bufferedExpected))
                            throw c.StandardError.DoesNotOnlyContain(bufferedExpected, bufferedActual, message);

                        AssertDistinct(c, bufferedActual, message);
                    }
                });
        }

        /// <summary>
        /// Asserts that a sequence contains at least one item that does appear in another specified sequence, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldNotOnlyContain<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) 
            where TExpected : TActual
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));

            return actual.RegisterAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TActual>>(message);

                    using (IBuffer<TActual> bufferedActual = actual.Buffer())
                    using (IBuffer<TExpected> bufferedExpected = expected.Buffer())
                    {
                        if (c.Test.ContainsAllItems(bufferedExpected, bufferedActual))
                            throw c.StandardError.OnlyContains(bufferedExpected, bufferedActual, message);
                    }
                });
        }

        /// <summary>
        /// Asserts that a sequence contains no duplicated elements, using the default equality comparer.
        /// </summary>
        public static Actual<TActual> ShouldBeDistinct<TActual>(this TActual actual, string message = null) 
            where TActual : IEnumerable
        {
            return actual.RegisterAssertion(c => AssertDistinct(c, actual, message));
        }

        private static void AssertDistinct<TActual>(AssertionContext assertionContext, TActual actual, string message) 
            where TActual : IEnumerable
        {
            actual.ShouldBeA<TActual>(message);

            using (IBuffer<object> bufferedActual = actual.Buffer())
            {
                if (bufferedActual.GroupBy(i => i).Any(group => group.Count() > 1))
                    throw assertionContext.StandardError.ContainsDuplicate(bufferedActual, message);
            }
        }

        /// <summary>
        /// Asserts that two sequences contain the same object instances in the same order.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldMatchReferences<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) 
            where TExpected : TActual
        {
            return actual.RegisterAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TActual>>(message);

                    using (IBuffer<TActual> bufferedActual = actual.Buffer())
                    using (IBuffer<TExpected> bufferedExpected = expected.Buffer())
                    {
                        if (bufferedActual.Count != bufferedExpected.Count)
                            throw c.StandardError.LengthMismatch(bufferedExpected.Count, bufferedActual, message);

                        if (!c.Test.CollectionsMatch(bufferedActual, bufferedExpected, ReferenceEquals))
                            throw c.StandardError.ItemsNotSame(bufferedExpected, bufferedActual, message);
                    }
                });
        }

        /// <summary>
        /// Asserts that a <see cref="KeyedCollection{TKey,TItem}"/> contains an item for the specified key.
        /// </summary>
        public static Actual<KeyedCollection<TKey, TItem>> ShouldContainKey<TKey, TItem>(this KeyedCollection<TKey, TItem> actual, TKey expectedKey, string message = null)
        {
            return actual.RegisterAssertion(c =>
                {
                    actual.ShouldBeA<KeyedCollection<TKey, TItem>>(message);
                    if (!actual.Contains(expectedKey))
                        throw c.StandardError.DoesNotContain(expectedKey, actual, "key", message);
                });
        }

        /// <summary>
        /// Asserts that a <see cref="KeyedCollection{TKey,TItem}"/> does not contain an item for the specified key.
        /// </summary>
        public static Actual<KeyedCollection<TKey, TItem>> ShouldNotContainKey<TKey, TItem>(this KeyedCollection<TKey, TItem> actual, TKey notExpectedKey, string message = null)
        {
            return actual.RegisterAssertion(c =>
                {
                    actual.ShouldBeA<KeyedCollection<TKey, TItem>>(message);
                    if (actual.Contains(notExpectedKey))
                        throw c.StandardError.Contains(notExpectedKey, actual, "key", message);
                });
        }

        /// <summary>
        /// Asserts that a sequence of items satisfies a matched sequence of assertions.
        /// </summary>
        public static Actual<IEnumerable<TItem>> ItemsSatisfy<TItem>(this IEnumerable<TItem> actual, params Action<TItem>[] assertions)
        {
            return actual.RegisterAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TItem>>(null);

                    using (IBuffer<TItem> bufferedActual = actual.Buffer())
                    {
                        if (bufferedActual.Count != assertions.Length)
                            throw c.StandardError.LengthMismatch(assertions.Length, bufferedActual);

                        for (int i = 0; i < assertions.Length; i++)
                            actual.RegisterIndexedAssertion(i, c2 => actual.RegisterUserAssertion(assertions[i], () => assertions[i](bufferedActual[i])));
                    }
                });
        }

        /// <summary>
        /// Asserts that all items in a sequence satisfy a specified assertion.
        /// </summary>
        public static Actual<IEnumerable<TItem>> AllItemsSatisfy<TItem>(this IEnumerable<TItem> actual, Action<TItem> assertion)
        {
            if (assertion == null) throw new ArgumentNullException(nameof(assertion));

            return actual.RegisterAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TItem>>();

                    int i = 0;
                    foreach (TItem item in actual)
                        item.RegisterIndexedAssertion(i, c2 => item.RegisterUserAssertion(assertion, () => assertion(item)));
                });
        }
    }
}
