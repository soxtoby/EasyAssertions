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
            IEnumerator actualEnumerator = actual.GetEnumerator();
            IEnumerator expectedEnumerator = expected.GetEnumerator();

            while (actualEnumerator.MoveNext())
            {
                if (!expectedEnumerator.MoveNext())
                    return false;
                if (!ObjectsMatch(actualEnumerator.Current, expectedEnumerator.Current))
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

        private static void Dispose(object obj)
        {
            IDisposable disposable = obj as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}