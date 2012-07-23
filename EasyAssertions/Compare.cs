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
        public static bool ObjectsAreEqual<T>(T actual, T expected)
        {
            return EqualityComparer<T>.Default.Equals(actual, expected);
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

            while (actualEnumerator.MoveNext())
            {
                if (!expectedEnumerator.MoveNext())
                    return false;
                if (!areEqual(actualEnumerator.Current, expectedEnumerator.Current))
                    return false;
            }
            bool equal = !expectedEnumerator.MoveNext();

            Dispose(actualEnumerator);
            Dispose(expectedEnumerator);

            return equal;
        }

        /// <summary>
        /// Determines whether two objects are equivalent.
        /// <see cref="IEnumerable"/> items are compared recursively.
        /// Non-<c>IEnumerable</c> items are compared using the default equality comparer.
        /// </summary>
        public static bool ObjectsMatch(object actual, object expected)
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
        public static bool ContainsAllItems(IEnumerable actual, IEnumerable expected)
        {
            HashSet<object> actualSet = new HashSet<object>(actual.Cast<object>());
            return expected.Cast<object>().All(actualSet.Contains);
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

        private static void Dispose(object obj)
        {
            IDisposable disposable = obj as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}
