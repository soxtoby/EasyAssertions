using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class FunctionAssertionTests : AssertionTests
    {
        [Test]
        public void ActionShouldThrowType_ThrowsCorrectType_ReturnsException()
        {
            var thrower = new ExceptionThrower(ExpectedException);

            var result = Should.Throw<Exception>(() => thrower.Throw());

            Assert.AreSame(ExpectedException, result.And);
        }

        [Test]
        public void ActionShouldThrowType_DoesNotThrow_FailsWithNoExceptionMessage()
        {
            Expression<Action> noThrow = () => "".Trim();
            Error.NoException(typeof(Exception), noThrow, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() =>
                Should.Throw<Exception>(noThrow, "foo"));
        }

        [Test]
        public void ActionShouldThrowType_ThrowsWrongType_FailsWithWrongExceptionMessage()
        {
            var innerException = new Exception();
            var thrower = new ExceptionThrower(innerException);
            Expression<Action> throwsException = () => thrower.Throw();
            Error.WrongException(typeof(InvalidOperationException), innerException, throwsException, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() =>
                Should.Throw<InvalidOperationException>(throwsException, "foo"));
        }

        [Test]
        public void FuncShouldThrowType_ThrowsCorrectType_ReturnsException()
        {
            var thrower = new ExceptionThrower(ExpectedException);

            var result = Should.Throw<Exception>(() => thrower.ThrowingProperty);

            Assert.AreSame(ExpectedException, result.And);
        }

        [Test]
        public void FuncShouldThrowType_DoesNotThrow_FailsWithNoExceptionMessage()
        {
            Expression<Func<object>> noThrow = () => "".Length;
            Error.NoException(typeof(Exception), noThrow, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() =>
                Should.Throw<Exception>(noThrow, "foo"));
        }

        [Test]
        public void ActionShouldThrow_Throws_ReturnsException()
        {
            var thrower = new ExceptionThrower(ExpectedException);

            var result = Should.Throw(() => thrower.Throw());

            Assert.AreSame(ExpectedException, result.And);
        }

        [Test]
        public void ActionShouldThrow_DoesNotThrow_FailsWithNoExceptionMessage()
        {
            Expression<Action> noThrow = () => "".Trim();
            Error.NoException(typeof(Exception), noThrow, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() =>
                Should.Throw(noThrow, "foo"));
        }

        [Test]
        public void FuncShouldThrow_Throws_ReturnsException()
        {
            var thrower = new ExceptionThrower(ExpectedException);

            var result = Should.Throw(() => thrower.ThrowingProperty);

            Assert.AreSame(ExpectedException, result.And);
        }

        [Test]
        public void FuncShouldThrow_DoesNotThrow_FailsWithNoExceptionMessage()
        {
            Expression<Func<object>> noThrow = () => "".Length;
            Error.NoException(typeof(Exception), noThrow, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() =>
                Should.Throw(noThrow, "foo"));
        }

        private class ExceptionThrower
        {
            private readonly Exception exception;

            public ExceptionThrower(Exception exception)
            {
                this.exception = exception;
            }

            public object ThrowingProperty
            {
                get { throw exception; }
            }

            public void Throw()
            {
                throw exception;
            }
        }
    }
}