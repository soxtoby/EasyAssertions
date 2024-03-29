﻿using System.Collections;

namespace EasyAssertions;

/// <summary>
/// Provides a set of standard testing functions for implementing assertion logic.
/// </summary>
public class StandardTests
{
    internal static readonly StandardTests Instance = new();

    /// <summary>
    /// Determines whether two objects are equal, using the default equality comparer.
    /// </summary>
    public bool ObjectsAreEqual<TActual, TExpected>(TActual actual, TExpected expected) where TExpected : TActual
    {
        return EqualityComparer<TActual>.Default.Equals(actual, expected);
    }

    /// <summary>
    /// Determines whether the difference between two <see cref="float"/> values is less than or equal to a given tolerance.
    /// </summary>
    public bool AreWithinTolerance(float actual, float expected, double tolerance)
    {
        return Math.Abs(actual - expected) <= tolerance;
    }

    /// <summary>
    /// Determines whether the difference between two <see cref="double"/> values is less than or equal to a given tolerance.
    /// </summary>
    public bool AreWithinTolerance(double actual, double expected, double tolerance)
    {
        return Math.Abs(actual - expected) <= tolerance;
    }

    /// <summary>
    /// Determines whether one sequence starts with the items in another sequence, in the same order.
    /// </summary>
    public bool CollectionStartsWith(IEnumerable actual, IEnumerable expectedToStartWith, Func<object, object, bool> areEqual)
    {
        using var bufferedActual = actual.Buffer();
        using var bufferedExpected = expectedToStartWith.Buffer();

        return bufferedActual.Count >= bufferedExpected.Count
            && CollectionsMatch(bufferedActual.Take(bufferedExpected.Count), bufferedExpected, areEqual);
    }

    /// <summary>
    /// Determines whether one sequence ends with the items in another sequence, in the same order.
    /// </summary>
    public bool CollectionEndsWith(IEnumerable actual, IEnumerable expectedToEndWith, Func<object, object, bool> areEqual)
    {
        using var bufferedActual = actual.Buffer();
        using var bufferedExpected = expectedToEndWith.Buffer();

        return bufferedActual.Count >= bufferedExpected.Count
            && CollectionsMatch(bufferedActual.Skip(bufferedActual.Count - bufferedExpected.Count), bufferedExpected, areEqual);
    }

    /// <summary>
    /// Determines whether two sequences contain the same items in the same order.
    /// <see cref="IEnumerable"/> items are compared recursively.
    /// Non-<c>IEnumerable</c> items are compared using the default equality comparer.
    /// </summary>
    public bool CollectionsMatch(IEnumerable actual, IEnumerable expected, Func<object, object, bool> areEqual)
    {
        var actualEnumerator = actual.GetEnumerator();
        var expectedEnumerator = expected.GetEnumerator();

        try
        {
            while (actualEnumerator.MoveNext())
            {
                if (!expectedEnumerator.MoveNext())
                    return false;
                if (!areEqual(actualEnumerator.Current, expectedEnumerator.Current))
                    return false;
            }
            return !expectedEnumerator.MoveNext();
        }
        finally
        {
            Dispose(actualEnumerator);
            Dispose(expectedEnumerator);
        }
    }

    /// <summary>
    /// Determines whether two objects are equivalent.
    /// <see cref="IEnumerable"/> items are compared recursively.
    /// Non-<c>IEnumerable</c> items are compared using the default equality comparer.
    /// </summary>
    public bool ObjectsMatch<TActual, TExpected>(TActual actual, TExpected expected)
    {
        return ObjectsMatch((object?)actual, (object?)expected);
    }

    internal bool ObjectsMatch(object? actual, object? expected)
    {
        if (actual is IEnumerable actualEnumerable && expected is IEnumerable expectedEnumerable)
            return CollectionsMatch(actualEnumerable, expectedEnumerable, ObjectsMatch);

        return ObjectsAreEqual(actual, expected);
    }

