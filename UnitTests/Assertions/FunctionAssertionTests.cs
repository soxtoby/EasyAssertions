using System;
using System.Linq.Expressions;
using NSubstitute;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class FunctionAssertionTests : AssertionTests
    {
        [Test]
        public void ShouldThrow_Throws_ReturnsException()
        {
            Exception expectedException = new Exception();
            ExceptionThrower thrower = new ExceptionThrower(expectedException);

            Actual<Exception> result = Should.Throw<Exception>(() => thrower.Throw());

            Assert.AreSame(expectedException, result.And);
        }

        [Test]
        public void ShouldThrow_DoesNotThrow_FailsWithNoExceptionMessage()
        {
            Expression<Action> noThrow = () => "".Trim();
            MockFormatter.NoException(typeof(Exception), noThrow, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                Should.Throw<Exception>(noThrow, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldThrow_ThrowsWrongType_FailsWithWrongExceptionMessage()
        {
            ExceptionThrower thrower = new ExceptionThrower(new Exception());
            Expression<Action> throwsException = () => thrower.Throw();
            MockFormatter.WrongException(typeof(InvalidOperationException), typeof(Exception), throwsException, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                Should.Throw<InvalidOperationException>(throwsException, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldThrow_ThrowsWrongType_FailsWithActualExceptionAsInnerException()
        {
            Exception expectedException = new Exception();
            ExceptionThrower thrower = new ExceptionThrower(expectedException);

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                Should.Throw<InvalidOperationException>(() => thrower.Throw()));

            Assert.AreSame(expectedException, result.InnerException);
        }

        [Test]
        public void FuncShouldThrow_Throws_ReturnsException()
        {
            Exception expectedException = new Exception();
            ExceptionThrower thrower = new ExceptionThrower(expectedException);

            Actual<Exception> result = Should.Throw<Exception>(() => thrower.ThrowingProperty);

            Assert.AreSame(expectedException, result.And);
        }

        [Test]
        public void FuncShouldThrow_DoesNotThrow_FailsWithNoExceptionMessage()
        {
            Expression<Func<object>> noThrow = () => "".Length;
            MockFormatter.NoException(typeof(Exception), noThrow, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() =>
                Should.Throw<Exception>(noThrow, "foo"));

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