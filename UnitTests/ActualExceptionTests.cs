using NSubstitute;
using NUnit.Framework;
using System;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class ActualExceptionTests : AssertionTests
    {
        [Test]
        public void CannotConstructWithNull()
        {
            AssertArgumentNullException("actual", () => new ActualException<Exception>(null));
        }

        [Test]
        public void And_ReturnsException()
        {
            Exception exception = new Exception();
            ActualException<Exception> sut = new ActualException<Exception>(exception);

            Assert.AreSame(exception, sut.And);
        }

        [Test]
        public void AndShouldNotBeA_ExceptionIsNotSpecifiedType_ReturnsSelf()
        {
            ActualException<Exception> sut = new ActualException<Exception>(new Exception());

            ActualException<Exception> result = sut.AndShouldNotBeA<InvalidOperationException>();

            Assert.AreSame(sut, result);
        }

        [Test]
        public void AndShouldNotBeA_ExceptionIsSpecifiedType_FailsWithTypesAreEqualMessage()
        {
            Exception exception = new InvalidOperationException();
            MockFormatter.AreEqual(typeof(InvalidOperationException), typeof(InvalidOperationException), "foo").Returns("bar");
            ActualException<Exception> sut = new ActualException<Exception>(exception);

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => sut.AndShouldNotBeA<InvalidOperationException>("foo"));

            Assert.AreEqual("bar", result.Message);
        }
    }
}