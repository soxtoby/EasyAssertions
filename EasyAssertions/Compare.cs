using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyAssertions
{
    public static class Compare
    {
        public static bool ObjectsAreEqual<T>(T actual, T expected)
        {
            return EqualityComparer<T>.Default.Equals(actual, expected);
        }

        public static bool CollectionsMatch(IEnumerable actual, IEnumerable expected)
        {
            return CollectionsMatch(actual, expected, ObjectsMatch);
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

        private static bool ObjectsMatch(object actual, object expected)
        {
            IEnumerable actualEnumerable = actual as IEnumerable;
            IEnumerable expectedEnumerable = expected as IEnumerable;
            if (actualEnumerable != null && expectedEnumerable != null)
                return CollectionsMatch(actualEnumerable, expectedEnumerable);

            return ObjectsAreEqual(actual, expected);
        }

        public static bool IsEmpty<TActual>(TActual actual) where TActual : IEnumerable
        {
            IEnumerator enumerator = actual.GetEnumerator();
            bool empty = !enumerator.MoveNext();
            Dispose(enumerator);
            return empty;
        }

        private static void Dispose(object obj)
        {
            IDisposable disposable = obj as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}