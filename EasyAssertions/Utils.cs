using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyAssertions
{
    static class Utils
    {
        public static int IndexOf<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            var i = -1;
            foreach (T item in enumerable)
            {
                i++;
                if (predicate(item))
                    break;
            }
            return i;
        }

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
    }
}