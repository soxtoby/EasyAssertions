using System;
using NSubstitute;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class EasyAssertionTests : AssertionTests
    {
        [Test]
        public void Assert_ReturnsActualValue()
        {
            var result = "foo".Assert(s => s.Length.ShouldBe(3));

            Assert.AreEqual("foo", result.And);
        }

        [Test]
        public void And_ReturnsActualValue()
        {
            var result = "foo".Assert(s => { }).And(s => s.Length.ShouldBe(3));

            Assert.AreEqual("foo", result.And);
        }

        [Test]
        public void ActionAssert_Passes_ReturnsActual()
        {
            var actual = new object();

            var result = actual.RegisterAssertion(c => { });

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void FuncAssert_Passes_ReturnsInnerActual()
        {
            var innerActual = new object();

            var result = new object().RegisterAssertion(c => new Actual<object>(innerActual));

            Assert.AreEqual(innerActual, result.And);
        }

        [Test]
        public void UseFrameworkExceptions_OverridesMessageException()
        {
            try
            {
                var expectedException = Substitute.For<Exception>();
                var exceptionFactory = Substitute.For<Func<string, Exception>>();
                exceptionFactory("foo").Returns(expectedException);

                EasyAssertion.UseFrameworkExceptions(exceptionFactory, null);
                var result = ErrorFactory.Instance.Custom("foo");

                Assert.AreSame(expectedException, result);
            }
            finally
            {
                EasyAssertion.UseEasyAssertionExceptions();
            }
        }

        [Test]
        public void UseFrameworkExceptions_OverridesInnerExceptionException()
        {
            try
            {
                var expectedInnerException = Substitute.For<Exception>();
                var expectedResultException = Substitute.For<Exception>();
                var exceptionFactory = Substitute.For<Func<string, Exception, Exception>>();
                exceptionFactory("foo", expectedInnerException).Returns(expectedResultException);

                EasyAssertion.UseFrameworkExceptions(null, exceptionFactory);
                var result = ErrorFactory.Instance.Custom("foo", expectedInnerException);

                Assert.AreSame(expectedResultException, result);
            }
            finally
            {
                EasyAssertion.UseEasyAssertionExceptions();
            }
        }

        [Test]
        public void UseEasyAssertionExceptions_ResetsExceptions()
        {
            EasyAssertion.UseFrameworkExceptions(Substitute.For<Func<string, Exception>>(), Substitute.For<Func<string, Exception, Exception>>());
            EasyAssertion.UseEasyAssertionExceptions();

            Assert.IsInstanceOf<EasyAssertionException>(ErrorFactory.Instance.Custom("foo"));
            Assert.IsInstanceOf<EasyAssertionException>(ErrorFactory.Instance.Custom("foo", new Exception()));
        }
    }
}