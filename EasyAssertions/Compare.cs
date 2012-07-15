using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EasyAssertions
{
    public static class Compare
    {
        public static bool ObjectsAreEqual<T>(T actual, T expected)
        {
            return EqualityComparer<T>.Default.Equals(actual, expected);
        }

        public static bool AreWithinDelta(float actual, float expected, float delta)
        {
            return Math.Abs(actual - expected) <= delta;
        }

        public static bool AreWithinDelta(double actual, double notExpected, double delta)
        {
            return Math.Abs(actual - notExpected) <= delta;
        }

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

        public static bool ObjectsMatch(object actual, object expected)
        {
            IEnumerable actualEnumerable = actual as IEnumerable;
            IEnumerable expectedEnumerable = expected as IEnumerable;
            if (actualEnumerable != null && expectedEnumerable != null)
                return CollectionsMatch(actualEnumerable, expectedEnumerable, ObjectsMatch);

            return ObjectsAreEqual(actual, expected);
        }

        public static bool IsEmpty<TActual>(TActual actual) where TActual : IEnumerable
        {
            IEnumerator enumerator = actual.GetEnumerator();
            bool empty = !enumerator.MoveNext();
            Dispose(enumerator);
            return empty;
        }

        public static bool ContainsAllItems(IEnumerable actual, IEnumerable expected)
        {
            HashSet<object> actualSet = new HashSet<object>(actual.Cast<object>());
            return expected.Cast<object>().All(actualSet.Contains);
        }

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
