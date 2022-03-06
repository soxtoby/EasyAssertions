using NSubstitute;
using NUnit.Framework;

namespace EasyAssertions.UnitTests;

[TestFixture]
public class ActualExceptionTests : AssertionTests
{
    [Test]
    public void CannotConstructWithNull()
    {
        AssertArgumentNullException("actual", () => new ActualException<Exception>(null!));
    }

    [Test]
    public void And_ReturnsException()
    {
        var exception = new Exception();
        var sut = new ActualException<Exception>(exception);

        Assert.AreSame(exception, sut.And);
    }

    [Test]
    public void AndShouldNotBeA_ExceptionIsNotSpecifiedType_ReturnsSelf()
    {
        var sut = new ActualException<Exception>(new Exception());

        var result = sut.AndShouldNotBeA<InvalidOperationException>();

        Assert.AreSame(sut, result);
    }

    [Test]
    public void AndShouldNotBeA_ExceptionIsSpecifiedType_FailsWithTypesAreEqualMessage()
    {
        Exception exception = new InvalidOperationException();
        Error.AreEqual(typeof(InvalidOperationException), typeof(InvalidOperationException), "foo").Returns(ExpectedException);
        var sut = new ActualException<Exception>(exception);

        AssertThrowsExpectedError(() => sut.AndShouldNotBeA<InvalidOperationException>("foo"));
    }
}