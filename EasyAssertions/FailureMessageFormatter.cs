using System;
using System.Collections;
using System.Linq.Expressions;

namespace EasyAssertions
{
    /// <summary>
    /// Provides access to a builder for the standard set of assertion failure messages.
    /// </summary>
    public static class FailureMessageFormatter
    {
        private static IFailureMessageFormatter current;

        /// <summary>
        /// The current failure message formatter.
        /// Can be overriden with <see cref="Override"/>.
        /// </summary>
        public static IFailureMessageFormatter Current
        {
            get { return current ?? DefaultFailureMessageFormatter.Instance; }
        }

        /// <summary>
        /// Overrides the message formatter used to provide the standard set of assertion failure messages.
        /// </summary>
        /// <param name="newFormatter"></param>
        public static void Override(IFailureMessageFormatter newFormatter)
        {
            current = newFormatter;
        }

        /// <summary>
        /// Resets the current failure message formatter to the formatter.
        /// </summary>
        public static void Default()
        {
            current = null;
        }
    }

    /// <summary>
    /// The standard set of assertion failure messages.
    /// </summary>
    public interface IFailureMessageFormatter
    {
        string NotEqual(object expected, object actual, string message = null);
        string NotEqual(string expected, string actual, string message = null);
        string AreEqual(object notExpected, object actual, string message = null);
        string IsNull(string message = null);
        string NotSame(object expected, object actual, string message = null);
        string AreSame(object actual, string message = null);
        string DoNotMatch(IEnumerable expected, IEnumerable actual, string message = null);
        string DoesNotContain(object expected, IEnumerable actual, string message = null);
        string DoesNotContainItems(IEnumerable expected, IEnumerable actual, string message = null);
        string Contains(IEnumerable expectedToNotContain, IEnumerable actual, string message = null);
        string DoesNotOnlyContain(IEnumerable expected, IEnumerable actual, string message = null);
        string ItemsNotSame(IEnumerable expected, IEnumerable actual, string message = null);
        string NoException(Type expectedExceptionType, LambdaExpression function, string message = null);
        string WrongException(Type expectedExceptionType, Type actualExceptionType, LambdaExpression function, string message = null);
        string NotEmpty(IEnumerable actual, string message = null);
        string IsEmpty(string message = null);
        string LengthMismatch(int expectedLength, IEnumerable actual, string message = null);
        string DoesNotContain(string expectedSubstring, string actual, string message = null);
        string Contains(string expectedToNotContain, string actual, string message = null);
        string DoesNotStartWith(string expectedStart, string actual, string message = null);
        string DoesNotEndWith(string expectedEnd, string actual, string message = null);
    }
}