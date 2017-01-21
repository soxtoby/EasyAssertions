using NSubstitute;
using NUnit.Framework;
using System;

namespace EasyAssertions.UnitTests
{
    public abstract class AssertionTests
    {
        protected IFailureMessageFormatter MockFormatter;

        [SetUp]
        public void BaseSetUp()
        {
            MockFormatter = Substitute.For<IFailureMessageFormatter>();
            FailureMessageFormatter.Override(MockFormatter);
            SourceExpressionProvider.ForCurrentThread.Reset();
        }

        [TearDown]
        public void BaseTearDown()
        {
            FailureMessageFormatter.Default();
            SourceExpressionProvider.ForCurrentThread.Reset();
        }

        protected static void AssertReturnsActual<T>(T actual, Func<Actual<T>> assert)
        {
            Actual<T> result = assert();

            Assert.AreSame(actual, result.And);
        }

        protected static void AssertArgumentNullException(string paramName, TestDelegate assertionCall)
        {
            ArgumentNullException result = Assert.Throws<ArgumentNullException>(assertionCall);

            Assert.AreEqual(paramName, result.ParamName);
        }

        protected void AssertFailsWithTypesNotEqualMessage(Type expectedType, Type actualType, Action<string> assertionCall)
        {
            MockFormatter.NotEqual(expectedType, actualType, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => assertionCall("foo"));

            Assert.AreEqual("bar", result.Message);
        }
    }
}