using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class AssertionMethodTests
    {
        private AssertionMethod sut;

        [SetUp]
        public void SetUp()
        {
            sut = new AssertionMethod(new SourceAddress(), "method");
        }

        [Test]
        public void GetActualSegment()
        {
            ExpressionSegment result = sut.GetActualSegment("some.actual.method(param).someOtherMethod()", 5);

            Assert.AreEqual("actual", result.Expression);
            Assert.AreEqual(25, result.IndexOfNextSegment);
        }

        [Test]
        public void GetExpectedSegment_FirstParameter()
        {
            ExpressionSegment result = sut.GetExpectedSegment("actual.method(expected, \"message\").someOtherMethod()", 0);

            Assert.AreEqual("expected", result.Expression);
            Assert.AreEqual(34, result.IndexOfNextSegment);
        }

        [Test]
        public void GetExpectedSegment_OnlyParameter()
        {
            ExpressionSegment result = sut.GetExpectedSegment("actual.method(expected).someOtherMethod()", 0);

            Assert.AreEqual("expected", result.Expression);
            Assert.AreEqual(23, result.IndexOfNextSegment);
        }
    }
}