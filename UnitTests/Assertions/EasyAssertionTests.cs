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
            Actual<string> result = "foo".Assert(s => s.Length.ShouldBe(3));

            Assert.AreEqual("foo", result.And);
        }

        [Test]
        public void And_ReturnsActualValue()
        {
            Actual<string> result = "foo".Assert(s => { }).And(s => s.Length.ShouldBe(3));

            Assert.AreEqual("foo", result.And);
        }

        [Test]
        public void ActionAssert_Passes_ReturnsActual()
        {
            object actual = new object();

            Actual<object> result = actual.RegisterAssert(a => { });

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void FuncAssert_Passes_ReturnsInnerActual()
        {
            object innerActual = new object();

            Actual<object> result = new object().RegisterAssert(i => new Actual<object>(innerActual));

            Assert.AreEqual(innerActual, result.And);
        }

        [Test]
        public void UseFrameworkExceptions_OverridesMessageException()
        {
            try
            {
                Exception expectedException = Substitute.For<Exception>();
                Func<string, Exception> exceptionFactory = Substitute.For<Func<string, Exception>>();
                exceptionFactory("foo").Returns(expectedException);

                EasyAssertions.UseFrameworkExceptions(exceptionFactory, null);
                Exception result = EasyAssertions.Failure("foo");

                Assert.AreSame(expectedException, result);
            }
            finally
            {
                EasyAssertions.UseEasyAssertionExceptions();
            }
        }

        [Test]
        public void UseFrameworkExceptions_OverridesInnerExceptionException()
        {
            try
            {
                Exception expectedInnerException = Substitute.For<Exception>();
                Exception expectedResultException = Substitute.For<Exception>();
                Func<string, Exception, Exception> exceptionFactory = Substitute.For<Func<string, Exception, Exception>>();
                exceptionFactory("foo", expectedInnerException).Returns(expectedResultException);

                EasyAssertions.UseFrameworkExceptions(null, exceptionFactory);
                Exception result = EasyAssertions.Failure("foo", expectedInnerException);

                Assert.AreSame(expectedResultException, result);
            }
            finally
            {
                EasyAssertions.UseEasyAssertionExceptions();
            }
        }

        [Test]
        public void UseEasyAssertionExceptions_ResetsExceptions()
        {
            EasyAssertions.UseFrameworkExceptions(s => new Exception(s), (s, exception) => new Exception(s, exception));
            EasyAssertions.UseEasyAssertionExceptions();

            Assert.IsInstanceOf<EasyAssertionException>(EasyAssertions.Failure("foo"));
            Assert.IsInstanceOf<EasyAssertionException>(EasyAssertions.Failure("foo", new Exception()));
        }
    }
}