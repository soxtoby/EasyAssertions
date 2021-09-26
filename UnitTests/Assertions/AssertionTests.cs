using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace EasyAssertions.UnitTests
{
    public abstract class AssertionTests
    {
        protected IStandardErrors Error = null!;
        protected Exception ExpectedException = null!;

        [SetUp]
        public void BaseSetUp()
        {
            Error = Substitute.ForPartsOf<TestErrors>();
            ExpectedException = new Exception();
            StandardErrors.Override(Error);
        }

        [TearDown]
        public void BaseTearDown()
        {
            StandardErrors.Default();
        }

        protected static void AssertReturnsActual<T>(T actual, Func<Actual<T>> assert)
        {
            var result = assert();

            Assert.AreSame(actual, result.And);
        }

        protected void AssertThrowsExpectedError(Action assert)
        {
            var result = Assert.Throws<Exception>(() => assert());

            Assert.AreSame(ExpectedException, result);
        }

        protected static void AssertArgumentNullException(string paramName, TestDelegate assertionCall)
        {
            var result = Assert.Throws<ArgumentNullException>(assertionCall);

            Assert.AreEqual(paramName, result.ParamName);
        }

        protected void AssertFailsWithTypesNotEqualMessage(Type expectedType, Type? actualType, Action<string> assertionCall)
        {
            Error.NotEqual(expectedType, actualType, "foo").Returns(ExpectedException);

            var result = Assert.Throws<Exception>(() => assertionCall("foo"));

            Assert.AreSame(ExpectedException, result);
        }

        public class TestErrors : IStandardErrors
        {
            readonly Exception unexpectedError = new InvalidOperationException("Unexpected standard error created. Make sure the message was passed through.");
            public virtual Exception NotEqual(object? expected, object? actual, string? message = null) => unexpectedError;
            public virtual Exception NotEqual(string expected, string actual, Case caseSensitivity = Case.Sensitive, string? message = null) => unexpectedError;
            public virtual Exception AreEqual(object? notExpected, object? actual, string? message = null) => unexpectedError;
            public virtual Exception IsNull(string? message = null) => unexpectedError;
            public virtual Exception NotSame(object? expected, object? actual, string? message = null) => unexpectedError;
            public virtual Exception AreSame(object? actual, string? message = null) => unexpectedError;
            public virtual Exception DoNotMatch<TActual, TExpected>(IEnumerable<TExpected> expected, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string? message = null) => unexpectedError;
            public virtual Exception DoesNotContain(object? expected, IEnumerable actual, string? itemType = null, string? message = null) => unexpectedError;
            public virtual Exception DoesNotContainItems<TActual, TExpected>(IEnumerable<TExpected> expected, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string? message = null) => unexpectedError;
            public virtual Exception Contains(object? expectedToNotContain, IEnumerable actual, string? itemType = null, string? message = null) => unexpectedError;
            public virtual Exception Contains(IEnumerable expectedToNotContain, IEnumerable actual, string? message = null) => unexpectedError;
            public virtual Exception OnlyContains(IEnumerable expected, IEnumerable actual, string? message = null) => unexpectedError;
            public virtual Exception DoesNotOnlyContain<TActual, TExpected>(IEnumerable<TExpected> expected, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string? message = null) => unexpectedError;
            public virtual Exception ContainsExtraItem<TActual, TExpected>(IEnumerable<TExpected> expectedSuperset, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string? message = null) => unexpectedError;
            public virtual Exception ContainsDuplicate(IEnumerable actual, string? message = null) => unexpectedError;
            public virtual Exception ItemsNotSame(IEnumerable expected, IEnumerable actual, string? message = null) => unexpectedError;
            public virtual Exception DoesNotStartWith<TActual, TExpected>(IEnumerable<TExpected> expected, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string? message = null) => unexpectedError;
            public virtual Exception DoesNotEndWith<TActual, TExpected>(IEnumerable<TExpected> expected, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string? message = null) => unexpectedError;
            public virtual Exception TreesDoNotMatch<TActual, TExpected>(IEnumerable<TestNode<TExpected>> expected, IEnumerable<TActual> actual, Func<TActual, IEnumerable<TActual>> getChildren, Func<object?, object?, bool> predicate, string? message = null) => unexpectedError;
            public virtual Exception NoException(Type expectedExceptionType, LambdaExpression? function = null, string? message = null) => unexpectedError;
            public virtual Exception WrongException(Type expectedExceptionType, Exception actualException, LambdaExpression? function = null, string? message = null) => unexpectedError;
            public virtual Exception NotEmpty(IEnumerable actual, string? message = null) => unexpectedError;
            public virtual Exception NotEmpty(string actual, string? message = null) => unexpectedError;
            public virtual Exception IsEmpty(string? message = null) => unexpectedError;
            public virtual Exception LengthMismatch(int expectedLength, IEnumerable actual, string? message = null) => unexpectedError;
            public virtual Exception DoesNotContain(string expectedSubstring, string actual, string? message = null) => unexpectedError;
            public virtual Exception Contains(string expectedToNotContain, string actual, string? message = null) => unexpectedError;
            public virtual Exception DoesNotStartWith(string expectedStart, string actual, string? message = null) => unexpectedError;
            public virtual Exception DoesNotEndWith(string expectedEnd, string actual, string? message = null) => unexpectedError;
            public virtual Exception NotGreaterThan(object? expected, object? actual, string? message = null) => unexpectedError;
            public virtual Exception NotLessThan(object? expected, object? actual, string? message = null) => unexpectedError;
            public virtual Exception DoesNotMatch(Regex regex, string actual, string? message = null) => unexpectedError;
            public virtual Exception Matches(Regex regex, string actual, string? message = null) => unexpectedError;
            public virtual Exception TaskTimedOut(TimeSpan timeout, string? message = null) => unexpectedError;
            public virtual Exception Matches(IEnumerable notExpected, IEnumerable actual, string? message = null) => unexpectedError;
        }
    }
}