    /// <summary>
    /// Returns true if a sequence does not contain any elements.
    /// </summary>
    public bool IsEmpty<TActual>(TActual actual) where TActual : IEnumerable
    {
        var enumerator = actual.GetEnumerator();
        bool empty;
        try
        {
            empty = !enumerator.MoveNext();
        }
        finally
        {
            Dispose(enumerator);
        }
        return empty;
    }

    /// <summary>
    /// Determines whether a sequence contains all of the items in another sequence, using a custom equality function.
    /// </summary>
    public bool ContainsAllItems<TSuper, TSub>(IEnumerable<TSuper> superset, IEnumerable<TSub> subset, Func<TSuper, TSub, bool> predicate)
    {
        var remainingInSuperset = superset.ToList();

        foreach (var expectedItem in subset)
        {
            var matchingIndex = remainingInSuperset.FindIndex(a => predicate(a, expectedItem));
            if (matchingIndex == -1)
                return false;
            remainingInSuperset.RemoveAt(matchingIndex);
        }

        return true;
    }

    /// <summary>
    /// Determines whether a sequence contains any of the items in another sequence, using the default equality comparer.
    /// </summary>
    public bool ContainsAny(IEnumerable actual, IEnumerable itemsToLookFor)
    {
        using var bufferedItemsToLookFor = itemsToLookFor.Buffer();

        if (IsEmpty(bufferedItemsToLookFor))
            return true;

        var actualSet = new HashSet<object>(actual.Cast<object>());
        return bufferedItemsToLookFor.Any(actualSet.Contains);
    }

    /// <summary>
    /// Determines whether a sequence contains any of the items in another sequence, using a custom equality function.
    /// </summary>
    public bool ContainsAny<TActual, TExpected>(IEnumerable<TActual> actual, IEnumerable<TExpected> itemsToLookFor, Func<TActual, TExpected, bool> predicate)
    {
        using var bufferedActual = actual.Buffer();
        using var bufferedItemsToLookFor = itemsToLookFor.Buffer();

        if (IsEmpty(bufferedItemsToLookFor))
            return true;

        return bufferedItemsToLookFor.Any(e => bufferedActual.Any(a => predicate(a, e)));
    }

    /// <summary>
    /// Determines whether a sequence contains all of the items in another sequence, and no other items, using a custom equality function.
    /// If the expected sequence has duplicates, the actual sequence must have the same amount.
    /// </summary>
    public bool ContainsOnlyExpectedItems<TActual, TExpected>(IEnumerable<TActual> actual, IEnumerable<TExpected> expected, Func<TActual, TExpected, bool> predicate)
    {
        var remainingActual = actual.ToList();

        foreach (var expectedItem in expected)
        {
            var matchingIndex = remainingActual.FindIndex(a => predicate(a, expectedItem));
            if (matchingIndex == -1)
                return false;
            remainingActual.RemoveAt(matchingIndex);
        }

        return remainingActual.Count == 0;
    }

    /// <summary>
    /// Determines whether a sequence contains any items that are equivalent, using a custom equality function.
    /// </summary>
    public bool ContainsDuplicate<T>(IEnumerable<T> items, Func<T, T, bool> predicate)
    {
        var itemList = items.ToList();
        while (itemList.Any())
        {
            var firstItem = itemList[0];
            var equivalentItems = itemList.RemoveAll(i => predicate(firstItem, i));
            if (equivalentItems == 0)
                itemList.RemoveAt(0);
            else if (equivalentItems > 1)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Determines whether two trees have the same structure and values.
    /// </summary>
    public bool TreesMatch<TActual, TExpected>(IEnumerable<TActual> actualNodes, IEnumerable<TestNode<TExpected>> expectedNodes, Func<TActual, IEnumerable<TActual>> getChildren, Func<TActual, TExpected, bool> areEqual)
    {
        return CollectionsMatch(actualNodes, expectedNodes, (a, e) =>
            {
                var actualNode = (TActual)a;
                var expectedNode = (TestNode<TExpected>)e;
                return areEqual(actualNode, expectedNode.Value)
                    && TreesMatch(getChildren(actualNode), expectedNode, getChildren, areEqual);
            });
    }

    static void Dispose(object obj)
    {
        (obj as IDisposable)?.Dispose();
    }
}