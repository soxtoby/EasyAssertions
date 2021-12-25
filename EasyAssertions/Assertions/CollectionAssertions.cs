﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace EasyAssertions
{
    /// <summary>
    /// Collection-related assertions.
    /// </summary>
    public static class CollectionAssertions
    {
        const string UnorderedCollectionComparisonError = "Do not compare unordered collections with ordered sequences. Use an assertion that does not test ordering instead.";
        const string AssertionReturnsBooleanError = "Assertion callbacks should use assertions rather than returning booleans.";

        /// <summary>Disallowed comparison.</summary>
        [Obsolete(UnorderedCollectionComparisonError, true)]
        public static void ShouldMatch<TActualKey, TActualValue, TExpectedKey, TExpectedValue>(this IDictionary<TActualKey, TActualValue>? actual, IEnumerable<KeyValuePair<TExpectedKey, TExpectedValue>>? expected, string? message = null) { }
        /// <summary>Disallowed comparison.</summary>
        [Obsolete(UnorderedCollectionComparisonError, true)]
        public static void ShouldMatch<TActualKey, TActualValue, TExpectedKey, TExpectedValue>(this IEnumerable<KeyValuePair<TActualKey, TActualValue>>? actual, IDictionary<TExpectedKey, TExpectedValue>? expected, string? message = null) { }
        /// <summary>Disallowed comparison.</summary>
        [Obsolete(UnorderedCollectionComparisonError, true)]
        public static void ShouldMatch<TActual, TExpected>(this ISet<TActual>? actual, IEnumerable<TExpected>? expected, string? message = null) { }
        /// <summary>Disallowed comparison.</summary>
        [Obsolete(UnorderedCollectionComparisonError, true)]
        public static void ShouldMatch<TActual, TExpected>(this IEnumerable<TActual>? actual, ISet<TExpected>? expected, string? message = null) { }
        /// <summary>Disallowed comparison.</summary>
        [Obsolete(UnorderedCollectionComparisonError, true)]
        public static void ShouldMatchReferences<TActualKey, TActualValue, TExpectedKey, TExpectedValue>(this IDictionary<TActualKey, TActualValue>? actual, IEnumerable<KeyValuePair<TExpectedKey, TExpectedValue>>? expected, string? message = null) { }
        /// <summary>Disallowed comparison.</summary>
        [Obsolete(UnorderedCollectionComparisonError, true)]
        public static void ShouldMatchReferences<TActualKey, TActualValue, TExpectedKey, TExpectedValue>(this IEnumerable<KeyValuePair<TActualKey, TActualValue>>? actual, IDictionary<TExpectedKey, TExpectedValue>? expected, string? message = null) { }
        /// <summary>Disallowed comparison.</summary>
        [Obsolete(UnorderedCollectionComparisonError, true)]
        public static void ShouldMatchReferences<TActual, TExpected>(this ISet<TActual>? actual, IEnumerable<TExpected>? expected, string? message = null) { }
        /// <summary>Disallowed comparison.</summary>
        [Obsolete(UnorderedCollectionComparisonError, true)]
        public static void ShouldMatchReferences<TActual, TExpected>(this IEnumerable<TActual>? actual, ISet<TExpected>? expected, string? message = null) { }
        /// <summary>Disallowed comparison.</summary>
        [Obsolete(AssertionReturnsBooleanError, true)]
        public static void ItemsSatisfy<TItem>(this IEnumerable<TItem>? actual, params Func<TItem, bool>[] assertions) { }
        /// <summary>Disallowed comparison.</summary>
        [Obsolete(AssertionReturnsBooleanError, true)]
        public static void AllItemsSatisfy<TItem>(this IEnumerable<TItem>? actual, Func<TItem, bool> assertion) { }

        /// <summary>
        /// Asserts that a sequence has no elements in it.
        /// </summary>
        public static Actual<TActual> ShouldBeEmpty<TActual>([NotNull] this TActual? actual, string? message = null)
            where TActual : IEnumerable
        {
            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<TActual>(message);

                    using var bufferedActual = actual.Buffer();

                    if (!c.Test.IsEmpty(bufferedActual))
                        throw c.StandardError.NotEmpty(bufferedActual, message);
                });
        }

        /// <summary>
        /// Asserts that a sequence contains at least one element.
        /// </summary>
        public static Actual<TActual> ShouldNotBeEmpty<TActual>([NotNull] this TActual? actual, string? message = null)
            where TActual : IEnumerable
        {
            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<TActual>(message);
                    if (c.Test.IsEmpty(actual))
                        throw c.StandardError.IsEmpty(message);
                });
        }

        /// <summary>
        /// Asserts that a sequence contains exactly one element.
        /// </summary>
        public static Actual<TActual> ShouldBeSingular<TActual>([NotNull] this TActual? actual, string? message = null)
            where TActual : IEnumerable
        {
            return actual.RegisterNotNullAssertion(c => AssertLength(actual, 1, message, c));
        }

        /// <summary>
        /// Asserts that a sequence contains exactly one element of the specified type.
        /// </summary>
        public static Actual<TExpected> ShouldBeASingular<TExpected>([NotNull] this IEnumerable? actual, string? message = null)
            where TExpected : notnull
        {
            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable>(message);
                    using var bufferedActual = actual.Buffer();
                    bufferedActual.ShouldBeSingular(message);
                    ObjectAssertions.AssertType<TExpected>(bufferedActual.Single(), message, c);
                    return new Actual<TExpected>((TExpected)bufferedActual.Single());
                });
        }

        /// <summary>
        /// Assert that all items in a sequence are assignable to the specified type.
        /// </summary>
        public static Actual<IEnumerable<TExpected>> ShouldAllBeA<TExpected>([NotNull] this IEnumerable? actual, string? message = null)
            where TExpected : notnull
        {
            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable>(message);

                    using var bufferedActual = actual.Buffer();
                    var i = 0;
                    foreach (var item in bufferedActual)
                        actual.RegisterAssertion(c2 => item.ShouldBeA<TExpected>(message), $"[{i++}]");

                    return new Actual<IEnumerable<TExpected>>(actual.Cast<TExpected>());
                });
        }

        /// <summary>
        /// Asserts that a sequence contains a specific number of elements.
        /// </summary>
        public static Actual<TActual> ShouldBeLength<TActual>([NotNull] this TActual? actual, int expectedLength, string? message = null)
            where TActual : IEnumerable
        {
            return actual.RegisterNotNullAssertion(c => AssertLength(actual, expectedLength, message, c));
        }

        static void AssertLength<TActual>([NotNull] TActual? actual, int expectedLength, string? message, IAssertionContext c) where TActual : IEnumerable
        {
            actual.ShouldBeA<TActual>(message);

            using var bufferedActual = actual.Buffer();

            if (bufferedActual.Count != expectedLength)
                throw c.StandardError.LengthMismatch(expectedLength, bufferedActual, message);
        }

        /// <summary>
        /// Asserts that two sequences contain the same items in the same order.
        /// <see cref="IEnumerable"/> items are compared recursively.
        /// Non-<c>IEnumerable</c> items are compared using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldMatch<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, IEnumerable<TExpected> expected, string? message = null)
            where TExpected : TActual
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));

            return actual.RegisterNotNullAssertion(c => AssertMatch(actual, expected, c.Test.ObjectsMatch, message, c));
        }

        /// <summary>
        /// Asserts that two sequences contain the same <see cref="float"/> values (within a specified tolerance), in the same order.
        /// </summary>
        public static Actual<IEnumerable<float>> ShouldMatch([NotNull] this IEnumerable<float>? actual, IEnumerable<float> expected, double tolerance, string? message = null)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));

            return actual.RegisterNotNullAssertion(c => AssertMatch(actual, expected, (a, e) => c.Test.AreWithinTolerance(a, e, tolerance), message, c));
        }

        /// <summary>
        /// Asserts that two sequences contain the same <see cref="double"/> values (within a specified tolerance), in the same order.
        /// </summary>
        public static Actual<IEnumerable<double>> ShouldMatch([NotNull] this IEnumerable<double>? actual, IEnumerable<double> expected, double tolerance, string? message = null)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));

            return actual.RegisterNotNullAssertion(c => AssertMatch(actual, expected, (a, e) => c.Test.AreWithinTolerance(a, e, tolerance), message, c));
        }

        /// <summary>
        /// Asserts that two sequences contain the same items in the same order, using a custom equality function.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldMatch<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, IEnumerable<TExpected> expected, Func<TActual, TExpected, bool> predicate, string? message = null)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return actual.RegisterNotNullAssertion(c => AssertMatch(actual, expected, predicate, message, c));
        }

        static void AssertMatch<TActual, TExpected>([NotNull] IEnumerable<TActual>? actual, IEnumerable<TExpected> expected, Func<TActual, TExpected, bool> predicate, string? message, IAssertionContext c)
        {
            actual.ShouldBeA<IEnumerable<TActual>>(message);

            using var bufferedActual = actual.Buffer();
            using var bufferedExpected = expected.Buffer();

            if (!c.Test.CollectionsMatch(bufferedActual, bufferedExpected, (a, e) => predicate((TActual)a, (TExpected)e)))
                throw c.StandardError.DoNotMatch(bufferedExpected, bufferedActual, predicate, message);
        }

        /// <summary>
        /// Asserts that two sequences contain the same items in the same order, using a custom equality function.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldMatch<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, IEnumerable<TExpected> expected, Action<TActual, TExpected> assertion)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (assertion == null) throw new ArgumentNullException(nameof(assertion));

            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TActual>>();

                    using var bufferedActual = actual.Buffer();
                    using var bufferedExpected = expected.Buffer();

                    if (bufferedActual.Count != bufferedExpected.Count)
                        throw c.StandardError.LengthMismatch(bufferedExpected.Count, bufferedActual);

                    for (var i = 0; i < bufferedExpected.Count; i++)
                        c.Call(() => assertion(bufferedActual[i], bufferedExpected[i]), $"[{i}]", $"[{i}]");
                });
        }

        /// <summary>
        /// Asserts that two sequences do not contain the same items in the same order.
        /// <see cref="IEnumerable"/> items are compared recursively.
        /// Non-<c>IEnumerable</c> items are compared using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldNotMatch<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, IEnumerable<TExpected> notExpected, string? message = null)
            where TExpected : TActual
        {
            if (notExpected == null) throw new ArgumentNullException(nameof(notExpected));

            return actual.RegisterNotNullAssertion(c => AssertNotMatch(actual, notExpected, c.Test.ObjectsMatch, message, c));
        }

        /// <summary>
        /// Asserts that two sequences do not contain the same <see cref="float"/> values (within a specified tolerance), in the same order.
        /// </summary>
        public static Actual<IEnumerable<float>> ShouldNotMatch([NotNull] this IEnumerable<float>? actual, IEnumerable<float> notExpected, double tolerance, string? message = null)
        {
            if (notExpected == null) throw new ArgumentNullException(nameof(notExpected));

            return actual.RegisterNotNullAssertion(c => AssertNotMatch(actual, notExpected, (a, e) => c.Test.AreWithinTolerance(a, e, tolerance), message, c));
        }

        /// <summary>
        /// Asserts that two sequences do not contain the same <see cref="double"/> values (within a specified tolerance), in the same order.
        /// </summary>
        public static Actual<IEnumerable<double>> ShouldNotMatch([NotNull] this IEnumerable<double>? actual, IEnumerable<double> notExpected, double tolerance, string? message = null)
        {
            if (notExpected == null) throw new ArgumentNullException(nameof(notExpected));

            return actual.RegisterNotNullAssertion(c => AssertNotMatch(actual, notExpected, (a, e) => c.Test.AreWithinTolerance(a, e, tolerance), message, c));
        }

        /// <summary>
        /// Asserts that two sequences do not contain the same items in the same order, using a custom equality function.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldNotMatch<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, IEnumerable<TExpected> notExpected, Func<TActual, TExpected, bool> predicate, string? message = null)
        {
            if (notExpected == null) throw new ArgumentNullException(nameof(notExpected));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return actual.RegisterNotNullAssertion(c => AssertNotMatch(actual, notExpected, predicate, message, c));
        }

        static void AssertNotMatch<TActual, TExpected>([NotNull] IEnumerable<TActual>? actual, IEnumerable<TExpected> notExpected, Func<TActual, TExpected, bool> predicate, string? message, IAssertionContext c)
        {
            actual.ShouldBeA<IEnumerable<TActual>>(message);

            using var bufferedActual = actual.Buffer();
            using var bufferedExpected = notExpected.Buffer();

            if (c.Test.CollectionsMatch(bufferedActual, bufferedExpected, (a, e) => predicate((TActual)a, (TExpected)e)))
                throw c.StandardError.Matches(bufferedExpected, bufferedActual, message);
        }

        /// <summary>
        /// Asserts that a sequence starts with another sub-sequence, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldStartWith<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, IEnumerable<TExpected> expectedStart, string? message = null)
            where TExpected : TActual
        {
            if (expectedStart == null) throw new ArgumentNullException(nameof(expectedStart));

            return actual.RegisterNotNullAssertion(c => AssertStartsWith(actual, expectedStart, c.Test.ObjectsAreEqual, message, c));
        }

        /// <summary>
        /// Asserts that a sequence starts with another sub-sequence, using a custom equality function.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldStartWith<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, IEnumerable<TExpected> expectedStart, Func<TActual, TExpected, bool> predicate, string? message = null)
        {
            if (expectedStart == null) throw new ArgumentNullException(nameof(expectedStart));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return actual.RegisterNotNullAssertion(c => AssertStartsWith(actual, expectedStart, predicate, message, c));
        }

        static void AssertStartsWith<TActual, TExpected>([NotNull] IEnumerable<TActual>? actual, IEnumerable<TExpected> expectedStart, Func<TActual, TExpected, bool> predicate, string? message, IAssertionContext c)
        {
            actual.ShouldBeA<IEnumerable<TActual>>(message);

            using var bufferedActual = actual.Buffer();
            using var bufferedExpected = expectedStart.Buffer();

            if (!c.Test.CollectionStartsWith(bufferedActual, bufferedExpected, (a, e) => predicate((TActual)a, (TExpected)e)))
                throw c.StandardError.DoesNotStartWith(bufferedExpected, bufferedActual, predicate, message);
        }

        /// <summary>
        /// Asserts that a sequence ends with another sub-sequence, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldEndWith<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, IEnumerable<TExpected> expectedEnd, string? message = null)
            where TExpected : TActual
        {
            if (expectedEnd == null) throw new ArgumentNullException(nameof(expectedEnd));

            return actual.RegisterNotNullAssertion(c => AssertEndsWith(actual, expectedEnd, c.Test.ObjectsAreEqual, message, c));
        }

        /// <summary>
        /// Asserts that a sequence ends with another sub-sequence, using a custom equality function.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldEndWith<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, IEnumerable<TExpected> expectedEnd, Func<TActual, TExpected, bool> predicate, string? message = null)
        {
            if (expectedEnd == null) throw new ArgumentNullException(nameof(expectedEnd));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return actual.RegisterNotNullAssertion(c => AssertEndsWith(actual, expectedEnd, predicate, message, c));
        }

        static void AssertEndsWith<TActual, TExpected>([NotNull] IEnumerable<TActual>? actual, IEnumerable<TExpected> expectedEnd, Func<TActual, TExpected, bool> predicate, string? message, IAssertionContext c)
        {
            actual.ShouldBeA<IEnumerable<TActual>>(message);

            using var bufferedActual = actual.Buffer();
            using var bufferedExpected = expectedEnd.Buffer();

            if (!c.Test.CollectionEndsWith(bufferedActual, bufferedExpected, (a, e) => predicate((TActual)a, (TExpected)e)))
                throw c.StandardError.DoesNotEndWith(bufferedExpected, bufferedActual, predicate, message);
        }

        /// <summary>
        /// Asserts that a sequence contains a specified element, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldContain<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, TExpected expected, string? message = null)
            where TExpected : TActual
        {
            return actual.RegisterNotNullAssertion(c => AssertContains(actual, expected, c.Test.ObjectsAreEqual, message, c));
        }

        /// <summary>
        /// Asserts that a sequence contains a specified element, using a custom equality function.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldContain<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, TExpected expected, Func<TActual, TExpected, bool> predicate, string? message = null)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return actual.RegisterNotNullAssertion(c => AssertContains(actual, expected, predicate, message, c));
        }

        static void AssertContains<TActual, TExpected>([NotNull] IEnumerable<TActual>? actual, TExpected expected, Func<TActual, TExpected, bool> predicate, string? message, IAssertionContext c)
        {
            actual.ShouldBeA<IEnumerable<TActual>>(message);

            using var bufferedActual = actual.Buffer();

            if (!c.Test.ContainsAllItems(bufferedActual, new[] { expected }, predicate))
                throw c.StandardError.DoesNotContain(expected, bufferedActual, message: message);
        }

        /// <summary>
        /// Asserts that a sequence contains all specified elements, in any order, using the default equality comparer.
        /// If the expected sequence has duplicates, the actual sequence must have at least as many.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldContainItems<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, IEnumerable<TExpected> expected, string? message = null)
            where TExpected : TActual
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));

            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TActual>>(message);

                    using var bufferedActual = actual.Buffer();
                    using var bufferedExpected = expected.Buffer();

                    if (!c.Test.ContainsAllItems(bufferedActual, bufferedExpected, c.Test.ObjectsAreEqual))
                        throw c.StandardError.DoesNotContainItems(bufferedExpected, bufferedActual, c.Test.ObjectsAreEqual, message);
                });
        }

        /// <summary>
        /// Asserts that a sequence contains all specified elements, in any order, using a custom equality function.
        /// If the expected sequence has duplicates, the actual sequence must have at least as many.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldContainItems<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, IEnumerable<TExpected> expected, Func<TActual, TExpected, bool> predicate, string? message = null)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TActual>>(message);

                    using var bufferedActual = actual.Buffer();
                    using var bufferedExpected = expected.Buffer();

                    if (!c.Test.ContainsAllItems(bufferedActual, bufferedExpected, predicate))
                        throw c.StandardError.DoesNotContainItems(bufferedExpected, bufferedActual, predicate, message);
                });
        }

        /// <summary>
        /// Asserts that all elements in a sequence are contained within another sequence, in any order, using the default equality comparer.
        /// If the actual sequence has duplicates, the expected sequence must have at least as many.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ItemsShouldBeIn<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, IEnumerable<TExpected> expectedSuperset, string? message = null)
            where TExpected : TActual
        {
            if (expectedSuperset == null) throw new ArgumentNullException(nameof(expectedSuperset));

            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TActual>>(message);

                    using var bufferedActual = actual.Buffer();
                    using var bufferedExpected = expectedSuperset.Buffer();

                    if (!c.Test.ContainsAllItems(bufferedExpected, bufferedActual, (e, a) => c.Test.ObjectsAreEqual(a, e)))
                        throw c.StandardError.ContainsExtraItem(bufferedExpected, bufferedActual, c.Test.ObjectsAreEqual, message);
                });
        }

        /// <summary>
        /// Asserts that all elements in a sequence are contained within another sequence, in any order, using a custom equality function.
        /// If the actual sequence has duplicates, the expected sequence must have at least as many.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ItemsShouldBeIn<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, IEnumerable<TExpected> expectedSuperset, Func<TActual, TExpected, bool> predicate, string? message = null)
        {
            if (expectedSuperset == null) throw new ArgumentNullException(nameof(expectedSuperset));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TActual>>(message);

                    using var bufferedActual = actual.Buffer();
                    using var bufferedExpected = expectedSuperset.Buffer();

                    if (!c.Test.ContainsAllItems(bufferedExpected, bufferedActual, (e, a) => predicate(a, e)))
                        throw c.StandardError.ContainsExtraItem(bufferedExpected, bufferedActual, predicate, message);
                });
        }

        /// <summary>
        /// Asserts that a sequence does not contain a specified element, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldNotContain<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, TExpected expectedToNotContain, string? message = null)
            where TExpected : TActual
        {
            return actual.RegisterNotNullAssertion(c => AssertDoesNotContain(actual, expectedToNotContain, c.Test.ObjectsAreEqual, message, c));
        }

        /// <summary>
        /// Asserts that a sequence does not contain a specified element, using a custom equality function.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldNotContain<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, TExpected expectedToNotContain, Func<TActual, TExpected, bool> predicate, string? message = null)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return actual.RegisterNotNullAssertion(c => AssertDoesNotContain(actual, expectedToNotContain, predicate, message, c));
        }

        static void AssertDoesNotContain<TActual, TExpected>([NotNull] IEnumerable<TActual>? actual, TExpected expectedToNotContain, Func<TActual, TExpected, bool> predicate, string? message, IAssertionContext c)
        {
            actual.ShouldBeA<IEnumerable<TActual>>(message);

            using var bufferedActual = actual.Buffer();

            if (c.Test.ContainsAllItems(bufferedActual, new[] { expectedToNotContain }, predicate))
                throw c.StandardError.Contains(expectedToNotContain, bufferedActual, message: message);
        }

        /// <summary>
        /// Asserts that a sequence does not contain any of the specified elements, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldNotContainItems<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, IEnumerable<TExpected> expectedToNotContain, string? message = null)
            where TExpected : TActual
        {
            if (expectedToNotContain == null) throw new ArgumentNullException(nameof(expectedToNotContain));

            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TActual>>(message);

                    using var bufferedActual = actual.Buffer();
                    using var bufferedExpected = expectedToNotContain.Buffer();

                    if (c.Test.ContainsAny(bufferedActual, bufferedExpected))
                        throw c.StandardError.Contains(bufferedExpected, bufferedActual, message);
                });
        }

        /// <summary>
        /// Asserts that a sequence does not contain any of the specified elements, using a custom equality function.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldNotContainItems<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, IEnumerable<TExpected> expectedToNotContain, Func<TActual, TExpected, bool> predicate, string? message = null)
        {
            if (expectedToNotContain == null) throw new ArgumentNullException(nameof(expectedToNotContain));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TActual>>(message);

                    using var bufferedActual = actual.Buffer();
                    using var bufferedExpected = expectedToNotContain.Buffer();

                    if (c.Test.ContainsAny(bufferedActual, bufferedExpected, predicate))
                        throw c.StandardError.Contains(bufferedExpected, bufferedActual, message);
                });
        }

        /// <summary>
        /// Asserts thats a sequence only contains the specified elements, and nothing else, in any order, using the default equality comparer.
        /// If the expected sequence has duplicates, the actual sequence must have the same amount.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldOnlyContain<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, IEnumerable<TExpected> expected, string? message = null)
            where TExpected : TActual
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));

            return actual.RegisterNotNullAssertion(c => AssertOnlyContains(actual, expected, c.Test.ObjectsAreEqual, message, c));
        }

        /// <summary>
        /// Asserts that a sequence only contains the specified elements, and nothing else, in any order, using a custom equality function.
        /// If the expected sequence has duplicates, the actual sequence must have the same amount.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldOnlyContain<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, IEnumerable<TExpected> expected, Func<TActual, TExpected, bool> predicate, string? message = null)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return actual.RegisterNotNullAssertion(c => AssertOnlyContains(actual, expected, predicate, message, c));
        }

        static void AssertOnlyContains<TActual, TExpected>([NotNull] IEnumerable<TActual>? actual, IEnumerable<TExpected> expected, Func<TActual, TExpected, bool> predicate, string? message, IAssertionContext c)
        {
            actual.ShouldBeA<IEnumerable<TActual>>(message);

            using var bufferedActual = actual.Buffer();
            using var bufferedExpected = expected.Buffer();

            if (!c.Test.ContainsOnlyExpectedItems(bufferedActual, bufferedExpected, predicate))
                throw c.StandardError.DoesNotOnlyContain(bufferedExpected, bufferedActual, predicate, message);
        }

        /// <summary>
        /// Asserts that a sequence contains no duplicated elements, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldBeDistinct<TActual>([NotNull] this IEnumerable<TActual>? actual, string? message = null)
        {
            return actual.RegisterNotNullAssertion(c => AssertDistinct(actual, c.Test.ObjectsAreEqual, message, c));
        }

        /// <summary>
        /// Asserts that a sequence contains no duplicated elements, using a custom equality function.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldBeDistinct<TActual>([NotNull] this IEnumerable<TActual>? actual, Func<TActual, TActual, bool> predicate, string? message = null)
        {
            return actual.RegisterNotNullAssertion(c => AssertDistinct(actual, predicate, message, c));
        }

        static void AssertDistinct<TActual>([NotNull] IEnumerable<TActual>? actual, Func<TActual, TActual, bool> predicate, string? message, IAssertionContext c)
        {
            actual.ShouldBeA<IEnumerable<TActual>>(message);

            using var bufferedActual = actual.Buffer();

            if (c.Test.ContainsDuplicate(bufferedActual, predicate))
                throw c.StandardError.ContainsDuplicate(bufferedActual, message);
        }

        /// <summary>
        /// Asserts that two sequences contain the same object instances in the same order.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldMatchReferences<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actual, IEnumerable<TExpected> expected, string? message = null)
            where TExpected : TActual
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));

            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TActual>>(message);

                    using var bufferedActual = actual.Buffer();
                    using var bufferedExpected = expected.Buffer();

                    if (bufferedActual.Count != bufferedExpected.Count)
                        throw c.StandardError.LengthMismatch(bufferedExpected.Count, bufferedActual, message);

                    if (!c.Test.CollectionsMatch(bufferedActual, bufferedExpected, ReferenceEquals))
                        throw c.StandardError.ItemsNotSame(bufferedExpected, bufferedActual, message);
                });
        }

        /// <summary>
        /// Asserts that a <see cref="KeyedCollection{TKey,TItem}"/> contains an item for the specified key.
        /// </summary>
        public static Actual<KeyedCollection<TKey, TItem>> ShouldContainKey<TKey, TItem>([NotNull] this KeyedCollection<TKey, TItem>? actual, TKey expectedKey, string? message = null)
        {
            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<KeyedCollection<TKey, TItem>>(message);
                    if (!actual.Contains(expectedKey))
                        throw c.StandardError.DoesNotContain(expectedKey, actual, "key", message);
                });
        }

        /// <summary>
        /// Asserts that a <see cref="KeyedCollection{TKey,TItem}"/> does not contain an item for the specified key.
        /// </summary>
        public static Actual<KeyedCollection<TKey, TItem>> ShouldNotContainKey<TKey, TItem>([NotNull] this KeyedCollection<TKey, TItem>? actual, TKey notExpectedKey, string? message = null)
        {
            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<KeyedCollection<TKey, TItem>>(message);
                    if (actual.Contains(notExpectedKey))
                        throw c.StandardError.Contains(notExpectedKey, actual, "key", message);
                });
        }

        /// <summary>
        /// Asserts that a sequence of items satisfies a matched sequence of assertions.
        /// </summary>
        public static Actual<IEnumerable<TItem>> ItemsSatisfy<TItem>([NotNull] this IEnumerable<TItem>? actual, params Action<TItem>[] assertions)
        {
            if (assertions == null) throw new ArgumentNullException(nameof(assertions));
            if (assertions.Any(a => a == null)) throw new ArgumentException("Assertions can not be null", nameof(assertions));

            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TItem>>();

                    using var bufferedActual = actual.Buffer();

                    if (bufferedActual.Count != assertions.Length)
                        throw c.StandardError.LengthMismatch(assertions.Length, bufferedActual);

                    for (var i = 0; i < assertions.Length; i++)
                        c.Call(() => assertions[i](bufferedActual[i]), $"[{i}]");
                });
        }

        /// <summary>
        /// Asserts that all items in a sequence satisfy a specified assertion.
        /// </summary>
        public static Actual<IEnumerable<TItem>> AllItemsSatisfy<TItem>([NotNull] this IEnumerable<TItem>? actual, Action<TItem> assertion)
        {
            if (assertion == null) throw new ArgumentNullException(nameof(assertion));

            return actual.RegisterNotNullAssertion(c =>
                {
                    actual.ShouldBeA<IEnumerable<TItem>>();

                    var i = 0;
                    foreach (var item in actual)
                        c.Call(() => assertion(item), $"[{i++}]");
                });
        }
    }
}
