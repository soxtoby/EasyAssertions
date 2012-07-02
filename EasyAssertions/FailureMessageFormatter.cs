using System;
using System.Collections;
using System.Linq.Expressions;

namespace EasyAssertions
{
    public static class FailureMessageFormatter
    {
        private static IFailureMessageFormatter current;
        public static IFailureMessageFormatter Current
        {
            get { return current ?? DefaultFailureMessageFormatter.Instance; }
        }

        public static void Override(IFailureMessageFormatter newFormatter)
        {
            current = newFormatter;
        }

        public static void Default()
        {
            current = null;
        }
    }

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
        string ItemsNotSame(IEnumerable expected, IEnumerable actual, string message = null);
        string NoException(Type expectedExceptionType, Expression<Action> function, string message = null);
        string WrongException(Type expectedExceptionType, Type actualExceptionType, Expression<Action> function, string message = null);
        string NotEmpty(IEnumerable actual, string message = null);
        string IsEmpty(string message = null);
        string LengthMismatch(int expectedLength, IEnumerable actual, string message = null);
        string DoesNotContain(string expectedSubstring, string actual, string message = null);
        string DoesNotStartWith(string expectedStart, string actual, string message = null);
        string DoesNotEndWith(string expectedEnd, string actual, string message = null);
    }
}