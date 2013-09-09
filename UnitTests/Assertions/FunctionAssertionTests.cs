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
            Exception expectedException = new Exception();
            ExceptionThrower thrower = new ExceptionThrower(expectedException);

            ActualException<Exception> result = Should.Throw<Exception>(() => thrower.Throw());

            Assert.AreSame(expectedException, result.And);
        }

        [Test]
        public void ActionShouldThrowType_DoesNotThrow_FailsWithNoExceptionMessage()
        {
            Expression<Action> noThrow = () => "".Trim();
            MockFormatter.NoException(typeof(Exception), noThrow, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                Should.Throw<Exception>(noThrow, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ActionShouldThrowType_ThrowsWrongType_FailsWithWrongExceptionMessage()
        {
            ExceptionThrower thrower = new ExceptionThrower(new Exception());
            Expression<Action> throwsException = () => thrower.Throw();
            MockFormatter.WrongException(typeof(InvalidOperationException), typeof(Exception), throwsException, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                Should.Throw<InvalidOperationException>(throwsException, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ActionShouldThrowType_ThrowsWrongType_FailsWithActualExceptionAsInnerException()
        {
            Exception expectedException = new Exception();
            ExceptionThrower thrower = new ExceptionThrower(expectedException);

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                Should.Throw<InvalidOperationException>(() => thrower.Throw()));

            Assert.AreSame(expectedException, result.InnerException);
        }

        [Test]
        public void FuncShouldThrowType_ThrowsCorrectType_ReturnsException()
        {
            Exception expectedException = new Exception();
            ExceptionThrower thrower = new ExceptionThrower(expectedException);

            ActualException<Exception> result = Should.Throw<Exception>(() => thrower.ThrowingProperty);

            Assert.AreSame(expectedException, result.And);
        }

        [Test]
        public void FuncShouldThrowType_DoesNotThrow_FailsWithNoExceptionMessage()
        {
            Expression<Func<object>> noThrow = () => "".Length;
            MockFormatter.NoException(typeof(Exception), noThrow, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                Should.Throw<Exception>(noThrow, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ActionShouldThrow_Throws_ReturnsException()
        {
            Exception expectedExeption = new Exception();
            ExceptionThrower thrower = new ExceptionThrower(expectedExeption);

            ActualException<Exception> result = Should.Throw(() => thrower.Throw());

            Assert.AreSame(expectedExeption, result.And);
        }

        [Test]
        public void ActionShouldThrow_DoesNotThrow_FailsWithNoExceptionMessage()
        {
            Expression<Action> noThrow = () => "".Trim();
            MockFormatter.NoException(typeof(Exception), noThrow, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                Should.Throw(noThrow, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void FuncShouldThrow_Throws_ReturnsException()
        {
            Exception expectedException = new Exception();
            ExceptionThrower thrower = new ExceptionThrower(expectedException);

            ActualException<Exception> result = Should.Throw(() => thrower.ThrowingProperty);

            Assert.AreSame(expectedException, result.And);
        }

        [Test]
        public void FuncShouldThrow_DoesNotThrow_FailsWithNoExceptionMessage()
        {
            Expression<Func<object>> noThrow = () => "".Length;
            MockFormatter.NoException(typeof(Exception), noThrow, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                Should.Throw(noThrow, "foo"));

            Assert.AreEqual("bar", result.Message);
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