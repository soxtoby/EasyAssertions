using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EasyAssertions
{
    public static class Compare
    {
        /// <summary>
        /// Determines whether two objects are equal, using the default equality comparer.
        /// </summary>
        public static bool ObjectsAreEqual<TActual, TExpected>(TActual actual, TExpected expected) where TExpected : TActual
        {
            return EqualityComparer<TActual>.Default.Equals(actual, expected);
        }

        /// <summary>
        /// Determines whether the difference between two <see cref="float"/> values is less than or equal to a given tolerance.
        /// </summary>
        public static bool AreWithinTolerance(float actual, float expected, float tolerance)
        {
            return Math.Abs(actual - expected) <= tolerance;
        }

        /// <summary>
        /// Determines whether the difference between two <see cref="double"/> values is less than or equal to a given tolerance.
        /// </summary>
        public static bool AreWithinTolerance(double actual, double notExpected, double tolerance)
        {
            return Math.Abs(actual - notExpected) <= tolerance;
        }

        /// <summary>
        /// Determines whether two sequences contain the same items in the same order.
        /// <see cref="IEnumerable"/> items are compared recursively.
        /// Non-<c>IEnumerable</c> items are compared using the default equality comparer.
        /// </summary>
        public static bool CollectionsMatch(IEnumerable actual, IEnumerable expected, Func<object, object, bool> areEqual)
        {
            IEnumerator actualEnumerator = actual.GetEnumerator();
            IEnumerator expectedEnumerator = expected.GetEnumerator();

            try
            {
                while (actualEnumerator.MoveNext())
                {
                    if (!expectedEnumerator.MoveNext())
                        return false;
                    if (!areEqual(actualEnumerator.Current, expectedEnumerator.Current))
                        return false;
                }
                return !expectedEnumerator.MoveNext();
            }
            finally
            {
                Dispose(actualEnumerator);
                Dispose(expectedEnumerator);
            }
        }

        /// <summary>
        /// Determines whether two objects are equivalent.
        /// <see cref="IEnumerable"/> items are compared recursively.
        /// Non-<c>IEnumerable</c> items are compared using the default equality comparer.
        /// </summary>
        public static bool ObjectsMatch<TActual, TExpected>(TActual actual, TExpected expected)
        {
            return ObjectsMatch((object)actual, (object)expected);
        }

        internal static bool ObjectsMatch(object actual, object expected)
        {
            IEnumerable actualEnumerable = actual as IEnumerable;
            IEnumerable expectedEnumerable = expected as IEnumerable;
            if (actualEnumerable != null && expectedEnumerable != null)
                return CollectionsMatch(actualEnumerable, expectedEnumerable, ObjectsMatch);

            return ObjectsAreEqual(actual, expected);
        }

        /// <summary>
        /// Returns true if a sequence does not contain any elements.
        /// </summary>
        public static bool IsEmpty<TActual>(TActual actual) where TActual : IEnumerable
        {
            IEnumerator enumerator = actual.GetEnumerator();
            bool empty = !enumerator.MoveNext();
            Dispose(enumerator);
            return empty;
        }

        /// <summary>
        /// Determines whether a sequence contains all of the items in another sequence.
        /// </summary>
        public static bool ContainsAllItems(IEnumerable superset, IEnumerable subset)
        {
            HashSet<object> actualSet = new HashSet<object>(superset.Cast<object>());
            return subset.Cast<object>().All(actualSet.Contains);
        }

        /// <summary>
        /// Determines whether a sequence contains any of the items in another sequence.
        /// </summary>
        public static bool ContainsAny(IEnumerable actual, IEnumerable itemsToLookFor)
        {
            if (IsEmpty(itemsToLookFor))
                return true;

            HashSet<object> actualSet = new HashSet<object>(actual.Cast<object>());
            return itemsToLookFor.Cast<object>().Any(actualSet.Contains);
        }

        /// <summary>
        /// Determines whether a sequence contains all of the items in another sequence, and no other items.
        /// </summary>
        public static bool ContainsOnlyExpectedItems(IEnumerable actual, IEnumerable expected)
        {
            HashSet<object> actualItems = new HashSet<object>(actual.Cast<object>());
            HashSet<object> expectedItems = new HashSet<object>(expected.Cast<object>());
            return expectedItems.All(actualItems.Contains)
                && actualItems.All(expectedItems.Contains);
        }

        /// <summary>
        /// Determines whether two trees have the same structure and values.
        /// </summary>
        public static bool TreesMatch<TActual, TExpected>(IEnumerable<TActual> actualNodes, IEnumerable<TestNode<TExpected>> expectedNodes, Func<TActual, IEnumerable<TActual>> getChildren, Func<TActual, TExpected, bool> areEqual)
        {
            return CollectionsMatch(actualNodes, expectedNodes, (a, e) =>
                {
                    TActual actualNode = (TActual)a;
                    TestNode<TExpected> expectedNode = (TestNode<TExpected>)e;
                    return areEqual(actualNode, expectedNode.Value)
                        && TreesMatch(getChildren(actualNode), expectedNode, getChildren, areEqual);
                });
        }

        public static bool CollectionStartsWith(List<object> actual, List<object> expected, Func<object, object, bool> areEqual)
        {
            IEnumerator actualEnumerator = actual.GetEnumerator();
            IEnumerator expectedEnumerator = expected.GetEnumerator();

            try
            {
                while (expectedEnumerator.MoveNext())
                {
                    if (!actualEnumerator.MoveNext())
                        return false;
                    if (!areEqual(actualEnumerator.Current, expectedEnumerator.Current))
                        return false;
                }
                return true;
            }
            finally
            {
                Dispose(actualEnumerator);
                Dispose(expectedEnumerator);
            }
        }

        private static void Dispose(object obj)
        {
            (obj as IDisposable)?.Dispose();
        }
    }
}
