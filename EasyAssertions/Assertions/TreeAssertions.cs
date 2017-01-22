using System;
using System.Collections.Generic;

namespace EasyAssertions
{
    public static class TreeAssertions
    {
        /// <summary>
        /// Asserts that two trees have the same structure and values.
        /// Values are compared using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldMatch<TActual, TExpected>(this IEnumerable<TActual> actualRootNodes, IEnumerable<TestNode<TExpected>> expectedRootNodes, Func<TActual, IEnumerable<TActual>> getChildren, string message = null) where TExpected : TActual
        {
            if (expectedRootNodes == null) throw new ArgumentNullException(nameof(expectedRootNodes));
            if (getChildren == null) throw new ArgumentNullException(nameof(getChildren));

            return actualRootNodes.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<IEnumerable<TActual>>(actualRootNodes, message);

                    if (!Compare.TreesMatch(actualRootNodes, expectedRootNodes, getChildren, Compare.ObjectsAreEqual))
                        throw EasyAssertion.Failure(FailureMessage.Standard.TreesDoNotMatch(expectedRootNodes, actualRootNodes, getChildren, Compare.ObjectsAreEqual, message));
                });
        }

        /// <summary>
        /// Asserts that two trees have the same structure and values.
        /// Values are compared using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldMatch<TActual, TExpected>(this IEnumerable<TActual> actualRootNodes, IEnumerable<TestNode<TExpected>> expectedRootNodes, Func<TActual, IEnumerable<TActual>> getChildren, Func<TActual, TExpected, bool> predicate, string message = null) where TExpected : TActual
        {
            if (expectedRootNodes == null) throw new ArgumentNullException(nameof(expectedRootNodes));
            if (getChildren == null) throw new ArgumentNullException(nameof(getChildren));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return actualRootNodes.RegisterAssert(() =>
                {
                    ObjectAssertions.AssertType<IEnumerable<TActual>>(actualRootNodes, message);

                    if (!Compare.TreesMatch(actualRootNodes, expectedRootNodes, getChildren, predicate))
                        throw EasyAssertion.Failure(FailureMessage.Standard.TreesDoNotMatch(expectedRootNodes, actualRootNodes, getChildren, (a, e) => predicate((TActual)a, (TExpected)e), message));
                });
        }
    }
}