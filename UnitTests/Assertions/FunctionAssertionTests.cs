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
            ExceptionThrower thrower = new ExceptionThrower(ExpectedException);

            ActualException<Exception> result = Should.Throw<Exception>(() => thrower.Throw());

            Assert.AreSame(ExpectedException, result.And);
        }

        [Test]
        public void ActionShouldThrowType_DoesNotThrow_FailsWithNoExceptionMessage()
        {
            Expression<Action> noThrow = () => "".Trim();
            Error.NoException(typeof(Exception), noThrow, "foo").Returns(ExpectedException);

            Exception result = Assert.Throws<Exception>(() =>
                Should.Throw<Exception>(noThrow, "foo"));

            Assert.AreSame(ExpectedException, result);
        }

        [Test]
        public void ActionShouldThrowType_ThrowsWrongType_FailsWithWrongExceptionMessage()
        {
            Exception innerException = new Exception();
            ExceptionThrower thrower = new ExceptionThrower(innerException);
            Expression<Action> throwsException = () => thrower.Throw();
            Error.WrongException(typeof(InvalidOperationException), innerException, throwsException, "foo").Returns(ExpectedException);

            Exception result = Assert.Throws<Exception>(() =>
                Should.Throw<InvalidOperationException>(throwsException, "foo"));

            Assert.AreSame(ExpectedException, result);
        }

        [Test]
        public void FuncShouldThrowType_ThrowsCorrectType_ReturnsException()
        {
            ExceptionThrower thrower = new ExceptionThrower(ExpectedException);

            ActualException<Exception> result = Should.Throw<Exception>(() => thrower.ThrowingProperty);

            Assert.AreSame(ExpectedException, result.And);
        }

        [Test]
        public void FuncShouldThrowType_DoesNotThrow_FailsWithNoExceptionMessage()
        {
            Expression<Func<object>> noThrow = () => "".Length;
            Error.NoException(typeof(Exception), noThrow, "foo").Returns(ExpectedException);

            Exception result = Assert.Throws<Exception>(() =>
                Should.Throw<Exception>(noThrow, "foo"));

            Assert.AreSame(ExpectedException, result);
        }

        [Test]
        public void ActionShouldThrow_Throws_ReturnsException()
        {
            ExceptionThrower thrower = new ExceptionThrower(ExpectedException);

            ActualException<Exception> result = Should.Throw(() => thrower.Throw());

            Assert.AreSame(ExpectedException, result.And);
        }

        [Test]
        public void ActionShouldThrow_DoesNotThrow_FailsWithNoExceptionMessage()
        {
            Expression<Action> noThrow = () => "".Trim();
            Error.NoException(typeof(Exception), noThrow, "foo").Returns(ExpectedException);

            Exception result = Assert.Throws<Exception>(() =>
                Should.Throw(noThrow, "foo"));

            Assert.AreSame(ExpectedException, result);
        }

        [Test]
        public void FuncShouldThrow_Throws_ReturnsException()
        {
            ExceptionThrower thrower = new ExceptionThrower(ExpectedException);

            ActualException<Exception> result = Should.Throw(() => thrower.ThrowingProperty);

            Assert.AreSame(ExpectedException, result.And);
        }

        [Test]
        public void FuncShouldThrow_DoesNotThrow_FailsWithNoExceptionMessage()
        {
            Expression<Func<object>> noThrow = () => "".Length;
            Error.NoException(typeof(Exception), noThrow, "foo").Returns(ExpectedException);

            Exception result = Assert.Throws<Exception>(() =>
                Should.Throw(noThrow, "foo"));

            Assert.AreSame(ExpectedException, result);
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