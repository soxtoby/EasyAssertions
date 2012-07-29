using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyAssertions
{
    static class Utils
    {
        public static string Join(this IEnumerable<string> strings, string delimiter)
        {
            StringBuilder sb = new StringBuilder();
            bool delimit = false;
            foreach (string str in strings)
            {
                if (delimit)
                    sb.Append(delimiter);
                else
                    delimit = true;

                sb.Append(str);
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
    }
}