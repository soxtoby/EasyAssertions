using NSubstitute;
using NUnit.Framework;
using System;

namespace EasyAssertions.UnitTests
{
    public abstract class AssertionTests
    {
        protected IStandardErrors Error = null!;
        protected Exception ExpectedException = null!;

        [SetUp]
        public void BaseSetUp()
        {
            Error = Substitute.For<IStandardErrors>();
            ExpectedException = new Exception();
            StandardErrors.Override(Error);
            SourceExpressionProvider.ForCurrentThread.Reset();
        }

        [TearDown]
        public void BaseTearDown()
        {
            StandardErrors.Default();
            SourceExpressionProvider.ForCurrentThread.Reset();
        }

        protected static void AssertReturnsActual<T>(T actual, Func<Actual<T>> assert)
        {
            Actual<T> result = assert();

            Assert.AreSame(actual, result.And);
        }

        protected void AssertThrowsExpectedError(Action assert)
        {
            Exception result = Assert.Throws<Exception>(() => assert());

            Assert.AreSame(ExpectedException, result);
        }

        protected static void AssertArgumentNullException(string paramName, TestDelegate assertionCall)
        {
            ArgumentNullException result = Assert.Throws<ArgumentNullException>(assertionCall);

            Assert.AreEqual(paramName, result.ParamName);
        }

        protected void AssertFailsWithTypesNotEqualMessage(Type expectedType, Type? actualType, Action<string> assertionCall)
        {
            Error.NotEqual(expectedType, actualType, "foo").Returns(ExpectedException);

            Exception result = Assert.Throws<Exception>(() => assertionCall("foo"));

            Assert.AreSame(ExpectedException, result);
        }
    }
}