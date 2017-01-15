using System;
using System.Linq.Expressions;
using NSubstitute;
using NUnit.Framework;
using System.Threading.Tasks;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class AsyncFunctionAssertionTests : AssertionTests
    {
        [Test]
        public async Task TaskShouldThrowType_ThrowsCorrectType_ReturnsException()
        {
            Exception expectedException = new Exception();
            AsyncExceptionThrower thrower = new AsyncExceptionThrower(expectedException);

            ActualException<Exception> result = await Should.Throw<Exception>(() => thrower.Throw());

            Assert.AreSame(expectedException, result.And);
        }

        [Test]
        public void TaskShouldThrowType_DoesNotThrow_FailsWithNoExceptionMessage()
        {
            Expression<Func<Task>> noThrow = () => Task.FromResult("".Trim());
            MockFormatter.NoException(typeof(Exception), noThrow, "foo").Returns("bar");

            EasyAssertionException result = Assert.ThrowsAsync<EasyAssertionException>(async () =>
                await Should.Throw<Exception>(noThrow, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void TaskShouldThrowType_ThrowsWrongType_FailsWithWrongExceptionMessage()
        {
            AsyncExceptionThrower thrower = new AsyncExceptionThrower(new Exception());
            Expression<Func<Task>> throwsException = () => thrower.Throw();
            MockFormatter.WrongException(typeof(InvalidOperationException), typeof(Exception), throwsException, "foo").Returns("bar");

            EasyAssertionException result = Assert.ThrowsAsync<EasyAssertionException>(async () =>
                await Should.Throw<InvalidOperationException>(throwsException, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void TaskShouldThrowType_ThrowsWrongType_FailsWithActualExceptionAsInnerException()
        {
            Exception expectedException = new Exception();
            AsyncExceptionThrower thrower = new AsyncExceptionThrower(expectedException);

            EasyAssertionException result = Assert.ThrowsAsync<EasyAssertionException>(async () =>
                await Should.Throw<InvalidOperationException>(() => thrower.Throw()));

            Assert.AreSame(expectedException, result.InnerException);
        }

        [Test]
        public async Task TaskShouldThrow_Throws_ReturnsException()
        {
            Exception expectedExeption = new Exception();
            AsyncExceptionThrower thrower = new AsyncExceptionThrower(expectedExeption);

            ActualException<Exception> result = await Should.Throw(() => thrower.Throw());

            Assert.AreSame(expectedExeption, result.And);
        }

        [Test]
        public void TaskShouldThrow_DoesNotThrow_FailsWithNoExceptionMessage()
        {
            Expression<Func<Task>> noThrow = () => Task.FromResult("".Length);
            MockFormatter.NoException(typeof(Exception), noThrow, "foo").Returns("bar");

            EasyAssertionException result = Assert.ThrowsAsync<EasyAssertionException>(async () =>
                await Should.Throw(noThrow, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        private class AsyncExceptionThrower
        {
            private readonly Exception _exception;

            public AsyncExceptionThrower(Exception exception)
            {
                _exception = exception;
            }

            public async Task Throw()
            {
                await Task.Yield(); // Forces the exception to be thrown in a continuation
                throw _exception;
            }
        }
    }
}