using NSubstitute;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class NumberAssertionTests : AssertionTests
    {
        /* // None of the lines in the following method should compile 
        public void ShouldNotCompile()
        {
            new object().ShouldBe(1f);
            new object().ShouldBe(1f, "message");
            1f.ShouldBe(1f);
            1f.ShouldBe(1f, "message");
            new object().ShouldBe(1d);
            new object().ShouldBe(1d, "message");
            1d.ShouldBe(1d);
            1d.ShouldBe(1d, "message");
        }/**/

        [Test]
        public void ShouldBe_FloatsWithinDelta_ReturnsActualValue()
        {
            const float actual = 1f;
            Actual<float> result = actual.ShouldBe(1f, 1f);

            Assert.AreEqual(actual, result.And);
        }

        [Test]
        public void ShouldBe_ActualNotAFloat_FailsWithTypesNotEqualMessage()
        {
            object actual = new object();
            MockFormatter.NotEqual(typeof(float), typeof(object), "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBe(1f, 1f, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldBe_FloatsOutsideDelta_FailsWithObjectsNotEqualMessage()
        {
            const float expected = 10f;
            const float actual = 1f;
            MockFormatter.NotEqual(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBe(expected, 0, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldBe_DoublesWithinDelta_ReturnsActualValue()
        {
            const double actual = 1d;

            Actual<double> result = actual.ShouldBe(1d, 0);

            Assert.AreEqual(actual, result.And);
        }

        [Test]
        public void ShouldBe_ActualNotADouble_FailsWithTypesNotEqualMessage()
        {
            object actual = new object();
            MockFormatter.NotEqual(typeof(double), typeof(object), "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBe(1d, 1d, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldBe_DoublesOutsideDelta_FailsWithObjectsNotEqualMessage()
        {
            const double expected = 10d;
            const double actual = 1d;
            MockFormatter.NotEqual(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBe(expected, 1d, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldNotBe_FloatsOutsideDelta_ReturnsActualValue()
        {
            const float actual = 1f;
            Actual<float> result = actual.ShouldNotBe(2f, 0);

            Assert.AreEqual(actual, result.And);
        }

        [Test]
        public void ShouldNotBe_FloatsWithinDelta_FailsWithObjectsEqualMessage()
        {
            const float actual = 1f;
            const float notExpected = 2f;
            MockFormatter.AreEqual(notExpected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotBe(notExpected, 1f, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldNotBe_DoublesOutsideDelta_ReturnsActualValue()
        {
            const double actual = 1d;

            Actual<double> result = actual.ShouldNotBe(2d, 0);

            Assert.AreEqual(actual, result.And);
        }

        [Test]
        public void ShouldNotBe_DoublesWithinDelta_FailsWithObjectsEqualMessage()
        {
            const double actual = 1d;
            const double notExpected = 2d;
            MockFormatter.AreEqual(notExpected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotBe(notExpected, 1d, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldBeGreaterThan_IsGreaterThan_ReturnsActualValue()
        {
            const int actual = 2;

            Actual<int> result = actual.ShouldBeGreaterThan(1);

            Assert.AreEqual(actual, result.And);
        }

        [Test]
        public void ShouldBeGreaterThan_NotGreaterThan_FailsWithNotGreaterThanMessage()
        {
            const int actual = 1;
            const int expected = 2;
            MockFormatter.NotGreaterThan(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeGreaterThan(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldBeLessThan_IsLessThan_ReturnsActualValue()
        {
            const int actual = 1;

            Actual<int> result = actual.ShouldBeLessThan(2);

            Assert.AreEqual(actual, result.And);
        }

        [Test]
        public void ShouldBeLessThan_NotLessThan_FailsWithNotLessThanMessage()
        {
            const int actual = 2;
            const int expected = 1;
            MockFormatter.NotLessThan(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeLessThan(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }
    }
}
