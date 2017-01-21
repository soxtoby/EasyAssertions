using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EasyAssertions
{
    static class Utils
    {
        public static string Join<T>(this IEnumerable<T> items, string delimiter)
        {
            StringBuilder sb = new StringBuilder();
            bool delimit = false;
            foreach (T item in items)
            {
                if (delimit)
                    sb.Append(delimiter);
                else
                    delimit = true;

                sb.Append(item);
            }
            return sb.ToString();
        }

        public static bool None<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.Any();
        }

        public static string NullIfEmpty(this string value)
        {
            return value == string.Empty
                ? null
                : value;
        }

        public static int IndexOfOrDefault<T>(this IEnumerable<T> sequence, Func<T, bool> predicate, int startIndex, int defaultValue)
        {
            int i = startIndex;
            foreach (T item in sequence.Skip(startIndex))
            {
                if (predicate(item))
                    return i;
                i++;
            }
            return defaultValue;
        }

        public static bool TryReadAllLines(SourceAddress assertionsAddress, out string[] sourceLines)
        {
            sourceLines = null;

            string fileName = assertionsAddress.FileName;
            if (fileName == null)
                return false;

            try
            {
                sourceLines = File.ReadAllLines(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static IEnumerable<TOut> Zip<TLeft, TRight, TOut>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TRight, TOut> select)
        {
            using (IEnumerator<TLeft> leftEnumerator = left.GetEnumerator())
            using (IEnumerator<TRight> rightEnumerator = right.GetEnumerator())
            {
                while (leftEnumerator.MoveNext() && rightEnumerator.MoveNext())
                    yield return select(leftEnumerator.Current, rightEnumerator.Current);
            }
        }

        public static IBuffer<object> Buffer(this IEnumerable source)
        {
            return source.Cast<object>().Buffer();
        }

        public static IBuffer<T> Buffer<T>(this IEnumerable<T> source)
        {
            return new Buffer<T>(source);
        }
    }
}
