using System;
using System.Collections;
using System.Collections.Generic;
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
                    if (Compare.IsEmpty(actual))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.IsEmpty(message));
                });
        }

        /// <summary>
        /// Asserts that two sequences contain the same items in the same order.
        /// <see cref="IEnumerable"/> items are compared recursively.
        /// Non-<c>IEnumerable</c> items are compared using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldMatch<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() => AssertMatch(actual, expected, Compare.ObjectsMatch, message));
        }

        /// <summary>
        /// Asserts that a sequence contains the specified items in the specified order.
        /// <see cref="IEnumerable"/> items are compared recursively.
        /// Non-<c>IEnumerable</c> items are compared using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldMatch<TActual, TExpected>(this IEnumerable<TActual> actual, params TExpected[] expectedItems) where TExpected : TActual
        {
            return actual.RegisterAssert(() => AssertMatch(actual, expectedItems, Compare.ObjectsMatch));
        }

        /// <summary>
        /// Asserts that two sequences contain the same <see cref="float"/> values (within a specified tolerance), in the same order.
        /// </summary>
        public static Actual<IEnumerable<float>> ShouldMatch(this IEnumerable<float> actual, IEnumerable<float> expected, float delta, string message = null)
        {
            return actual.RegisterAssert(() => AssertMatch(actual, expected, (a, e) => Compare.AreWithinTolerance((float)a, (float)e, delta), message));
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
            List<TActual> actualList = actual.ToList();
            List<TExpected> expectedList = expected.ToList();

            if (!Compare.CollectionsMatch(actualList, expectedList, (a, e) => predicate((TActual)a, (TExpected)e)))
                throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoNotMatch(expected, actual, predicate, message));
        }

        /// <summary>
        /// Asserts that a sequence contains a specified element, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldContain<TActual, TExpected>(this IEnumerable<TActual> actual, TExpected expected, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() =>
                {
                    if (!actual.Contains(expected))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoesNotContain(expected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that a sequence contains all specified elements, in any order, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldContainItems<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() =>
                {
                    if (!Compare.ContainsAllItems(actual, expected))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoesNotContainItems(expected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that a sequence does not contain any of the specified elements, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldNotContainItems<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expectedToNotContain, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() =>
                {
                    if (Compare.ContainsAny(actual, expectedToNotContain))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.Contains(expectedToNotContain, actual, message));
                });
        }

        /// <summary>
        /// Asserts thats a sequence only contains the specified elements, and nothing else, in any order, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldOnlyContain<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() =>
                {
                    if (!Compare.ContainsOnlyExpectedItems(actual, expected))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.DoesNotOnlyContain(expected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that two sequences contain the same object instances in the same order.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldMatchReferences<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() =>
                {
                    List<TActual> actualList = actual.ToList();
                    List<TExpected> expectedList = expected.ToList();

                    if (actualList.Count != expectedList.Count)
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.LengthMismatch(expectedList.Count, actual, message));

                    if (!Compare.CollectionsMatch(actual, expected, ReferenceEquals))
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.ItemsNotSame(expected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that a sequence of items satisfies a matched sequence of assertions.
        /// </summary>
        public static Actual<IEnumerable<TItem>> ItemsSatisfy<TItem>(this IEnumerable<TItem> actual, params Action<TItem>[] assertions)
        {
            return actual.RegisterAssert(() =>
                {
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
            return actual.RegisterAssert(() =>
                {
                    int i = 0;
                    foreach (TItem item in actual)
                        EasyAssertion.RegisterIndexedAssert(i++, assertion.Method, () => assertion(item));
                });
        }
    }
}