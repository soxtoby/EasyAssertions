using System.Collections;
using NSubstitute;

namespace EasyAssertions.UnitTests;

static class EnumerableArg
{
    public static IEnumerable Matches(IEnumerable enumerable)
    {
        return Arg.Is((IEnumerable value) => StandardTests.Instance.CollectionsMatch(value, enumerable, StandardTests.Instance.ObjectsAreEqual));
    }

    public static IEnumerable<T> Matches<T>(IEnumerable enumerable)
    {
        return Arg.Is((IEnumerable<T> value) => StandardTests.Instance.CollectionsMatch(value, enumerable, StandardTests.Instance.ObjectsAreEqual));
    }
}