using System.Collections;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace EasyAssertions;

/// <summary>
/// The standard set of assertion failure messages.
/// </summary>
public interface IStandardErrors
{
    /// <summary>
    /// Objects should be equal, but weren't.
    /// </summary>
    Exception NotEqual(object? expected, object? actual, string? message = null);

    /// <summary>
    /// Strings should be equal, but weren't.
    /// </summary>
    Exception NotEqual(string expected, string actual, Case caseSensitivity = Case.Sensitive, string? message = null);

    /// <summary>
    /// Objects should not be equal, but were.
    /// </summary>
    Exception AreEqual(object? notExpected, object? actual, string? message = null);

    /// <summary>
    /// Object should not be null, but was.
    /// </summary>
    Exception IsNull(string? message = null);

    /// <summary>
    /// Objects should be the same instance, but weren't.
    /// </summary>
    Exception NotSame(object? expected, object? actual, string? message = null);

    /// <summary>
    /// Objects should not be the same instance, but were.
    /// </summary>
    Exception AreSame(object? actual, string? message = null);

    /// <summary>
    /// Sequences should match, but didn't.
    /// </summary>
    Exception DoNotMatch<TActual, TExpected>(IEnumerable<TExpected> expected, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string? message = null);

    /// <summary>
    /// Sequence should contain a particular item, but didn't.
    /// </summary>
    Exception DoesNotContain(object? expected, IEnumerable actual, string? itemType = null, string? message = null);

    /// <summary>
    /// Sequence should contain all the items in another sequence, but didn't.
    /// </summary>
    Exception DoesNotContainItems<TActual, TExpected>(IEnumerable<TExpected> expected, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string? message = null);

    /// <summary>
    /// Sequence should not contain a particular item, but did.
    /// </summary>
    Exception Contains(object? expectedToNotContain, IEnumerable actual, string? itemType = null, string? message = null);

    /// <summary>
    /// Sequence should not contain any of the items in another sequence, but did.
    /// </summary>
    Exception Contains(IEnumerable expectedToNotContain, IEnumerable actual, string? message = null);

    /// <summary>
    /// Sequence should have contained an item another sequence doesn't have, but didn't.
    /// </summary>
    Exception OnlyContains(IEnumerable expected, IEnumerable actual, string? message = null);

    /// <summary>
    /// Sequence should only contain the items in another sequence,
    /// but was missing at least one item, or had extra items.
    /// </summary>
    Exception DoesNotOnlyContain<TActual, TExpected>(IEnumerable<TExpected> expected, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string? message = null);

    /// <summary>
    /// Sequence should be a subset of another sequence, but wasn't.
    /// </summary>
    Exception ContainsExtraItem<TActual, TExpected>(IEnumerable<TExpected> expectedSuperset, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string? message = null);

    /// <summary>
    /// Sequence should have had a distinct set of items, but it had duplicates.
    /// </summary>
    Exception ContainsDuplicate(IEnumerable actual, string? message = null);

    /// <summary>
    /// Sequence should contain the same object references, in the same order, as another sequence, but didn't.
    /// </summary>
    Exception ItemsNotSame(IEnumerable expected, IEnumerable actual, string? message = null);

    /// <summary>
    /// Sequence should start with a particular sub-sequence, but didn't.
    /// </summary>
    Exception DoesNotStartWith<TActual, TExpected>(IEnumerable<TExpected> expected, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string? message = null);

    /// <summary>
    /// Sequence should end with a particular sub-sequence, but didn't.
    /// </summary>
    Exception DoesNotEndWith<TActual, TExpected>(IEnumerable<TExpected> expected, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string? message = null);

    /// <summary>
    /// Trees should match, but didn't.
    /// </summary>
    Exception TreesDoNotMatch<TActual, TExpected>(IEnumerable<TestNode<TExpected>> expected, IEnumerable<TActual> actual, Func<TActual, IEnumerable<TActual>> getChildren, Func<object?, object?, bool> predicate, string? message = null);

    /// <summary>
    /// Function should throw an exception, but didn't.
    /// </summary>
    Exception NoException(Type expectedExceptionType, LambdaExpression? function = null, string? message = null);

    /// <summary>
    /// Function should throw a particular exception type, but threw a different one.
    /// </summary>
    Exception WrongException(Type expectedExceptionType, Exception actualException, LambdaExpression? function = null, string? message = null);

    /// <summary>
    /// Sequence should be empty, but wasn't.
    /// </summary>
    Exception NotEmpty(IEnumerable actual, string? message = null);

    /// <summary>
    /// String should be empty, but wasn't.
    /// </summary>
    Exception NotEmpty(string actual, string? message = null);

    /// <summary>
    /// Sequence should not be empty, but was.
    /// </summary>
    Exception IsEmpty(string? message = null);

    /// <summary>
    /// Sequence should be a specific length, but wasn't.
    /// </summary>
    Exception LengthMismatch(int expectedLength, IEnumerable actual, string? message = null);

    /// <summary>
    /// String should contain a particular substring, but didn't.
    /// </summary>
    Exception DoesNotContain(string expectedSubstring, string actual, string? message = null);

    /// <summary>
    /// String should not contain a particular substring, but did.
    /// </summary>
    Exception Contains(string expectedToNotContain, string actual, string? message = null);

    /// <summary>
    /// String should start with a particular substring, but didn't.
    /// </summary>
    Exception DoesNotStartWith(string expectedStart, string actual, string? message = null);

    /// <summary>
    /// String should end with a particular substring, but didn't.
    /// </summary>
    Exception DoesNotEndWith(string expectedEnd, string actual, string? message = null);

    /// <summary>
    /// Value should be greater than another value, but wasn't.
    /// </summary>
    Exception NotGreaterThan(object? expected, object? actual, string? message = null);

    /// <summary>
    /// Value should be less than another value, but wasn't.
    /// </summary>
    Exception NotLessThan(object? expected, object? actual, string? message = null);

    /// <summary>
    /// String should have matched regex pattern, but didn't.
    /// </summary>
    Exception DoesNotMatch(Regex regex, string actual, string? message = null);

    /// <summary>
    /// String should not have matched regex pattern, but did.
    /// </summary>
    Exception Matches(Regex regex, string actual, string? message = null);

    /// <summary>
    /// Task timed out while waiting for it to complete.
    /// </summary>
    Exception TaskTimedOut(TimeSpan timeout, string? message = null);

    /// <summary>
    /// Sequences should not match, but did.
    /// </summary>
    Exception Matches(IEnumerable notExpected, IEnumerable actual, string? message = null);
}