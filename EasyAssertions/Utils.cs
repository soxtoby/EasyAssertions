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
    }
}