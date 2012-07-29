using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class ContinuationTests
    {
        private Continuation sut;

        [SetUp]
        public void SetUp()
        {
            sut = new Continuation(new SourceAddress(), "property");
        }

        [Test]
        public void GetActualSegment()
        {
            ExpressionSegment result = sut.GetActualSegment("foo.property.bar", 3);

            Assert.AreEqual(string.Empty, result.Expression);
            Assert.AreEqual(12, result.IndexOfNextSegment);
        }

        [Test]
        public void GetActualSegment_PropertyNotInSource_ReturnsEmptySegment()
        {
            const int fromIndex = 1;
            ExpressionSegment result = sut.GetActualSegment("foo.bar", fromIndex);

            Assert.AreEqual(string.Empty, result.Expression);
            Assert.AreEqual(fromIndex, result.IndexOfNextSegment);
        }

        [Test]
        public void GetExpectedSegment()
        {
            ExpressionSegment result = sut.GetExpectedSegment("foo.property.bar", 3);

            Assert.AreEqual(string.Empty, result.Expression);
            Assert.AreEqual(12, result.IndexOfNextSegment);
        }

        [Test]
        public void GetExpectedSegment_PropertyNotInSource_ReturnsEmptySegment()
        {
            const int fromIndex = 1;
            ExpressionSegment result = sut.GetExpectedSegment("foo.bar", fromIndex);

            Assert.AreEqual(string.Empty, result.Expression);
            Assert.AreEqual(fromIndex, result.IndexOfNextSegment);
        }

    }
}