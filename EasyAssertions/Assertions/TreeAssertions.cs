using System.Diagnostics.CodeAnalysis;

namespace EasyAssertions;

/// <summary>
/// Experimental assertions for asserting on tree structures.
/// </summary>
public static class TreeAssertions
{
    /// <summary>
    /// Asserts that two trees have the same structure and values.
    /// Values are compared using the default equality comparer.
    /// </summary>
    public static Actual<IEnumerable<TActual>> ShouldMatch<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actualRootNodes, IEnumerable<TestNode<TExpected>> expectedRootNodes, Func<TActual, IEnumerable<TActual>> getChildren, string? message = null)
        where TExpected : TActual
    {
        if (expectedRootNodes == null) throw new ArgumentNullException(nameof(expectedRootNodes));
        if (getChildren == null) throw new ArgumentNullException(nameof(getChildren));

        return actualRootNodes.RegisterNotNullAssertion(c =>
            {
                actualRootNodes.ShouldBeA<IEnumerable<TActual>>(message);

                using var actualRootNodesBuffer = actualRootNodes.Buffer();
                using var expectedRootNodesBuffer = expectedRootNodes.Buffer();

                if (!c.Test.TreesMatch(actualRootNodesBuffer, expectedRootNodesBuffer, getChildren, c.Test.ObjectsAreEqual))
                    throw c.StandardError.TreesDoNotMatch(expectedRootNodesBuffer, actualRootNodesBuffer, getChildren, c.Test.ObjectsAreEqual, message);
            });
    }

    /// <summary>
    /// Asserts that two trees have the same structure and values.
    /// Values are compared using the default equality comparer.
    /// </summary>
    public static Actual<IEnumerable<TActual>> ShouldMatch<TActual, TExpected>([NotNull] this IEnumerable<TActual>? actualRootNodes, IEnumerable<TestNode<TExpected>> expectedRootNodes, Func<TActual, IEnumerable<TActual>> getChildren, Func<TActual, TExpected, bool> predicate, string? message = null)
        where TExpected : TActual
    {
        if (expectedRootNodes == null) throw new ArgumentNullException(nameof(expectedRootNodes));
        if (getChildren == null) throw new ArgumentNullException(nameof(getChildren));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return actualRootNodes.RegisterNotNullAssertion(c =>
            {
                actualRootNodes.ShouldBeA<IEnumerable<TActual>>(message);

                var actualRootNodesBuffer = actualRootNodes.Buffer();
                var expectedRootNodesBuffer = expectedRootNodes.Buffer();

                if (!c.Test.TreesMatch(actualRootNodesBuffer, expectedRootNodesBuffer, getChildren, predicate))
                    throw c.StandardError.TreesDoNotMatch(expectedRootNodesBuffer, actualRootNodesBuffer, getChildren, (a, e) => predicate((TActual)a!, (TExpected)e!), message);
            });
    }
}