using System.Collections;
using System.Collections.Generic;
using NSubstitute;

namespace EasyAssertions.UnitTests
{
    static class EnumerableArg
    {
        public static IEnumerable Matches(IEnumerable enumerable)
        {
            return Arg.Is((IEnumerable value) => Compare.CollectionsMatch(value, enumerable, Compare.ObjectsAreEqual));
        }

        public static IEnumerable<T> Matches<T>(IEnumerable enumerable)
        {
            return Arg.Is((IEnumerable<T> value) => Compare.CollectionsMatch(value, enumerable, Compare.ObjectsAreEqual));
        }
    }
}
