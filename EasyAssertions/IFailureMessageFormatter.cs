using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EasyAssertions
{
    /// <summary>
    /// The standard set of assertion failure messages.
    /// </summary>
    public interface IFailureMessageFormatter
    {
        /// <summary>
        /// Objects should be equal, but weren't.
        /// </summary>
        string NotEqual(object expected, object actual, string message = null);

        /// <summary>
        /// Strings should be equal, but weren't.
        /// </summary>
        string NotEqual(string expected, string actual, string message = null);

        /// <summary>
        /// Objects should not be equal, but were.
        /// </summary>
        string AreEqual(object notExpected, object actual, string message = null);

        /// <summary>
        /// Object should not be null, but was.
        /// </summary>
        string IsNull(string message = null);

        /// <summary>
        /// Objects should be the same instance, but weren't.
        /// </summary>
        string NotSame(object expected, object actual, string message = null);

        /// <summary>
        /// Objects should not be the same instance, but were.
        /// </summary>
        string AreSame(object actual, string message = null);

        /// <summary>
        /// Sequences should match, but didn't.
        /// </summary>
        string DoNotMatch<TActual, TExpected>(IEnumerable<TExpected> expected, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string message = null);

        /// <summary>
        /// Sequence should contain a particular item, but didn't.
        /// </summary>
        string DoesNotContain(object expected, IEnumerable actual, string message = null);

        /// <summary>
        /// Sequence should contain all the items in another sequence, but didn't.
        /// </summary>
        string DoesNotContainItems(IEnumerable expected, IEnumerable actual, string message = null);

        /// <summary>
        /// Sequence should not contain any of the items in another sequence, but did.
        /// </summary>
        string Contains(IEnumerable expectedToNotContain, IEnumerable actual, string message = null);

        /// <summary>
        /// Sequence should only contain the items in another sequence,
        /// but was missing at least one item, or had extra items.
        /// </summary>
        string DoesNotOnlyContain(IEnumerable expected, IEnumerable actual, string message = null);

        /// <summary>
        /// Sequence should contain the same object references, in the same order, as another sequence, but didn't.
        /// </summary>
        string ItemsNotSame(IEnumerable expected, IEnumerable actual, string message = null);

        /// <summary>
        /// Function should throw an exception, but didn't.
        /// </summary>
        string NoException(Type expectedExceptionType, LambdaExpression function, string message = null);

        /// <summary>
        /// Function should throw a particular exception type, but threw a different one.
        /// </summary>
        string WrongException(Type expectedExceptionType, Type actualExceptionType, LambdaExpression function, string message = null);

        /// <summary>
        /// Sequence should be empty, but wasn't.
        /// </summary>
        string NotEmpty(IEnumerable actual, string message = null);

        /// <summary>
        /// Sequence should not be empty, but was.
        /// </summary>
        string IsEmpty(string message = null);

        /// <summary>
        /// Sequence should be a specific length, but wasn't.
        /// </summary>
        string LengthMismatch(int expectedLength, IEnumerable actual, string message = null);

        /// <summary>
        /// String should contain a particular substring, but didn't.
        /// </summary>
        string DoesNotContain(string expectedSubstring, string actual, string message = null);

        /// <summary>
        /// String should not contain a particular substring, but did.
        /// </summary>
        string Contains(string expectedToNotContain, string actual, string message = null);

        /// <summary>
        /// String should start with a particular substring, but didn't.
        /// </summary>
        string DoesNotStartWith(string expectedStart, string actual, string message = null);

        /// <summary>
        /// String should end with a particular substring, but didn't.
        /// </summary>
        string DoesNotEndWith(string expectedEnd, string actual, string message = null);

        /// <summary>
        /// Value should be greater than another value, but wasn't.
        /// </summary>
        string NotGreaterThan(object expected, object actual, string message = null);

        /// <summary>
        /// Value should be less than another value, but wasn't.
        /// </summary>
        string NotLessThan(object expected, object actual, string message = null);
    }
}